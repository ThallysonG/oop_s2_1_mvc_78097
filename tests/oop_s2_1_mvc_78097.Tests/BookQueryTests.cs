using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Models;

namespace oop_s2_1_mvc_78097.Tests
{
    public class BookQueryTests
    {
        [Fact]
        public async Task BookSearch_ByTitleOrAuthor_ReturnsExpectedMatches()
        {
            using var context = TestDbContextFactory.Create();

            context.Books.AddRange(
                new Book
                {
                    Title = "C# in Depth",
                    Author = "Jon Skeet",
                    Isbn = "123",
                    Category = "Technology",
                    IsAvailable = true
                },
                new Book
                {
                    Title = "History of Europe",
                    Author = "Jane Doe",
                    Isbn = "456",
                    Category = "History",
                    IsAvailable = true
                },
                new Book
                {
                    Title = "ASP.NET Core Guide",
                    Author = "Jon Smith",
                    Isbn = "789",
                    Category = "Technology",
                    IsAvailable = false
                });

            await context.SaveChangesAsync();

            string searchTerm = "Jon";

            var results = await context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm))
                .ToListAsync();

            results.Should().HaveCount(2);
            results.Should().Contain(b => b.Title == "C# in Depth");
            results.Should().Contain(b => b.Title == "ASP.NET Core Guide");
        }

        [Fact]
        public async Task AvailabilityFilter_ReturnsOnlyAvailableBooks()
        {
            using var context = TestDbContextFactory.Create();

            context.Books.AddRange(
                new Book
                {
                    Title = "Book A",
                    Author = "Author A",
                    Isbn = "111",
                    Category = "Fiction",
                    IsAvailable = true
                },
                new Book
                {
                    Title = "Book B",
                    Author = "Author B",
                    Isbn = "222",
                    Category = "Science",
                    IsAvailable = false
                },
                new Book
                {
                    Title = "Book C",
                    Author = "Author C",
                    Isbn = "333",
                    Category = "History",
                    IsAvailable = true
                });

            await context.SaveChangesAsync();

            var results = await context.Books
                .Where(b => b.IsAvailable)
                .ToListAsync();

            results.Should().HaveCount(2);
            results.Should().OnlyContain(b => b.IsAvailable);
        }
    }
}