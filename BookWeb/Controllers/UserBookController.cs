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

				// Kullanıcının daha önce aktif ettiği bir kitabı var mı kontrol edelim
				var alreadyActivatedBook = _context.UserBooks.Any(ub => ub.UserId == user.Id && ub.IsActive && ub.IsPrimaryAccess);


				if (alreadyActivatedBook)
				{
					// Kullanıcı zaten bir kitabı aktive etmiş, işlemi gerçekleştiremeyiz
					ModelState.AddModelError("", "Zaten aktive edilmiş kitabınız var. Lütfen yenisini etkinleştirmeden önce iade ediniz!");
					return View("Index", "Home");
				}

				// Kullanıcının daha önce bu kitabı aktive edip etmediğini kontrol edelim
				var existingUserBook = _context.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.BookId == viewModel.BookId);


				if (existingUserBook != null)
				{
					// Kullanıcı bu kitabı daha önce aktive etmiş, sadece aktif hale getirelim
					existingUserBook.IsActive = true;
					existingUserBook.IsPrimaryAccess = true;

					// Kitabın IsActive durumunu false yap
					var bookToUpdate = await _context.Books.FindAsync(viewModel.BookId);
					if (bookToUpdate != null)
					{
						bookToUpdate.IsActive = false;
					}

				}
				else
				{
					// Kullanıcı bu kitabı daha önce aktive etmemiş, yeni bir UserBook oluşturalım
					var newUserBook = new UserBook
					{
						UserId = user.Id,
						BookId = viewModel.BookId,
						IsActive = true,
						IsPrimaryAccess = true
					};

					_context.UserBooks.Add(newUserBook);

					// Kitabın IsActive durumunu false yap
					var bookToUpdate = await _context.Books.FindAsync(viewModel.BookId);
					if (bookToUpdate != null)
					{
						bookToUpdate.IsActive = false;
					}
				}

				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Kitabı başarıyla etkinleştirdiniz!";
				return RedirectToAction("Index", "Home");
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
