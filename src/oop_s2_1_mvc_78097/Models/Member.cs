namespace oop_s2_1_mvc_78097.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}