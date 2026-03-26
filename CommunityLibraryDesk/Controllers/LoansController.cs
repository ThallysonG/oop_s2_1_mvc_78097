using CommunityLibraryDesk.Data;
using CommunityLibraryDesk.Models.ViewModels;
using CommunityLibraryDesk.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CommunityLibraryDesk.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LoanService _loanService;

        public LoansController(ApplicationDbContext context, LoanService loanService)
        {
            _context = context;
            _loanService = loanService;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();

            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CreateLoanViewModel
            {
                AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .OrderBy(b => b.Title)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = $"{b.Title} - {b.Author}"
                    }).ToListAsync(),

                Members = await _context.Members
                    .OrderBy(m => m.FullName)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    }).ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLoanViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Title
                    }).ToListAsync();

                vm.Members = await _context.Members
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.FullName
                    }).ToListAsync();

                return View(vm);
            }

            var result = await _loanService.CreateLoanAsync(vm.BookId, vm.MemberId, vm.LoanDate, vm.DueDate);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return await Create();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int id)
        {
            await _loanService.MarkReturnedAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}