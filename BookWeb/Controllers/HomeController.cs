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

        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                PopularBooks = _context.Books.ToList()
            };
            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

    }
}
