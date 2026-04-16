using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_78097.Data;
using oop_s2_1_mvc_78097.Models;
using oop_s2_1_mvc_78097.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace oop_s2_1_mvc_78097.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeDashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                AvailableBooks = await _context.Books.CountAsync(b => b.IsAvailable),
                OnLoanBooks = await _context.Books.CountAsync(b => !b.IsAvailable),
                TotalMembers = await _context.Members.CountAsync(),
                ActiveLoans = await _context.Loans.CountAsync(l => l.ReturnedDate == null),
                OverdueLoans = await _context.Loans.CountAsync(l => l.ReturnedDate == null && l.DueDate < DateTime.Today)
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}