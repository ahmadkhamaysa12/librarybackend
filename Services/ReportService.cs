
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class ReportService
    {
        private Library library;
        private LoanService loanService;
        private FineService fineService;

        public ReportService(
            Library library,
            LoanService loanService,
            FineService fineService)
        {
            this.library = library;
            this.loanService = loanService;
            this.fineService = fineService;
        }

        // 1. Available, borrowed, and inactive items
        public void ShowItemsReport()
        {
            Console.WriteLine("\n=== AVAILABLE ITEMS ===");

            foreach (LibraryItem item in library.GetAllItems())
            {
                if (item.IsAvailable())
                {
                    Console.WriteLine(
                        $"{item.Id} - {item.Title}"
                    );
                }
            }

            Console.WriteLine("\n=== BORROWED ITEMS ===");

            foreach (LibraryItem item in library.GetAllItems())
            {
                if (item.IsBorrowed)
                {
                    Console.WriteLine(
                        $"{item.Id} - {item.Title}"
                    );
                }
            }

            Console.WriteLine("\n=== INACTIVE ITEMS ===");

            foreach (LibraryItem item in library.GetAllItems())
            {
                if (!item.IsActive)
                {
                    Console.WriteLine(
                        $"{item.Id} - {item.Title}"
                    );
                }
            }
        }

        // Helper: Count borrowing history for an item
        private int CountLoansForItem(int itemId)
        {
            int count = 0;

            foreach (Loan loan in loanService.GetAllLoans())
            {
                if (loan.Item.Id == itemId)
                    count++;
            }

            return count;
        }

        // 2. Most borrowed items
        public void ShowMostBorrowedItems()
        {
            Console.WriteLine("\n=== MOST BORROWED ITEMS ===");

            int maxCount = 0;

            foreach (LibraryItem item in library.GetAllItems())
            {
                int count = CountLoansForItem(item.Id);

                if (count > maxCount)
                    maxCount = count;
            }

            if (maxCount == 0)
            {
                Console.WriteLine("No borrowing history.");
                return;
            }

            foreach (LibraryItem item in library.GetAllItems())
            {
                int count = CountLoansForItem(item.Id);

                if (count == maxCount)
                {
                    Console.WriteLine(
                        $"{item.Title} - {count} loans"
                    );
                }
            }
        }

        // 3. Members with overdue loans
        public void ShowOverdueMembers(DateTime date)
        {
            Console.WriteLine("\n=== OVERDUE MEMBERS ===");

            List<Loan> overdue =
                loanService.GetOverdueLoans(date);

            if (overdue.Count == 0)
            {
                Console.WriteLine("No overdue loans.");
                return;
            }

            List<int> displayedMembers = new List<int>();

            foreach (Loan loan in overdue)
            {
                int memberId = loan.Member.Id;

                if (displayedMembers.Contains(memberId))
                    continue;

                displayedMembers.Add(memberId);

                Console.WriteLine(
                    $"\nMember: {loan.Member.Name} ({memberId})"
                );

                foreach (Loan memberLoan in overdue)
                {
                    if (memberLoan.Member.Id == memberId)
                    {
                        Console.WriteLine(
                            $"Item: {memberLoan.Item.Title}"
                        );

                        Console.WriteLine(
                            $"Due: {memberLoan.DueDate:yyyy-MM-dd}"
                        );

                        Console.WriteLine(
                            $"Late Days: {memberLoan.GetLateDays(date)}"
                        );
                    }
                }
            }
        }

        // 4. Members with unpaid fines
        public void ShowMembersWithUnpaidFines()
        {
            Console.WriteLine("\n=== MEMBERS WITH UNPAID FINES ===");

            bool found = false;

            foreach (Member member in library.GetAllMembers())
            {
                decimal unpaid =
                    fineService.GetMemberUnpaidTotal(member.Id);

                if (unpaid > 0)
                {
                    found = true;

                    Console.WriteLine(
                        $"{member.Id} - {member.Name}: {unpaid} NIS"
                    );
                }
            }

            if (!found)
                Console.WriteLine("No unpaid fines.");
        }

        // 5. Total collected fines
        public void ShowTotalCollected()
        {
            Console.WriteLine("\n=== TOTAL COLLECTED FINES ===");

            Console.WriteLine(
                $"Total: {fineService.GetTotalCollected()} NIS"
            );
        }

        // 6. Member borrowing history
        public void ShowMemberHistory(int memberId)
        {
            Console.WriteLine("\n=== MEMBER LOAN HISTORY ===");

            Member? member = library.SearchMember(memberId);

            if (member == null)
            {
                Console.WriteLine("Member not found.");
                return;
            }

            Console.WriteLine($"Member: {member.Name}");

            List<Loan> history =
                loanService.GetMemberLoans(memberId);

            if (history.Count == 0)
            {
                Console.WriteLine("No borrowing history.");
                return;
            }

            foreach (Loan loan in history)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine($"Loan ID: {loan.Id}");
                Console.WriteLine($"Item: {loan.Item.Title}");
                Console.WriteLine(
                    $"Borrowed: {loan.BorrowDate:yyyy-MM-dd}"
                );
                Console.WriteLine(
                    $"Due: {loan.DueDate:yyyy-MM-dd}"
                );

                if (loan.IsReturned)
                {
                    Console.WriteLine(
                        $"Returned: {loan.ReturnDate:yyyy-MM-dd}"
                    );
                    Console.WriteLine("Status: Returned");
                }
                else
                {
                    Console.WriteLine("Status: Active");
                }
            }

            Console.WriteLine("----------------------");
        }
    }
}
