using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    PublishedDate = new DateTime(1925, 4, 10),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "978-0061120084",
                    PublishedDate = new DateTime(1960, 7, 11),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "978-0451524935",
                    PublishedDate = new DateTime(1949, 6, 8),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 4,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "978-1503290563",
                    PublishedDate = new DateTime(1813, 1, 28),
                    IsAvailable = true
                }

                );
        }
    }
}
