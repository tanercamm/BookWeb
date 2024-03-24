using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
	public class AuthorController : Controller
	{
		private readonly BookContext _context;
		public AuthorController(BookContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult AuthorCreate()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AuthorCreate(AuthorCreateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var author = new Author
				{
					FullName = model.FullName,
					Biography = model.Biography,
					ImageAuthor = "default.png"
				};

				_context.Authors.Add(author);
				_context.SaveChanges();

				return RedirectToAction("AuthorList");
			}

			return View(model);
		}

		public IActionResult AuthorList()
		{
			// Author varlık sınıfından AuthorViewModel sınıfına dönüşüm yap
			var authors = _context.Authors.Select(m => new AuthorViewModel
			{
				AuthorId = m.AuthorId,
				FullName = m.FullName,
				Biography = m.Biography,
				ImageAuthor = "default.png"
			}).ToList();

			// AuthorViewModel listesini görünüme gönder
			return View(authors);
		}


	}
}
