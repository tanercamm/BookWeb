namespace BookWeb.Entity
{
    public class Author
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; }

        // her author için kitap(lar) olmalıdır, kitabı olmayan author sistemde olmamalı
        public ICollection<Book> Books { get; set; }

    }
}
