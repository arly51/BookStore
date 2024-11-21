using Microsoft.EntityFrameworkCore;
using BLL;

namespace BLL.DAL
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book - Author relationship (one-to-many)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            // Book - Genre relationship (many-to-many through BookGenre)
            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId);

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId);

            // User - Role relationship (one-to-many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()  // No navigation property back to Users as per diagram
                .HasForeignKey(u => u.RoleId);
        }
    }
}