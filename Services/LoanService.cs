
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class LoanService
    {
        private Library library;
        private FineService fineService;
        private List<Loan> loans;
        private int nextLoanId;

        // Constructor
        public LoanService(Library library, FineService fineService)
        {
            this.library = library;
            this.fineService = fineService;

            loans = new List<Loan>();
            nextLoanId = 1;
        }

        // Check if member has overdue loans
        private bool HasOverdueLoans(int memberId)
        {
            foreach (Loan loan in loans)
            {
                if (loan.Member.Id == memberId &&
                    loan.IsOverdue(DateTime.Today))
                {
                    return true;
                }
            }

            return false;
        }

        // Borrow using today's date
        public Loan? BorrowItem(int memberId, int itemId)
        {
            return BorrowItem(
                memberId,
                itemId,
                DateTime.Today
            );
        }

        // Borrow using a specified date
        public Loan? BorrowItem(
            int memberId,
            int itemId,
            DateTime borrowDate)
        {
            Member? member = library.SearchMember(memberId);
            LibraryItem? item = library.SearchItem(itemId);

            // Check member and item existence
            if (member == null || item == null)
                return null;

            // Check membership status
            if (!member.IsActive)
                return null;

            // Prevent borrowing with unpaid fines
            if (fineService.HasUnpaidFines(memberId))
                return null;

            // Prevent borrowing with overdue loans
            if (HasOverdueLoans(memberId))
                return null;

            // Check item availability
            if (!item.IsAvailable())
                return null;

            // Prevent future borrowing dates
            if (borrowDate.Date > DateTime.Today)
                return null;

            // Count member's active loans
            int activeCount = 0;

            foreach (Loan loan in loans)
            {
                if (loan.Member.Id == memberId &&
                    !loan.IsReturned)
                {
                    activeCount++;
                }
            }

            // Check borrowing limit
            if (activeCount >= member.MaxLoans)
                return null;

            // Create new loan
            Loan newLoan = new Loan(
                nextLoanId,
                member,
                item,
                borrowDate
            );

            // Update item status
            if (!item.MarkBorrowed())
                return null;

            // Save loan history
            loans.Add(newLoan);
            nextLoanId++;

            return newLoan;
        }

        // Search loan by ID
        public Loan? SearchLoan(int loanId)
        {
            foreach (Loan loan in loans)
            {
                if (loan.Id == loanId)
                    return loan;
            }

            return null;
        }

        // Return using today's date
        public bool ReturnItem(int loanId)
        {
            return ReturnItem(
                loanId,
                DateTime.Today
            );
        }

        // Return using a specified date
        public bool ReturnItem(
            int loanId,
            DateTime returnDate)
        {
            Loan? loan = SearchLoan(loanId);

            // Check loan existence and status
            if (loan == null || loan.IsReturned)
                return false;

            // Prevent return before borrowing
            if (returnDate.Date < loan.BorrowDate)
                return false;

            // Prevent future return dates
            if (returnDate.Date > DateTime.Today)
                return false;

            // Update item status
            if (!loan.Item.MarkReturned())
                return false;

            // Complete loan
            loan.CompleteReturn(returnDate);

            // Register late fine automatically
            fineService.RegisterFine(loan);

            return true;
        }

        // Get all loans
        public List<Loan> GetAllLoans()
        {
            return new List<Loan>(loans);
        }

        // Get active loans
        public List<Loan> GetActiveLoans()
        {
            List<Loan> active = new List<Loan>();

            foreach (Loan loan in loans)
            {
                if (!loan.IsReturned)
                {
                    active.Add(loan);
                }
            }

            return active;
        }

        // Get all loans for a specific member
        public List<Loan> GetMemberLoans(int memberId)
        {
            List<Loan> history = new List<Loan>();

            foreach (Loan loan in loans)
            {
                if (loan.Member.Id == memberId)
                {
                    history.Add(loan);
                }
            }

            return history;
        }

        // Get overdue loans
        public List<Loan> GetOverdueLoans(DateTime date)
        {
            List<Loan> overdue = new List<Loan>();

            foreach (Loan loan in loans)
            {
                if (loan.IsOverdue(date))
                {
                    overdue.Add(loan);
                }
            }

            return overdue;
        }
    }
}
