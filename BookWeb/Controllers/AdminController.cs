using BookWeb.Data;
using BookWeb.Entity;
using BookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly BookContext _context;
        public AdminController(BookContext context)
        {
            _context = context;
        }

        // BOOK
        private AuthorViewModel GetAuthors()
        {
            var model = new AuthorViewModel();
            model.Authors = _context.Authors.Select(m => new AuthorEditViewModel
            {
                AuthorId = m.AuthorId,
                FullName = m.FullName,
                Biography = m.Biography,
                ImageAuthor = m.ImageAuthor
            }).ToList();
            return model;
        }

        [HttpGet]
        public IActionResult BookCreate()
        {
            var viewModel = new AdminCreateBookViewModel();

            // Yazarları doldur
            viewModel.Authors = GetAuthors().Authors;
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

                if (model.Authors == null)
                {
                    model.Authors = GetAuthors().Authors; // Örnek bir metot çağrısı, gerçek verileri nasıl alıyorsanız ona göre düzenleyin
                }

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

        [HttpPost]
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
                                IsActive = m.IsActive,
                                Genres = m.Genres.ToList(),
                                Author = new AuthorEditViewModel
                                {
                                    AuthorId = m.Author.AuthorId,
                                    ImageAuthor = m.Author.ImageAuthor,
                                    FullName = m.Author.FullName,
                                    Biography = m.Author.Biography,
                                }
                            }).ToList()
            };
            return View(books);
        }

        [HttpGet]
        public IActionResult BookEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _context.Books.Select(b => new AdminEditBookViewModel
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                Publisher = b.Publisher,
                PageCount = b.PageCount,
                ImageUrl = b.ImageUrl,
                IsActive = b.IsActive,
                AuthorId = b.AuthorId,
                GenreIds = b.Genres.Select(g => g.GenreId).ToArray()
            })
        .FirstOrDefault(m => m.BookId == id);

            if (entity == null)
            {
                return NotFound();
            }


            entity.Authors = _context.Authors.Select(a => new AuthorEditViewModel
            {
                AuthorId = a.AuthorId,
                FullName = a.FullName
            }).ToList();

            ViewBag.Genres = _context.Genres.ToList();

            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> BookEdit(AdminEditBookViewModel model, int[] genreIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Books.Include("Genres").FirstOrDefault(g => g.BookId == model.BookId);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.Title = model.Title;
                entity.Description = model.Description;
                entity.Publisher = model.Publisher;
                entity.PageCount = model.PageCount;
                entity.IsActive = model.IsActive;

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);  // .jpg , .png  uzantılarını aldık
                    var fileName = Path.GetFileName(file.FileName); // Dosya adı uzantısı ile birlikte
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\book", fileName);
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

                // türlerin güncellemesini sağlıyoruz
                entity.Genres = genreIds.Select(id => _context.Genres.FirstOrDefault(i => i.GenreId == id)).ToList();

                _context.SaveChanges();

                return RedirectToAction("BookList");
            }
            model.Authors = _context.Authors.Select(a => new AuthorEditViewModel
            {
                AuthorId = a.AuthorId,
                FullName = a.FullName
            }).ToList();

            ViewBag.Genres = _context.Genres.ToList();
            return View(model);
        }

        // GENRE

        private List<AdminGenreViewModel> GetGenres()
        {
            return _context.Genres.Select(g => new AdminGenreViewModel
            {
                GenreId = g.GenreId,
                Name = g.Name,
                Count = g.Books.Count()
            }).ToList();
        }

        public IActionResult GenreList()
        {
            return View(GetGenres());
        }

        [HttpPost]
        public IActionResult GenreCreate(AdminGenreViewModel model)
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

        [HttpGet]
        public IActionResult GenreUpdate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _context
                .Genres
                .Where(g => g.GenreId == id)
                .Select(g => new AdminGenreGetViewModel
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    Books = g.Books.Select(b => new AdminWithoutAuthorBookViewModel
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Publisher = b.Publisher,
                        ImageUrl = b.ImageUrl,
                        PageCount = b.PageCount
                    }).ToList()
                })
                .FirstOrDefault();

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost]
        public IActionResult GenreUpdate(AdminGenreEditViewModel model, int[] bookIds)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Genres.Include(g => g.Books).FirstOrDefault(g => g.GenreId == model.GenreId);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;

                _context.SaveChanges();

                foreach (var bookId in bookIds)
                {
                    var book = entity.Books.FirstOrDefault(b => b.BookId == bookId);
                    if (book != null)
                    {
                        entity.Books.Remove(book);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction("GenreList");
            }
            return View(model);
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

        // USER - BOOK => RELATİONSHİP

        public IActionResult Relationship()
        {
            var userBooksFromDb = _context.UserBooks
                .Include(ub => ub.Book)
                .Include(ub => ub.User)
                .ToList();

            var userBookViewModels = userBooksFromDb.Select(ub => new UserBookViewModel
            {
                BookId = ub.BookId,
                BookTitle = ub.Book.Title,
                BookPublisher = ub.Book.Publisher,
                BookDescription = ub.Book.Description,
                BookImageUrl = ub.Book.ImageUrl,
                UserId = ub.UserId,
                UserFullName = ub.User.FullName,
                UserUserName = ub.User.UserName,
                UserEmail = ub.User.Email
            }).ToList();

            return View(userBookViewModels);
        }


    }
}
