using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
	public class UsersEditViewModel
	{
		public string? Id { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 6, ErrorMessage = "İsim için 6-50 karakter aralığında girmelisin!")]
		[RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "Geçerli bir isim giriniz.")]
		public string? FullName { get; set; }

		[StringLength(15, MinimumLength = 4, ErrorMessage = "Kullanıcı ismi için 4-15 karakter aralığında girmelisin!")]
		[RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Kullanıcı adı yalnızca harflerden oluşabilir.")]
		public string? UserName { get; set; }

		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		public string? ImageUrl { get; set; }

		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Parola eşleşmiyor.")]
		public string? ConfirmPassword { get; set; }

		public IList<string>? SelectedRoles { get; set; }

	}
}
