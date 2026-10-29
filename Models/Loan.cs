
namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int Id { get; private set; }

        public Member Member { get; private set; }

        public LibraryItem Item { get; private set; }

        public DateTime BorrowDate { get; private set; }

        public DateTime DueDate { get; private set; }

        public DateTime? ReturnDate { get; private set; }

        public bool IsReturned
        {
            get { return ReturnDate.HasValue; }
        }

        public Loan(int id, Member member,
                    LibraryItem item, DateTime borrowDate)
        {
            Id = id;
            Member = member;
            Item = item;

            BorrowDate = borrowDate.Date;

            DueDate = BorrowDate.AddDays(
                item.GetLoanPeriod()
            );

            ReturnDate = null;
        }

        public bool IsOverdue(DateTime date)
        {
            return !IsReturned && date.Date > DueDate;
        }



public int GetLateDays(DateTime date)
{
    DateTime endDate = ReturnDate ?? date.Date;

    int days = (endDate - DueDate).Days;

    return Math.Max(0, days);
}



        public decimal GetLateFee(DateTime date)
        {
            int lateDays = GetLateDays(date);

            return Item.CalculateLateFee(lateDays);
        }

        internal void CompleteReturn(DateTime date)
        {
            ReturnDate = date.Date;
        }
    }
}
