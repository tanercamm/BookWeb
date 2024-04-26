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

		public IActionResult Details(int id)
		{
			var book = _context.Books.Find(id);
			return View(book);
		}

	}
}
