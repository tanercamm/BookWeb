using BookWeb.Entity;

namespace BookWeb.Models
{
	public class BooksViewModel
	{
		public List<Book> Books { get; set; }

		public PageInfo PageInfo { get; set; } = new();
	}
}
