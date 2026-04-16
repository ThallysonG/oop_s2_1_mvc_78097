namespace oop_s2_1_mvc_78097.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }

        public Member? Member { get; set; }
        public Book? Book { get; set; }
        public bool IsOverdue => ReturnedDate == null && DueDate.Date < DateTime.Today;
    }
}