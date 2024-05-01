using System.ComponentModel.DataAnnotations;
using BookWeb.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWeb.Models
{
	public class AdminBooksViewModel
	{
		public List<AdminBookViewModel> Books { get; set; }
	}

	// description olmadan bir model yazıyoruz
	public class AdminBookViewModel
	{
		public int BookId { get; set; }
		public string Title { get; set; }
		public string Publisher { get; set; }
		public string PageCount { get; set; }
		public string ImageUrl { get; set; }
		public bool IsActive { get; set; }
		public List<Genre> Genres { get; set; }
		public AuthorEditViewModel Author { get; set; }
	}

	public class AdminWithoutAuthorBookViewModel
	{
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string PageCount { get; set; }
        public string ImageUrl { get; set; }
        public List<Genre> Genres { get; set; }
    }

	public class AdminCreateBookViewModel
	{
		[Display(Name = "Başlık")]
		[Required(ErrorMessage = "Kitap başlığı girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Film başlığı 3-30 aralığında olmalıdır.")]
		public string Title { get; set; }

		[Display(Name = "Açıklama")]
		[Required(ErrorMessage = "Kitap açıklaması girmelisiniz!")]
		[StringLength(5000, MinimumLength = 100, ErrorMessage = "Film açıklaması 100-5000 aralığında olmalıdır.")]
		public string Description { get; set; }

		[Display(Name = "Author")]
		[Required(ErrorMessage = "Please select the book's author.")]
		public int AuthorId { get; set; } // Author's ID

		[Display(Name = "Yayınevi")]
		[Required(ErrorMessage = "Yayınevi ismi girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Yayınevi ismi 3-30 aralığında olmalıdır.")]
		public string Publisher { get; set; }

		[Display(Name = "Sayfa Sayısı")]
		[Required(ErrorMessage = "Kitap sayfa sayısı girmelisiniz!")]
		[StringLength(4, MinimumLength = 1, ErrorMessage = "Kitap sayfa sayısı 1-4 rakam arasında olmalıdır.")]
		public string PageCount { get; set; }

		[Required(ErrorMessage = "En az bir tür seçmelisiniz!")]
		public int[] GenreIds { get; set; }

		public List<AuthorEditViewModel> Authors { get; set; }

		[Display(Name = "Aktif mi?")]
		public bool IsActive { get; set; }

		public AdminCreateBookViewModel()
		{
			// Set IsActive to default value
			IsActive = true;
		}

	}

	public class AdminEditBookViewModel
	{
		public int BookId { get; set; }

		[Display(Name = "Başlık")]
		[Required(ErrorMessage = "Kitap başlığı girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Film başlığı 3-30 aralığında olmalıdır.")]
		public string Title { get; set; }

		[Display(Name = "Açıklama")]
		[Required(ErrorMessage = "Kitap açıklaması girmelisiniz!")]
		[StringLength(5000, MinimumLength = 100, ErrorMessage = "Film açıklaması 100-5000 aralığında olmalıdır.")]
		public string Description { get; set; }

		[Display(Name = "Yazar")]
		[Required(ErrorMessage = "Yazar seçmelisiniz!")]
		public int AuthorId { get; set; } // Yazarın ID'si

		[Display(Name = "Yayınevi")]
		[Required(ErrorMessage = "Yayınevi ismi girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Yayınevi ismi 3-30 aralığında olmalıdır.")]
		public string Publisher { get; set; }

		[Display(Name = "Sayfa Sayısı")]
		[Required(ErrorMessage = "Kitap sayfa sayısı girmelisiniz!")]
		[StringLength(4, MinimumLength = 1, ErrorMessage = "Kitap sayfa sayısı 1-4 rakam arasında olmalıdır.")]
		public string PageCount { get; set; }

		public string ImageUrl { get; set; }

		[Required(ErrorMessage = "En az bir tür seçmelisiniz!")]
		public int[] GenreIds { get; set; }

		public List<AuthorEditViewModel> Authors { get; set; }

		[Display(Name = "Aktif mi?")]
		public bool IsActive { get; set; }

	}

}
