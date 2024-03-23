namespace BookWeb.Entity
{
    public class Book
    {
        public int BookId { get; set; }

        public string Title { get; set; }  // kitap başlığı

        public string Description { get; set; }  // details sayfası için gerekli property

        public string Publisher { get; set; }  // yayınevi

        public int PageCount { get; set; }  // sayfa sayısı

        public List<Genre> Genres { get; set; }

        // kitap için bir authorID gerekli, her kitap bir author olmalıdır
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    // bir kitap için birden fazla tür olabilir
}
