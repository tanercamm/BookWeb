namespace BookWeb.Models
{
	public class UserBookViewModel
	{
		// Kitap Bilgileri
		public int BookId { get; set; }
		public string BookTitle { get; set; }
		public string BookPublisher { get; set; }
		public string BookDescription { get; set; }
		public string BookImageUrl { get; set; }

		// Kullanıcı Bilgileri
		public string UserId { get; set; }
		public string UserFullName { get; set; }
		public string UserUserName { get; set; }
		public string UserEmail { get; set; }
	}
}
