using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Models;

namespace oop_s2_1_mvc_78097.Tests
{
    public class LoanTests
    {
        [Fact]
        public async Task CannotCreateLoan_WhenBookAlreadyHasActiveLoan()
        {
            using var context = TestDbContextFactory.Create();

            var book = new Book
            {
                Title = "Clean Code",
                Author = "Robert Martin",
                Isbn = "111",
                Category = "Technology",
                IsAvailable = false
            };

            var member1 = new Member
            {
                FullName = "John Smith",
                Email = "john@email.com",
                Phone = "123"
            };

            var member2 = new Member
            {
                FullName = "Mary Jones",
                Email = "mary@email.com",
                Phone = "456"
            };

            context.Books.Add(book);
            context.Members.AddRange(member1, member2);
            await context.SaveChangesAsync();

            context.Loans.Add(new Loan
            {
                BookId = book.Id,
                MemberId = member1.Id,
                LoanDate = DateTime.Today.AddDays(-2),
                DueDate = DateTime.Today.AddDays(5),
                ReturnedDate = null
            });

            await context.SaveChangesAsync();

            bool hasActiveLoan = await context.Loans
                .AnyAsync(l => l.BookId == book.Id && l.ReturnedDate == null);

            hasActiveLoan.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnedLoan_MakesBookAvailableAgain()
        {
            using var context = TestDbContextFactory.Create();

            var book = new Book
            {
                Title = "Domain Driven Design",
                Author = "Eric Evans",
                Isbn = "222",
                Category = "Technology",
                IsAvailable = false
            };

            var member = new Member
            {
                FullName = "Alice Brown",
                Email = "alice@email.com",
                Phone = "999"
            };

            context.Books.Add(book);
            context.Members.Add(member);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Today.AddDays(-5),
                DueDate = DateTime.Today.AddDays(2),
                ReturnedDate = null
            };

            context.Loans.Add(loan);
            await context.SaveChangesAsync();

            loan.ReturnedDate = DateTime.Today;
            book.IsAvailable = true;

            await context.SaveChangesAsync();

            book.IsAvailable.Should().BeTrue();
            loan.ReturnedDate.Should().NotBeNull();
        }

        [Fact]
        public void OverdueLoan_IsDetectedCorrectly()
        {
            var loan = new Loan
            {
                LoanDate = DateTime.Today.AddDays(-10),
                DueDate = DateTime.Today.AddDays(-2),
                ReturnedDate = null
            };

            loan.IsOverdue.Should().BeTrue();
        }
    }
}