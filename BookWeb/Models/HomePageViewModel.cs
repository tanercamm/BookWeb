using BookWeb.Entity;

namespace BookWeb.Models
{
	public class HomePageViewModel
	{
		public List<Book> PopularBooks { get; set; }

		public PageInfo PageInfo { get; set; } = new();
	}

	public class PageInfo
	{
		public int TotalItems { get; set; }

		public int ItemsPerPage { get; set; }

		// 6.8 gibi bir değer çıkarsa 6 sayfa değil 7 sayfa yapılacak, Math.Ceiling ile!
		public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

	}
}
