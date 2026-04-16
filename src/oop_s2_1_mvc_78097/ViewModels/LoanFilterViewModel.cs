using oop_s2_1_mvc_78097.Models;

namespace oop_s2_1_mvc_78097.ViewModels
{
    public class LoanFilterViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;

        public string Status { get; set; } = "All";

        public List<Loan> Loans { get; set; } = new();
    }
}