using BookWeb.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Data
{
    public class BookContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<UserBook> UserBooks { get; set; }

        public DbSet<Comment> Comments { get; set; }

        // entity'lerin özellik tanımlamasını burada işleyebiliriz 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Movie ve Genre entity'lerinin özelliklerini belirtebiliriz
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(30);
            modelBuilder.Entity<Genre>().Property(b => b.Name).IsRequired().HasMaxLength(25);

            modelBuilder.Entity<UserBook>().HasKey(ub => new { ub.UserId, ub.BookId });

            modelBuilder.Entity<Comment>().HasOne(c => c.Book).WithMany(b => b.Comments).HasForeignKey(c => c.BookId);

            modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId);

        }
    }
}
