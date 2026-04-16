using Microsoft.AspNetCore.Mvc.Rendering;

namespace oop_s2_1_mvc_78097.ViewModels
{
    public class LoanCreateViewModel
    {
        public int BookId { get; set; }

        public int MemberId { get; set; }

        public DateTime LoanDate { get; set; } = DateTime.Today;

        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        public List<SelectListItem> AvailableBooks { get; set; } = new();

        public List<SelectListItem> Members { get; set; } = new();
    }
}