using CommunityLibraryDesk.Data;
using CommunityLibraryDesk.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityLibraryDesk.Services
{
    public class LoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateLoanAsync(int bookId, int memberId, DateTime loanDate, DateTime dueDate)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
                return (false, "Livro não encontrado.");

            var hasActiveLoan = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.ReturnedDate == null);

            if (hasActiveLoan)
                return (false, "Este livro já está em um empréstimo ativo.");

            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                LoanDate = loanDate,
                DueDate = dueDate,
                ReturnedDate = null
            };

            book.IsAvailable = false;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> MarkReturnedAsync(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
                return (false, "Empréstimo não encontrado.");

            if (loan.ReturnedDate != null)
                return (false, "Este empréstimo já foi devolvido.");

            loan.ReturnedDate = DateTime.Now;

            if (loan.Book != null)
                loan.Book.IsAvailable = true;

            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }
    }
}