using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult ErrorPage()
		{
			return View();
		}
	}
}
