using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
    public class AdminGenresViewModel
    {
        [Required(ErrorMessage = "Tür bilgisi girmelisiniz!")]
        public string Name { get; set; }

        public List<AdminGenreViewModel> Genres { get; set; }
    }

    // count parametresi ekliyoruz bu sebeple artık bunu kullanacağız
    public class AdminGenreViewModel
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }
    }

    // edit genre model
    public class AdminGenreEditViewModel
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Tür bilgisi girmelisiniz!")]
        public string Name { get; set; }

        public List<AdminGenreViewModel> Movies { get; set; }

    }

}
