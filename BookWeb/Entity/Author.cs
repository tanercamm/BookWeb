namespace BookWeb.Entity
{
    public class Author
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; }

        public ICollection<Book> Books { get; set; }

    }
}
