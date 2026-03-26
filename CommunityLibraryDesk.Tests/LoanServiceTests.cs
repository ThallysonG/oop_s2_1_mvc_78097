using CommunityLibraryDesk.Data;
using CommunityLibraryDesk.Models;
using CommunityLibraryDesk.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityLibraryDesk.Tests
{
    public class LoanServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Cannot_Create_Loan_For_Book_With_Active_Loan()
        {
            var context = CreateContext();

            var book = new Book { Title = "Book A", Author = "Author A", Isbn = "111", Category = "Tech", IsAvailable = false };
            var member = new Member { FullName = "John Doe", Email = "john@test.com", Phone = "123" };

            context.Books.Add(book);
            context.Members.Add(member);
            await context.SaveChangesAsync();

            context.Loans.Add(new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                ReturnedDate = null
            });

            await context.SaveChangesAsync();

            var service = new LoanService(context);

            var result = await service.CreateLoanAsync(book.Id, member.Id, DateTime.Today, DateTime.Today.AddDays(7));

            Assert.False(result.Success);
        }

        [Fact]
        public async Task Returned_Loan_Makes_Book_Available_Again()
        {
            var context = CreateContext();

            var book = new Book { Title = "Book B", Author = "Author B", Isbn = "222", Category = "Science", IsAvailable = false };
            var member = new Member { FullName = "Mary Doe", Email = "mary@test.com", Phone = "456" };
            context.Books.Add(book);
            context.Members.Add(member);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7)
            };

            context.Loans.Add(loan);
            await context.SaveChangesAsync();

            var service = new LoanService(context);
            var result = await service.MarkReturnedAsync(loan.Id);

            Assert.True(result.Success);
            Assert.True(book.IsAvailable);
        }

        [Fact]
        public void Overdue_Loan_Is_Detected_Correctly()
        {
            var loan = new Loan
            {
                DueDate = DateTime.Today.AddDays(-1),
                ReturnedDate = null
            };

            Assert.True(loan.IsOverdue);
        }
    }
}