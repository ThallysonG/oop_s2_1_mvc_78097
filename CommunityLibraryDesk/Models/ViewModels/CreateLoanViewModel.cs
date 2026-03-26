using CommunityLibraryDesk.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CommunityLibraryDesk.Models.ViewModels
{
    public class CreateLoanViewModel
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public DateTime LoanDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        public List<SelectListItem> AvailableBooks { get; set; } = new();
        public List<SelectListItem> Members { get; set; } = new();
    }
}