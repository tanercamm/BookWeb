namespace BookWeb.Entity
{
    public class Author
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; }

        public string Biography { get; set; }

        public string ImageAuthor { get; set; }

		//public List<Book> Books { get; set; } // An author can have multiple books
	}
}
