using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookWeb.Models
{
	public class ProfileUpdateViewModel
	{
		public string Id { get; set; }

		[ReadOnly(true)]
		public string FullName { get; set; }

		[ReadOnly(true)]
		public string UserName { get; set; }

		[ReadOnly(true)]
		public string Email { get; set; }

		public string ImageUrl { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Please enter your current password for security reasons.")]
		public string CurrentPassword { get; set; }

		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Password does not match.")]
		public string ConfirmPassword { get; set; }

		public bool isChangePassword { get; set; }

	}
}
