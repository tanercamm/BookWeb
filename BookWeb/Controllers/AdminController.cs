using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
	public class AdminController : Controller
	{
		private readonly BookContext _context;
		public AdminController(BookContext context)
		{
			_context = context;
		}

		// BOOK

		[HttpGet]
		public IActionResult BookCreate()
		{
			var viewModel = new AdminCreateBookViewModel();

			// Retrieve authors from the database and populate the Authors property of the view model
			viewModel.Authors = _context.Authors
				.Select(author => new AuthorViewModel
				{
					AuthorId = author.AuthorId,
					FullName = author.FullName,
					Biography = author.Biography,
					ImageAuthor = author.ImageAuthor
				})
				.ToList();

			if (viewModel.Authors == null)
			{
				viewModel.Authors = new List<AuthorViewModel>();
			}

			ViewBag.Genres = _context.Genres.ToList();
			return View(viewModel);
		}

		[HttpPost]
		public IActionResult BookCreate(AdminCreateBookViewModel model)
		{
			if (model.Title != null && model.Title.Contains("@"))
			{
				ModelState.AddModelError("", "Kitap başlığı '@' işareti içeremez!");
			}
			if (ModelState.IsValid)
			{
				var entity = new Book
				{
					Title = model.Title,
					Description = model.Description,
					Publisher = model.Publisher,
					PageCount = model.PageCount,
					ImageUrl = "default.jpg"
				};

				//foreach (var id in model.GenreIds)
				//{
				//	entity.Genres.Add(_context.Genres.FirstOrDefault(i => i.GenreId == id));
				//}
				entity.Genres = _context.Genres.Where(g => model.GenreIds.Contains(g.GenreId)).ToList();

				// Retrieve the author from the database based on the selected AuthorId
				var author = _context.Authors.FirstOrDefault(a => a.AuthorId == model.AuthorId);
				if (author == null)
				{
					ModelState.AddModelError("", "Geçersiz yazar seçimi!");
					ViewBag.Genres = _context.Genres.ToList();
					return View(model);
				}

				// Associate the author with the book
				entity.Author = author;

				_context.Books.Add(entity);
				_context.SaveChanges();
				return RedirectToAction("BookList");
			}
			ViewBag.Genres = _context.Genres.ToList();
			return View(model);
		}

		public IActionResult BookDelete(int bookIds)
		{
			var entity = _context.Books.Find(bookIds);

			if (entity != null)
			{
				_context.Books.Remove(entity);
				_context.SaveChanges();
			}
			return RedirectToAction("BookList");
		}

		public IActionResult BookList()
		{
			var books = new AdminBooksViewModel
			{
				Books = _context.Books
							.Include(m => m.Genres)
							.Select(m => new AdminBookViewModel
							{
								BookId = m.BookId,
								Title = m.Title,
								Publisher = m.Publisher,
								PageCount = m.PageCount,
								ImageUrl = m.ImageUrl,
								Genres = m.Genres.ToList()
							}).ToList()
			};
			return View(books);
		}



		// GENRE

		private AdminGenresViewModel GetGenres()
		{
			var genres = new AdminGenresViewModel
			{
				Genres = _context.Genres.Select(g => new AdminGenreViewModel
				{
					GenreId = g.GenreId,
					Name = g.Name,
					Count = g.Books.Count()
				}).ToList()
			};
			return genres;
		}

		public IActionResult GenreList()
		{
			return View(GetGenres());
		}

		[HttpPost]
		public IActionResult GenreCreate(AdminGenresViewModel model)
		{
			ModelState.Remove("Genres");  // Genres özelliği ModalState'den kaldırılır

			if (model.Name != null && model.Name.Length < 3)
			{
				ModelState.AddModelError("Name", "Tür adı minimum 3 karakterli olmalıdır!");
			}

			if (ModelState.IsValid)
			{
				_context.Genres.Add(new Genre { Name = model.Name });
				_context.SaveChanges();
				return RedirectToAction("GenreList");
			}

			return View("GenreList", GetGenres());
		}

		[HttpPost]
		public IActionResult GenreDelete(int genreId)
		{
			var entity = _context.Genres.Find(genreId);

			if (entity != null)
			{
				_context.Genres.Remove(entity);
				_context.SaveChanges();
			}
			return RedirectToAction("GenreList");
		}



	}
}
