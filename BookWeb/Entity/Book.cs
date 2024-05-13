namespace BookWeb.Entity
{
	public class Book
	{
		public int BookId { get; set; }

		public string Title { get; set; }  // kitap başlığı

		public string Description { get; set; }  // details sayfası için gerekli property

		public string Publisher { get; set; }  // yayınevi

		public string PageCount { get; set; }  // sayfa sayısı

		public string ImageUrl { get; set; }

		// kitabın sahip edilebilirliğine bakılabilmesi gerekiyor
		public bool IsEmpty { get; set; }

		public List<Genre> Genres { get; set; }

		public int AuthorId { get; set; } // Her kitabın yalnızca bir yazarı olacağı için
		public Author Author { get; set; }

		public List<UserBook> Users { get; set; }

		public List<Comment> Comments { get; set; } = new List<Comment>();

	}

	// bir kitap için birden fazla tür olabilir
}
