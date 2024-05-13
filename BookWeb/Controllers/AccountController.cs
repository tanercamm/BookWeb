using System.IO;
using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
	public class AccountController : Controller
	{
		private readonly BookContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, BookContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
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
					// Kullanıcı başarıyla kaydedildi, rol ataması yapılacak
					await _userManager.AddToRoleAsync(user, "customer");

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

		// LOGIN

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

		// PROFILE

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToAction("Login");
			}

			var model = new UserProfileViewModel
			{
				FullName = user.FullName,
				UserName = user.UserName,
				Email = user.Email,
				ImageUrl = user.ImageUrl
			};

			var userBooks = _context.UserBooks
				.Where(ub => ub.UserId == user.Id && ub.IsActive)
				.Select(ub => new UserBookViewModel
				{
					BookId = ub.BookId,
					BookTitle = ub.Book.Title,
					BookPublisher = ub.Book.Publisher,
					BookDescription = ub.Book.Description,
					BookImageUrl = ub.Book.ImageUrl,
					UserId = user.Id,
					UserFullName = user.FullName,
					UserUserName = user.UserName,
					UserEmail = user.Email
				})
				.ToList();

			model.UserBooks = userBooks;

			return View(model);
		}

		// Profile Update

		[HttpGet]
		public async Task<IActionResult> ProfileUpdate()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToAction("Login");
			}

			var model = new ProfileUpdateViewModel
			{
				Id = user.Id,
				FullName = user.FullName,
				UserName = user.UserName,
				Email = user.Email,
				ImageUrl = user.ImageUrl
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ProfileUpdate(ProfileUpdateViewModel model, IFormFile file)
		{
			ModelState.Remove("FullName");
			ModelState.Remove("UserName");
			ModelState.Remove("Email");

			if (ModelState.IsValid)
			{
				var entity = await _userManager.GetUserAsync(User);

				if (entity == null)
				{
					return NotFound();
				}

				// fotoğraf güncelleme
				if (file != null)
				{
					var extension = Path.GetExtension(file.FileName);
					var fileName = Path.GetFileName(file.FileName);
					var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\users", fileName);
					entity.ImageUrl = fileName;
					using (var stream = new FileStream(path, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}
				}
				else // yeni dosya yüklenmemişse, mevcut resmi tanımlarız
				{
					entity.ImageUrl = model.ImageUrl;
				}

				// Şifre değiştirme kontrolleri
				if (model.isChangePassword)
				{
					if (string.IsNullOrEmpty(model.CurrentPassword))
					{
						ModelState.AddModelError(string.Empty, "Please enter the current password.");
						return View(model);
					}

					var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(entity, model.CurrentPassword);
					if (!isCurrentPasswordValid)
					{
						ModelState.AddModelError(string.Empty, "The current password is incorrect.");
						return View(model);
					}

					if (string.IsNullOrEmpty(model.NewPassword))
					{
						ModelState.AddModelError(string.Empty, "Please enter the new password.");
						return View(model);
					}

					if (model.NewPassword == model.CurrentPassword)
					{
						ModelState.AddModelError(string.Empty, "The new password cannot be the same as the old password.");
						return View(model);
					}

					if (model.NewPassword != model.ConfirmPassword)
					{
						ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
						return View(model);
					}

					// Şifre değiştirme işlemi
					var changePasswordResult = await _userManager.ChangePasswordAsync(entity, model.CurrentPassword, model.NewPassword);
					if (!changePasswordResult.Succeeded)
					{
						foreach (var error in changePasswordResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
						return View(model);
					}
				}
				await _userManager.UpdateAsync(entity);
				return RedirectToAction("Profile");
			}
			return View(model);
		}

	}
}

