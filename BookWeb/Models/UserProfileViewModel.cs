namespace BookWeb.Models
{
	public class UserProfileViewModel
	{
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string ImageUrl { get; set; }
		public List<UserBookViewModel> UserBooks { get; set; }
	}
}
