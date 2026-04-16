using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Data;
using oop_s2_1_mvc_78097.Models;
using oop_s2_1_mvc_78097.ViewModels;

namespace oop_s2_1_mvc_78097.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();

            return View(loans);
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new LoanCreateViewModel
            {
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .OrderBy(b => b.Title)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = $"{b.Title} - {b.Author}"
                    })
                    .ToListAsync(),

                Members = await _context.Members
                    .OrderBy(m => m.FullName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Loans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanCreateViewModel viewModel)
        {
            bool hasActiveLoan = await _context.Loans
                .AnyAsync(l => l.BookId == viewModel.BookId && l.ReturnedDate == null);

            if (hasActiveLoan)
            {
                ModelState.AddModelError(string.Empty, "This book is already on an active loan.");
            }

            var book = await _context.Books.FindAsync(viewModel.BookId);
            if (book == null)
            {
                ModelState.AddModelError(nameof(viewModel.BookId), "Selected book was not found.");
            }

            var member = await _context.Members.FindAsync(viewModel.MemberId);
            if (member == null)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "Selected member was not found.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .OrderBy(b => b.Title)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = $"{b.Title} - {b.Author}"
                    })
                    .ToListAsync();

                viewModel.Members = await _context.Members
                    .OrderBy(m => m.FullName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    })
                    .ToListAsync();

                return View(viewModel);
            }

            var loan = new Loan
            {
                BookId = viewModel.BookId,
                MemberId = viewModel.MemberId,
                LoanDate = viewModel.LoanDate,
                DueDate = viewModel.DueDate,
                ReturnedDate = null
            };

            _context.Loans.Add(loan);

            book!.IsAvailable = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Loans/MarkReturned/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            if (loan.ReturnedDate == null)
            {
                loan.ReturnedDate = DateTime.Today;

                if (loan.Book != null)
                {
                    loan.Book.IsAvailable = true;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}