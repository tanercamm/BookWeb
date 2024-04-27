using BookWeb.Data;
using BookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
	[Authorize]
	public class BooksController : Controller
	{
		private readonly BookContext _context;

		public BooksController(BookContext context)
		{
			_context = context;
		}

		public int pageSize = 10;

		public IActionResult List(int? id, string q, int page = 1)
		{
			//IEnumerable belleğe atıp, ardından sorgular
			//IQueryable direkt olarak server üzerinden sorgular (hız avantajı)
			var books = _context.Books.AsQueryable();

			// tür id göre kimlik atıyoruz
			if (id != null)
			{
				books = books.Include(m => m.Genres).Where(m => m.Genres.Any(g => g.GenreId == id));
			}

			if (!string.IsNullOrEmpty(q)) // q parametresi boş olmadığı koşul
			{
				books = books.Where(i =>
					i.Title.ToLower().Contains(q.ToLower()) ||
					i.Description.ToLower().Contains(q.ToLower())
				);  // ilgili string ifadeyi Title veya Description içerisinde bulma sorgusu
			}

			var model = new BooksViewModel()
			{
				Books = books.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
				PageInfo = new PageInfo
				{
					ItemsPerPage = pageSize,
					TotalItems = _context.Books.Count()
				}
			};

			return View("Books", model);
		}

		// Kitap için yazar bilgilerini almak için bir metot
		public IActionResult GetAuthorInfoForBook(int bookId)
		{
			// Belirli bir kitabın AuthorId değerine göre ilgili yazarın bilgilerini al
			var book = _context.Books
							   .Include(b => b.Author)
							   .FirstOrDefault(b => b.BookId == bookId);

			// Eğer kitap null değilse ve yazar bilgisi varsa, yazar bilgilerini görüntüle
			if (book != null && book.Author != null)
			{
				var authorFullName = book.Author.FullName;
				// Diğer yazar bilgilerini kullanabilirsiniz.
				return Ok(new { AuthorFullName = authorFullName});
			}
			else
			{
				return NotFound(); // Kitap veya yazar bulunamazsa 404 döndür
			}
		}


		public IActionResult Details(int id)
		{
			var book = _context.Books.Find(id);

			if (book == null)
			{
				return NotFound();
			}

			// Eğer kitap bulunursa, yazar bilgilerini almak için GetAuthorInfoForBook metodunu çağır
			var authorInfoResult = GetAuthorInfoForBook(book.BookId);

			// Eğer yazar bilgileri başarıyla alınırsa, yazar bilgilerini ViewData üzerinden Details.cshtml görünümüne gönder
			if (authorInfoResult is OkObjectResult okResult)
			{
				var authorInfo = okResult.Value as dynamic;
				ViewData["AuthorFullName"] = authorInfo.AuthorFullName;

				ViewData["AuthorId"] = book.AuthorId;

			}

			return View(book);
		}



	}
}
