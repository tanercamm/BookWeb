﻿using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
	public class RolesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
