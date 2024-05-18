﻿using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookWeb.Controllers
{
	public class CommentsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly BookContext _context;

		public CommentsController(UserManager<AppUser> userManager, BookContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> CommentCreate()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound();
			}

			var activeUserBook = _context.UserBooks.Include(ub => ub.Book).ThenInclude(b => b.Author)
												   .FirstOrDefault(ub => ub.UserId == user.Id && ub.IsActive);

			if (activeUserBook == null || activeUserBook.Book == null)
			{
				// Kullanıcının aktif kitabı bulunamadı veya kitap bilgisi eksik
				ModelState.AddModelError("", "You don't have an active book or the book information is missing.");
				return RedirectToAction("Activate", "UserBook");
			}

			var viewModel = new CommentViewModel
			{
				BookId = activeUserBook.BookId,
				Title = activeUserBook.Book.Title,
				AuthorFullName = activeUserBook.Book.Author.FullName,
				Publisher = activeUserBook.Book.Publisher,
				ImageUrl = activeUserBook.Book.ImageUrl, // Kitap resmi URL'si

				UserId = user.Id,
				UserName = user.UserName,
				FullName = user.FullName,
				Email = user.Email,
				UserImageUrl = user.ImageUrl, // Kullanıcı resmi URL'si
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> CommentCreate(CommentViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);

				if (user != null)
				{
					var existingComment = await _context.Comments
						.FirstOrDefaultAsync(c => c.BookId == model.BookId && c.UserId == user.Id);

					if (existingComment != null)
					{
						ModelState.AddModelError("", "You have already commented on this book.");
					}
					else
					{
						var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == model.BookId);

						if (book != null)
						{
							var comment = new Comment
							{
								Text = model.Text,
								BookId = model.BookId,
								UserId = user.Id,
								DateTime = DateTime.Now
							};

							_context.Comments.Add(comment);
							await _context.SaveChangesAsync();

							TempData["SuccessMessage"] = "Your comment has been added successfully.";
							return RedirectToAction("Profile", "Account");
						}
						else
						{
							ModelState.AddModelError("", "The book was not found.");
						}
					}
				}
				else
				{
					ModelState.AddModelError("", "User not found.");
				}
			}

			// Hata durumunda tekrar kitap ve kullanıcı bilgilerini ekleyerek view'e döneriz
			var bookForModel = await _context.Books.FindAsync(model.BookId);
			var currentUser = await _userManager.GetUserAsync(User);

			if (bookForModel != null && currentUser != null)
			{
				model.Title = bookForModel.Title;
				model.Publisher = bookForModel.Publisher;
				model.ImageUrl = bookForModel.ImageUrl;

				model.UserName = currentUser.UserName;
				model.FullName = currentUser.FullName;
				model.Email = currentUser.Email;
				model.UserImageUrl = currentUser.ImageUrl;
			}

			return View(model);
		}




	}
}
