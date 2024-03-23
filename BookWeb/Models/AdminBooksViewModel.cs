using System.ComponentModel.DataAnnotations;
using BookWeb.Entity;

namespace BookWeb.Models
{
    public class AdminBooksViewModel
    {
        public List<AdminBookViewModel> Books { get; set; }
    }

    // ViewModel'den her bilginin gelmesini istemiyoruz bu yüzden;
    // List<Movie> değil de List<AdminMovieViewModel> veri tipi ile Movies parametresine ulaşıyoruz!
    // böylelikle Description tipini çağırmayı engelledik!

    // description olmadan bir model yazıyoruz
    public class AdminBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PageCount { get; set; }
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

        [Display(Name = "Yazar")]
        [Required(ErrorMessage = "Yazar ismi girmelisiniz!")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Yazar ismi 3-30 aralığında olmalıdır.")]
        public string Author { get; set; }

        [Display(Name = "Yayınevi")]
        [Required(ErrorMessage = "Yayınevi ismi girmelisiniz!")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Yayınevi ismi 3-30 aralığında olmalıdır.")]
        public string Publisher { get; set; }

        [Display(Name = "Sayfa Sayısı")]
        [Required(ErrorMessage = "Kitap sayfa sayısı girmelisiniz!")]
        [Length(50,3000, ErrorMessage = "Kitap sayfa sayısı 50-3000 aralığında olmalıdır.")]
        public int PageCount { get; set; }

        [Required(ErrorMessage = "En az bir tür seçmelisiniz!")]
        public int[] GenreIds { get; set; }
    }

    public class AdminEditBookViewModel
    {
        [Display(Name = "Başlık")]
        [Required(ErrorMessage = "Kitap başlığı girmelisiniz!")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Film başlığı 3-30 aralığında olmalıdır.")]
        public string Title { get; set; }

        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Kitap açıklaması girmelisiniz!")]
        [StringLength(5000, MinimumLength = 100, ErrorMessage = "Film açıklaması 100-5000 aralığında olmalıdır.")]
        public string Description { get; set; }

        [Display(Name = "Yazar")]
        [Required(ErrorMessage = "Yazar ismi girmelisiniz!")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Yazar ismi 3-30 aralığında olmalıdır.")]
        public string Author { get; set; }

        [Display(Name = "Yayınevi")]
        [Required(ErrorMessage = "Yayınevi ismi girmelisiniz!")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Yayınevi ismi 3-30 aralığında olmalıdır.")]
        public string Publisher { get; set; }

        [Display(Name = "Sayfa Sayısı")]
        [Required(ErrorMessage = "Kitap sayfa sayısı girmelisiniz!")]
        [Length(50, 3000, ErrorMessage = "Kitap sayfa sayısı 50-3000 aralığında olmalıdır.")]
        public int PageCount { get; set; }

        [Required(ErrorMessage = "En az bir tür seçmelisiniz!")]
        public int[] GenreIds { get; set; }
    }


}
