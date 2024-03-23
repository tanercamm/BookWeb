namespace BookWeb.Entity
{
    public class Genre
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }

    // bir tür için birden fazla kitap olabilir
}
