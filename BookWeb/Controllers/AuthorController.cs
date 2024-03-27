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
                    ImageAuthor = "default.jpg"
                };

                _context.Authors.Add(author);
                _context.SaveChanges();

                return RedirectToAction("AuthorList");
            }

            return View(model);
        }

        public IActionResult AuthorList()
        {
            // Yazarları AuthorViewModel'e dönüştür
            var authors = _context.Authors.Select(m => new AuthorViewModel
            {
                Authors = new List<Author> // Her bir yazar için yeni bir Author örneği oluştur
				{
                    new Author
                    {
                        AuthorId = m.AuthorId,
                        FullName = m.FullName,
                        Biography = m.Biography,
                        ImageAuthor = m.ImageAuthor
                    }
                }
            }).ToList();

            // AuthorViewModel listesini görünüme gönder
            return View(authors);
        }


        [HttpGet]
        public IActionResult AuthorEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _context.Authors.Select(p => new AuthorEditViewModel
            {
                AuthorId = p.AuthorId,
                FullName = p.FullName,
                Biography = p.Biography,
                ImageAuthor = p.ImageAuthor
            }).FirstOrDefault(p => p.AuthorId == id);

            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorEdit(AuthorEditViewModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Authors.FirstOrDefault(g => g.AuthorId == model.AuthorId);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.FullName = model.FullName;
                entity.Biography = model.Biography;

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);  // .jpg , .png  uzantılarını aldık
                    var fileName = Path.GetFileName(file.FileName); // Dosya adı uzantısı ile birlikte
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\author", fileName);
                    entity.ImageAuthor = fileName;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else // yeni dosya yüklenmemişse, mevcut resmi tanımlarız
                {
                    entity.ImageAuthor = model.ImageAuthor;
                }

                _context.SaveChanges();

                return RedirectToAction("AuthorList");
            }
            return View(model);
        }


        // Details
        public IActionResult AuthorDetails(int id)
        {
            var author = _context.Authors.Find(id);
            return View(author);
        }

        [HttpPost]
        public IActionResult AuthorDelete(int authorId)
        {
            var entity = _context.Authors.Find(authorId);

            if (entity != null)
            {
                _context.Authors.Remove(entity);
                _context.SaveChanges();
            }
            return RedirectToAction("AuthorList");
        }
    }
}
