﻿using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		// REGİSTER

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new AppUser
				{
					FullName = model.FullName,
					UserName = model.UserName.ToLower(),    // kullanıcı ismini küçük harflerle sınırlandıralım
					Email = model.Email,
					ImageUrl = "default.jpg"
				};
				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					// Başarılı kayıt işlemi
					return RedirectToAction("Login", "Account");
				}
				// Başarısız kayıt sonrası hatayı göster
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}

		// LOGİN

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				// mail bilgisine göre user'ları getir
				var user = await _userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					// erişim sağlanma durumu olabilir, direkt oturum dışı ediyoruz, login yapılacağı için
					await _signInManager.SignOutAsync();
					// girdi sağlayan kullanıcının bilgilerinin eşleşmesi için sorguyla kontrol ediyoruz
					var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
					// sorgu olumlu ise
					if (result.Succeeded)
					{
						await _userManager.ResetAccessFailedCountAsync(user);
						await _userManager.SetLockoutEndDateAsync(user, null);  // süreler sıfırlandı

						return RedirectToAction("Index", "Home");
					}
					// hesap kitlenmesi mevcut durumda ise, bilgilendirme
					else if (result.IsLockedOut)
					{
						var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
						var timeLeft = lockoutDate.Value - DateTime.UtcNow;
						ModelState.AddModelError("", $"Hesabınız kitlendi, Lütfen {timeLeft.Minutes} dakika sonra tekrar deneyiniz.");
					}
					// sorgu olumsuz ise
					else
					{
						ModelState.AddModelError("", "Parolanız hatalı!");
					}
				}
				else
				{
					ModelState.AddModelError("", "Bu Email adresi ile bir hesap bulunamadı!");
				}

			}
			return View(model);
		}

		// LOGOUT

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}

	}
}