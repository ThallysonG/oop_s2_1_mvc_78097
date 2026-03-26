using CommunityLibraryDesk.Controllers;
using CommunityLibraryDesk.Data;
using CommunityLibraryDesk.Models;
using CommunityLibraryDesk.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityLibraryDesk.Tests
{
    public class BooksControllerTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Book_Search_Returns_Expected_Matches()
        {
            var context = CreateContext();

            context.Books.AddRange(
                new Book { Title = "C# Basics", Author = "Alice", Isbn = "1", Category = "Tech", IsAvailable = true },
                new Book { Title = "History of Rome", Author = "Bob", Isbn = "2", Category = "History", IsAvailable = true }
            );

            await context.SaveChangesAsync();

            var controller = new BooksController(context);

            var result = await controller.Index("C#", null, null) as Microsoft.AspNetCore.Mvc.ViewResult;
            var model = Assert.IsType<BookFilterViewModel>(result!.Model);

            Assert.Single(model.Books);
            Assert.Equal("C# Basics", model.Books.First().Title);
        }

        [Fact]
        public async Task Book_Filter_By_Category_Returns_Only_Category_Items()
        {
            var context = CreateContext();

            context.Books.AddRange(
                new Book { Title = "Book 1", Author = "Author 1", Isbn = "10", Category = "Tech", IsAvailable = true },
                new Book { Title = "Book 2", Author = "Author 2", Isbn = "20", Category = "History", IsAvailable = true }
            );

            await context.SaveChangesAsync();

            var controller = new BooksController(context);

            var result = await controller.Index(null, "Tech", null) as Microsoft.AspNetCore.Mvc.ViewResult;
            var model = Assert.IsType<BookFilterViewModel>(result!.Model);

            Assert.Single(model.Books);
            Assert.Equal("Tech", model.Books.First().Category);
        }
    }
}