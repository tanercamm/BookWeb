using System.Diagnostics;
using BookWeb.Data;
using BookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly BookContext _context;
		public HomeController(BookContext context)
		{
			_context = context;
		}

		public int pageSize = 10;

		public IActionResult Index(int page = 1)
		{
			var model = new HomePageViewModel
			{
				PopularBooks = _context.Books.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
				PageInfo = new PageInfo()
				{
					ItemsPerPage = pageSize,
					TotalItems = _context.Books.Count()
				}
			};
			return View(model);
		}

		public IActionResult About()
		{
			return View();
		}

	}
}
