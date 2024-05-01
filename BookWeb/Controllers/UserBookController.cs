using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
	public class UserBookController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly BookContext _context;

		public UserBookController(UserManager<AppUser> userManager, BookContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		// Kitap için yazar bilgilerini almak için bir metot
		public async Task<IActionResult> GetAuthorInfoForBook(int bookId)
		{
			// Belirli bir kitabın AuthorId değerine göre ilgili yazarın bilgilerini al
			var book = await _context.Books
							   .Include(b => b.Author)
							   .FirstOrDefaultAsync(b => b.BookId == bookId);

			// Eğer kitap null değilse ve yazar bilgisi varsa, yazar bilgilerini görüntüle
			if (book != null && book.Author != null)
			{
				var authorFullName = book.Author.FullName;

				return Ok(new { AuthorFullName = authorFullName });
			}
			else
			{
				return NotFound(); // Kitap veya yazar bulunamazsa 404 döndür
			}
		}

		public async Task<IActionResult> Activate(int id)
		{
			var book = await _context.Books.FindAsync(id);

			if (book == null)
			{
				return NotFound();
			}

			// Oturum açmış kullanıcının bilgilerini al
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				ModelState.AddModelError("", "Kullanıcı bulunamadı tekrardan login olunuz!");
				return View("Login", "Account");
			}

			// Eğer kitap bulunursa, yazar bilgilerini almak için GetAuthorInfoForBook metodunu çağır
			var authorInfoResult = await GetAuthorInfoForBook(book.BookId);

			// Eğer yazar bilgileri başarıyla alınırsa, yazar bilgilerini ViewData üzerinden Activate.cshtml görünümüne gönder
			if (authorInfoResult is OkObjectResult okResult)
			{
				var authorInfo = okResult.Value as dynamic;
				ViewData["AuthorFullName"] = authorInfo.AuthorFullName;

				ViewData["AuthorId"] = book.AuthorId;
			}

			var viewModel = new UserBookViewModel
			{
				BookId = book.BookId,
				BookTitle = book.Title,
				BookPublisher = book.Publisher,
				BookDescription = book.Description,
				BookImageUrl = book.ImageUrl,

				UserId = user.Id,
				UserFullName = user.FullName,
				UserUserName = user.UserName,
				UserEmail = user.Email
			};

			return View("Activate", viewModel); // Activate.cshtml görünümünü kitap modeliyle birlikte döndür
		}

		[HttpPost]
		public async Task<IActionResult> Activate(UserBookViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);

				if (user == null)
				{
					ModelState.AddModelError("", "User not found, log in again!");
					TempData["ErrorMessage"] = "User not found, log in again!";
					return View("Login", "Account");
				}

				// Kullanıcının daha önce aktif ettiği bir kitabı var mı kontrol edelim
				var activeUserBooks = _context.UserBooks.Where(ub => ub.UserId == user.Id && ub.IsActive).ToList();

				if (activeUserBooks.Any())
				{
					// Kullanıcı zaten aktif bir kitabı var, işlemi gerçekleştiremeyiz
					ModelState.AddModelError("", "You already have an active book. Please return your current book before activating a new one!");
					TempData["ErrorMessage"] = "You already have an active book. Please return your current book before activating a new one!";
					return RedirectToAction("Profile", "Account");
				}

				var existingUserBook = _context.UserBooks.FirstOrDefault(ub => ub.BookId == viewModel.BookId && ub.IsActive);

				if (existingUserBook != null)
				{
					// Kitap zaten aktif edilmiş, başka bir kullanıcı tarafından
					ModelState.AddModelError("", "The relevant book was activated by another user. Please try again later!");
					TempData["ErrorMessage"] = "The relevant book was activated by another user. Please try again later!";
					return RedirectToAction("Profile", "Account");
				}
				// Kullanıcı bu kitabı daha önce aktive etmemiş, yeni bir UserBook oluşturalım
				var newUserBook = new UserBook
				{
					UserId = user.Id,
					BookId = viewModel.BookId,
					IsActive = true
				};

				_context.UserBooks.Add(newUserBook);

				// Kitabın IsActive durumunu false yap
				var bookToUpdate = await _context.Books.FindAsync(viewModel.BookId);
				if (bookToUpdate != null)
				{
					bookToUpdate.IsActive = false;
				}


				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "You have successfully activated the book!";
				return RedirectToAction("Profile", "Account");
			}
			else
			{
				// ModelState hatalarını kullanıcıya gösterin
				foreach (var modelStateValue in ModelState.Values)
				{
					foreach (var error in modelStateValue.Errors)
					{
						ModelState.AddModelError("", error.ErrorMessage);
					}
				}
				// Activate View'ını tekrar gösterin
				return View(viewModel);
			}
		}

	}
}
