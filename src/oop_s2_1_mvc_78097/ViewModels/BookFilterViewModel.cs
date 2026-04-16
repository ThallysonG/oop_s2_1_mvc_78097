using oop_s2_1_mvc_78097.Models;

namespace oop_s2_1_mvc_78097.ViewModels
{
    public class BookFilterViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Availability { get; set; } = "All";

        public List<string> Categories { get; set; } = new();

        public List<Book> Books { get; set; } = new();
    }
}