using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
    // count parametresi ekliyoruz bu sebeple artık bunu kullanacağız
    public class AdminGenreViewModel
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }
    }

    public class AdminGenreGetViewModel
    {
        public AdminGenreGetViewModel()
        {
            Books = new List<AdminWithoutAuthorBookViewModel>();
        }

        public int GenreId { get; set; }

        [Required(ErrorMessage = "Tür bilgisi girmelisiniz!")]
        [StringLength(25)]
        public string Name { get; set; }

        public List<AdminWithoutAuthorBookViewModel> Books { get; set; }
    }

    // count parametresi ekliyoruz bu sebeple artık bunu kullanacağız
    public class AdminGenreEditViewModel
    {
        public int GenreId { get; set; }

        public string Name { get; set; }
    }

}
