using System.Net;
using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
	[Authorize]
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

		// Kitabı aktive etme

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

				var activeUserBook = _context.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.IsActive);
				if (activeUserBook != null)
				{
					// Kullanıcı zaten aktif bir kitabı var, işlemi gerçekleştiremeyiz
					ModelState.AddModelError("", "You already have an active book. Please return your current book before activating a new one!");
					TempData["ErrorMessage"] = "You already have an active book. Please return your current book before activating a new one!";
					return RedirectToAction("Profile", "Account");
				}

				var existingUserBook = _context.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.BookId == viewModel.BookId && ub.IsActive);
				if (existingUserBook != null)
				{
					// Kitap zaten aktif edilmiş, başka bir kullanıcı tarafından
					ModelState.AddModelError("", "The relevant book was activated by another user. Please try again later!");
					TempData["ErrorMessage"] = "The relevant book was activated by another user. Please try again later!";
					return RedirectToAction("Profile", "Account");
				}

				// Kullanıcı bu kitabı daha önce aktive etmemiş, yeni bir UserBook oluşturalım veya mevcut kitabı aktif hale getirelim
				var newUserBook = _context.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.BookId == viewModel.BookId);
				if (newUserBook != null)
				{
					// Mevcut kaydı güncelleyin
					newUserBook.IsActive = true;
				}
				else
				{
					// Yeni bir kayıt oluşturun
					newUserBook = new UserBook
					{
						UserId = user.Id,
						BookId = viewModel.BookId,
						IsActive = true
					};
					_context.UserBooks.Add(newUserBook);
				}

				// Kitabın IsEmpty durumunu false yap
				var bookToUpdate = await _context.Books.FindAsync(viewModel.BookId);
				if (bookToUpdate != null)
				{
					bookToUpdate.IsEmpty = false;
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

		// Kitabı teslim etme

		[HttpPost]
		public async Task<IActionResult> Deliver(int bookId)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				TempData["ErrorMessage"] = "User not found, log in again!";
				return RedirectToAction("Login", "Account");
			}

			// Kullanıcının aktif kitabını kontrol et
			var activeUserBook = _context.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.BookId == bookId && ub.IsActive);

			if (activeUserBook == null)
			{
				// Kullanıcının bu kitabı aktif etme yetkisi yok veya kitap zaten iade edilmiş
				TempData["ErrorMessage"] = "You don't have permission to deliver this book or the book is already delivered!";
				return RedirectToAction("Profile", "Account");
			}

            // Kullanıcının bu kitap hakkında yorum yapıp yapmadığını kontrol et
            var userComment = _context.Comments.FirstOrDefault(c => c.UserId == user.Id && c.BookId == bookId);
            if (userComment == null)
            {
                // Kullanıcı bu kitap hakkında yorum yapmamış, yorum yapma sayfasına yönlendir
                TempData["ErrorMessage"] = "You need to write a comment before delivering the book!";
                return RedirectToAction("Profile", "Account");
            }

            // Kitabın durumunu false (bağlantılı değil) yapılsın
            activeUserBook.IsActive = false;

			// Kitabın IsEmpty durumunu true (kullanılabilir) yap
			var bookToUpdate = await _context.Books.FindAsync(bookId);
			if (bookToUpdate != null)
			{
				bookToUpdate.IsEmpty = true;
			}

			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "You have successfully delivered the book!";
			return RedirectToAction("Profile", "Account");
		}

	}
}
