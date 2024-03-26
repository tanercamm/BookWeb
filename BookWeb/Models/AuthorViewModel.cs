using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
	public class AuthorViewModel
	{
		public int AuthorId { get; set; }

		public string FullName { get; set; }

		public string Biography { get; set; }

		public string ImageAuthor { get; set; }

	}


	public class AuthorCreateViewModel
	{
		[Display(Name = "Yazar")]
		[Required(ErrorMessage = "Yazar ismi girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Yazar ismi 3-30 aralığında olmalıdır.")]
		public string FullName { get; set; }

		[Display(Name = "Biyografi")]
		[Required(ErrorMessage = "Yazar biyografisi girmelisiniz!")]
		[StringLength(5000, MinimumLength = 100, ErrorMessage = "Biyografi alanı 100-5000 aralığında olmalıdır.")]
		public string Biography { get; set; }

	}

	public class AuthorEditViewModel
	{
		public int AuthorId { get; set; }

		[Display(Name = "Yazar")]
		[Required(ErrorMessage = "Yazar ismi girmelisiniz!")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Yazar ismi 3-30 aralığında olmalıdır.")]
		public string FullName { get; set; }

		[Display(Name = "Biyografi")]
		[Required(ErrorMessage = "Yazar biyografisi girmelisiniz!")]
		[StringLength(5000, MinimumLength = 100, ErrorMessage = "Biyografi alanı 100-5000 aralığında olmalıdır.")]
		public string Biography { get; set; }

		public string ImageAuthor { get; set; }

	}

}
