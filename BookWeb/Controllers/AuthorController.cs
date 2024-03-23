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

		public IActionResult AuthorCreate()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AuthorCreate(AuthorCreateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var author = new Author
				{
					FullName = model.FullName,
					Biography = model.Biography,
					ImageUrl = "default.png"
				};

				_context.Author.Add(author);
				_context.SaveChanges();

				return RedirectToAction("AuthorList");
			}

			return View(model);
		}

		public IActionResult AuthorList()
		{
			var authors = _context.Author.ToList();
			return View(authors);
		}


	}
}
