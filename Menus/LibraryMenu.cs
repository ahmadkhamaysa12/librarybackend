using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Menus
{
    public class LibraryMenu
    {
        private Library library;
        private AuthService authService;
        private LoanService loanService;
        private FineService fineService;
        private ReportService reportService;

        public LibraryMenu(
            Library library,
            AuthService authService,
            LoanService loanService,
            FineService fineService,
            ReportService reportService)
        {
            this.library = library;
            this.authService = authService;
            this.loanService = loanService;
            this.fineService = fineService;
            this.reportService = reportService;
        }

        private string ReadText(string message)
        {
            Console.Write(message);
            return Console.ReadLine()?.Trim() ?? "";
        }

        private int ReadInt(string message)
        {
            string value = ReadText(message);

            if (int.TryParse(value, out int result))
                return result;

            Console.WriteLine("Invalid number.");
            return -1;
        }

        private decimal ReadAmount(string message)
        {
            string value = ReadText(message);

            if (decimal.TryParse(value, out decimal result))
                return result;

            Console.WriteLine("Invalid amount.");
            return -1;
        }

        private string ReadPassword()
        {
            Console.Write("Password: ");
            string password = "";

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(
                            0, password.Length - 1
                        );

                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            Console.WriteLine();
            return password;
        }

        // Main menu
        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine(
                    "\n===== Library Management System ====="
                );
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register New Member");
                Console.WriteLine("3. Browse Library");
                Console.WriteLine("4. Search for Items");
                Console.WriteLine("5. Exit");

                int choice = ReadInt("Choose: ");

                switch (choice)
                {
                    case 1:
                        Login();
                        break;

                    case 2:
                        RegisterMember();
                        break;

                    case 3:
                        BrowseLibrary();
                        break;

                    case 4:
                        SearchItems();
                        break;

                    case 5:
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

        private void RegisterMember()
        {
            Console.WriteLine("\n=== Register Member ===");

            string name = ReadText("Name: ");
            string username = ReadText("Username: ");
            string password = ReadPassword();

            Member? member = authService.RegisterMember(
                name, username, password
            );

            if (member == null)
            {
                Console.WriteLine(
                    "Registration failed. Check the data, " +
                    "username, and password length (min 8)."
                );
                return;
            }

            Console.WriteLine("Registration successful!");
            Console.WriteLine($"Member ID: {member.Id}");
        }

        private void Login()
        {
            Console.WriteLine("\n=== Login ===");

            string username = ReadText("Username: ");
            string password = ReadPassword();

            Person? person = authService.Login(
                username, password
            );

            if (person == null)
            {
                Console.WriteLine("Invalid username or password.");
                return;
            }

            Console.WriteLine($"Welcome, {person.Name}!");

            if (person is Librarian librarian)
            {
                LibrarianMenu(librarian);
            }
            else if (person is Member member)
            {
                if (!member.IsActive)
                {
                    Console.WriteLine("Membership disabled.");
                    return;
                }

                MemberMenu(member);
            }
        }

        private void BrowseLibrary()
        {
            Console.WriteLine("\n=== Library Items ===");

            List<LibraryItem> items = library.GetAllItems();

            if (items.Count == 0)
            {
                Console.WriteLine("No items found.");
                return;
            }

            foreach (LibraryItem item in items)
            {
                Console.WriteLine(
                    $"{item.Id} - {item.Title} " +
                    $"| Available: {item.IsAvailable()} " +
                    $"| Active: {item.IsActive}"
                );
            }
        }

        private void SearchItems()
        {
            Console.WriteLine("\n=== Search Items ===");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Title");

            int choice = ReadInt("Choose: ");

            if (choice == 1)
            {
                int id = ReadInt("Item ID: ");
                LibraryItem? item = library.SearchItem(id);

                if (item == null)
                {
                    Console.WriteLine("Item not found.");
                    return;
                }

                item.DisplayInfo();
            }
            else if (choice == 2)
            {
                string title = ReadText("Title: ");

                List<LibraryItem> results =
                    library.SearchItem(title);

                if (results.Count == 0)
                    Console.WriteLine("No results.");

                foreach (LibraryItem item in results)
                {
                    item.DisplayInfo();
                    Console.WriteLine("--------------------");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        // Member menu
        private void MemberMenu(Member member)
        {
            while (true)
            {
                Console.WriteLine(
                    "\n===== Member Menu ====="
                );
                Console.WriteLine("1. Browse Library");
                Console.WriteLine("2. Search Items");
                Console.WriteLine("3. My Loan History");
                Console.WriteLine("4. My Fines");
                Console.WriteLine("5. My Profile");
                Console.WriteLine("0. Logout");

                int choice = ReadInt("Choose: ");

                switch (choice)
                {
                    case 1:
                        BrowseLibrary();
                        break;

                    case 2:
                        SearchItems();
                        break;

                    case 3:
                        reportService.ShowMemberHistory(
                            member.Id
                        );
                        break;

                    case 4:
                        ShowMemberFines(member.Id);
                        break;

                    case 5:
                        member.DisplayInfo();
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void ShowMemberFines(int memberId)
        {
            List<Fine> fines =
                fineService.GetMemberFines(memberId);

            if (fines.Count == 0)
            {
                Console.WriteLine("No fines.");
                return;
            }

            foreach (Fine fine in fines)
            {
                Console.WriteLine(
                    $"Fine #{fine.Id} " +
                    $"| Amount: {fine.Amount} " +
                    $"| Paid: {fine.GetPaidAmount()} " +
                    $"| Remaining: {fine.GetRemainingAmount()}"
                );
            }

            Console.WriteLine(
                $"Total Unpaid: " +
                $"{fineService.GetMemberUnpaidTotal(memberId)} NIS"
            );
        }

        // Librarian menu
        private void LibrarianMenu(Librarian librarian)
        {
            while (true)
            {
                Console.WriteLine(
                    "\n===== Librarian Menu ====="
                );
                Console.WriteLine($"Employee: {librarian.Name}");
                Console.WriteLine("1. Add Library Item");
                Console.WriteLine("2. Browse Library");
                Console.WriteLine("3. Search Items");
                Console.WriteLine("4. Register Member");
                Console.WriteLine("5. View Members");
                Console.WriteLine("6. Borrow Item");
                Console.WriteLine("7. Return Item");
                Console.WriteLine("8. Pay Fine");
                Console.WriteLine("9. Reports");
                Console.WriteLine("10. Disable Item");
                Console.WriteLine("11. Disable Member");
                Console.WriteLine("12. Update Item Title");
                Console.WriteLine("13. Change Borrow Limit");
                Console.WriteLine("14. View Item Details");
                Console.WriteLine("15. Update Item Details");
                Console.WriteLine("16. Search Member");
                Console.WriteLine("17. View Active Loans");
                Console.WriteLine("0. Logout");

                int choice = ReadInt("Choose: ");

                switch (choice)
                {
                    case 1:
                        AddItem();
                        break;
                    case 2:
                        BrowseLibrary();
                        break;
                    case 3:
                        SearchItems();
                        break;
                    case 4:
                        RegisterMember();
                        break;
                    case 5:
                        ViewMembers();
                        break;
                    case 6:
                        BorrowItem();
                        break;
                    case 7:
                        ReturnItem();
                        break;
                    case 8:
                        PayFine();
                        break;
                    case 9:
                        ReportsMenu();
                        break;
                    case 10:
                        DisableItem();
                        break;
                    case 11:
                        DisableMember();
                        break;
                    case 12:
                        UpdateItemTitle();
                        break;
                    case 13:
                        ChangeBorrowLimit();
                        break;
                    case 14:
                        ViewItemDetails();
                        break;
                    case 15:
                        UpdateItemDetails();
                        break;
                    case 16:
                        SearchMember();
                        break;
                    case 17:
                        ViewActiveLoans();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void AddItem()
        {
            Console.WriteLine("\n=== Add Item ===");
            Console.WriteLine("1. Book");
            Console.WriteLine("2. Magazine");
            Console.WriteLine("3. Research Paper");

            int type = ReadInt("Type: ");
            int id = ReadInt("Item ID: ");
            string title = ReadText("Title: ");

            if (id <= 0 || string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Invalid item data.");
                return;
            }

            LibraryItem? item = null;

            switch (type)
            {
                case 1:
                    string author = ReadText("Author: ");
                    int pages = ReadInt("Pages: ");

                    if (pages <= 0 ||
                        string.IsNullOrWhiteSpace(author))
                        return;

                    item = new Book(id, title, author, pages);
                    break;

                case 2:
                    int issue = ReadInt("Issue Number: ");

                    if (issue <= 0)
                        return;

                    item = new Magazine(id, title, issue);
                    break;

                case 3:
                    string researcher = ReadText("Researcher: ");
                    string field = ReadText("Research Field: ");

                    if (string.IsNullOrWhiteSpace(researcher) ||
                        string.IsNullOrWhiteSpace(field))
                        return;

                    item = new ResearchPaper(
                        id, title, researcher, field
                    );
                    break;

                default:
                    Console.WriteLine("Invalid item type.");
                    return;
            }

            bool added = library.AddItem(item);

            Console.WriteLine(
                added ? "Item added." : "Item ID already exists."
            );
        }

        private void ViewMembers()
        {
            foreach (Member member in library.GetAllMembers())
            {
                member.DisplayInfo();
                Console.WriteLine("-----------------");
            }
        }

        // NEW: Search for member by membership number
        private void SearchMember()
        {
            Console.WriteLine("\n=== Search Member ===");
            int memberId = ReadInt("Member ID: ");

            Member? member = library.SearchMember(memberId);

            if (member == null)
            {
                Console.WriteLine("Member not found.");
                return;
            }

            member.DisplayInfo();

            List<Loan> history = loanService.GetMemberLoans(memberId);
            int activeCount = 0;

            foreach (Loan loan in history)
            {
                if (!loan.IsReturned)
                    activeCount++;
            }

            Console.WriteLine($"Total Loans: {history.Count}");
            Console.WriteLine($"Active Loans: {activeCount}");
            Console.WriteLine(
                $"Unpaid Fines: {fineService.GetMemberUnpaidTotal(memberId)} NIS"
            );
        }

        // NEW: Display all active loans without changing their status
        private void ViewActiveLoans()
        {
            Console.WriteLine("\n=== Active Loans ===");

            List<Loan> activeLoans = loanService.GetActiveLoans();

            if (activeLoans.Count == 0)
            {
                Console.WriteLine("No active loans.");
                return;
            }

            foreach (Loan loan in activeLoans)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine($"Loan ID: {loan.Id}");
                Console.WriteLine($"Member: {loan.Member.Name} ({loan.Member.Id})");
                Console.WriteLine($"Item: {loan.Item.Title} ({loan.Item.Id})");
                Console.WriteLine($"Borrowed: {loan.BorrowDate:yyyy-MM-dd}");
                Console.WriteLine($"Due: {loan.DueDate:yyyy-MM-dd}");

                if (loan.IsOverdue(DateTime.Today))
                {
                    Console.WriteLine(
                        $"Status: Overdue ({loan.GetLateDays(DateTime.Today)} days)"
                    );
                }
                else
                {
                    Console.WriteLine("Status: Active");
                }
            }

            Console.WriteLine("----------------------");
            Console.WriteLine($"Total Active Loans: {activeLoans.Count}");
        }

        private void BorrowItem()
        {
            int memberId = ReadInt("Member ID: ");
            int itemId = ReadInt("Item ID: ");

            Loan? loan = loanService.BorrowItem(
                memberId, itemId
            );

            if (loan == null)
            {
                Console.WriteLine(
                    "Borrow failed. Check availability, " +
                    "membership, limit, or unpaid fines."
                );
                return;
            }

            Console.WriteLine("Borrow successful!");
            Console.WriteLine($"Loan ID: {loan.Id}");
            Console.WriteLine(
                $"Due Date: {loan.DueDate:yyyy-MM-dd}"
            );
        }

        private void ReturnItem()
        {
            int loanId = ReadInt("Loan ID: ");

            bool returned = loanService.ReturnItem(loanId);

            Console.WriteLine(
                returned ? "Return successful." : "Return failed."
            );

            if (!returned)
                return;

            Loan? loan = loanService.SearchLoan(loanId);

            if (loan != null)
            {
                Console.WriteLine(
                    $"Late Days: {loan.GetLateDays(DateTime.Today)}"
                );
                Console.WriteLine(
                    $"Late Fee: {loan.GetLateFee(DateTime.Today)} NIS"
                );
            }
        }

        private void PayFine()
        {
            int fineId = ReadInt("Fine ID: ");
            decimal amount = ReadAmount("Amount: ");
            string reference = ReadText("Payment Reference: ");

            bool paid = fineService.PayFine(
                fineId, amount, reference
            );

            Console.WriteLine(
                paid ? "Payment successful." : "Payment failed."
            );
        }

        private void ReportsMenu()
        {
            Console.WriteLine("\n=== Reports ===");
            Console.WriteLine("1. Items Report");
            Console.WriteLine("2. Most Borrowed Items");
            Console.WriteLine("3. Overdue Members");
            Console.WriteLine("4. Members With Unpaid Fines");
            Console.WriteLine("5. Total Collected Fines");
            Console.WriteLine("6. Member Loan History");

            int choice = ReadInt("Choose: ");

            switch (choice)
            {
                case 1:
                    reportService.ShowItemsReport();
                    break;
                case 2:
                    reportService.ShowMostBorrowedItems();
                    break;
                case 3:
                    reportService.ShowOverdueMembers(DateTime.Today);
                    break;
                case 4:
                    reportService.ShowMembersWithUnpaidFines();
                    break;
                case 5:
                    reportService.ShowTotalCollected();
                    break;
                case 6:
                    int memberId = ReadInt("Member ID: ");
                    reportService.ShowMemberHistory(memberId);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void DisableItem()
        {
            int id = ReadInt("Item ID: ");

            bool success = library.DeactivateItem(id);

            Console.WriteLine(
                success ? "Item disabled." : "Operation failed."
            );
        }

        private void DisableMember()
        {
            int id = ReadInt("Member ID: ");

            bool success = library.DisableMember(id);

            Console.WriteLine(
                success ? "Member disabled." : "Operation failed."
            );
        }

        private void UpdateItemTitle()
        {
            int id = ReadInt("Item ID: ");
            string title = ReadText("New Title: ");

            bool success = library.UpdateItemTitle(id, title);

            Console.WriteLine(
                success ? "Title updated." : "Operation failed."
            );
        }

        private void ChangeBorrowLimit()
        {
            int id = ReadInt("Member ID: ");
            int limit = ReadInt("New Borrow Limit: ");

            bool success = library.ChangeBorrowLimit(id, limit);

            Console.WriteLine(
                success ? "Limit updated." : "Operation failed."
            );
        }

        private void ViewItemDetails()
        {
            Console.WriteLine("\n=== Item Details ===");

            int id = ReadInt("Item ID: ");

            bool found = library.ShowItemDetails(id);

            if (!found)
            {
                Console.WriteLine("Item not found.");
            }
        }

        private void UpdateItemDetails()
        {
            Console.WriteLine("\n=== Update Item Details ===");

            int id = ReadInt("Item ID: ");

            LibraryItem? item = library.SearchItem(id);

            if (item == null)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            Console.WriteLine("\nCurrent Details:");
            item.DisplayInfo();

            bool updated = false;

            if (item is Book)
            {
                Console.WriteLine("\n=== Update Book ===");

                string author = ReadText("New Author: ");
                int pages = ReadInt("New Pages: ");

                updated = library.UpdateBookDetails(
                    id, author, pages
                );
            }
            else if (item is Magazine)
            {
                Console.WriteLine("\n=== Update Magazine ===");

                int issue = ReadInt("New Issue Number: ");

                updated = library.UpdateMagazineDetails(
                    id, issue
                );
            }
            else if (item is ResearchPaper)
            {
                Console.WriteLine("\n=== Update Research Paper ===");

                string researcher = ReadText("New Researcher: ");
                string field = ReadText("New Research Field: ");

                updated = library.UpdateResearchPaperDetails(
                    id, researcher, field
                );
            }

            if (updated)
            {
                Console.WriteLine("\nItem updated successfully!");

                Console.WriteLine("\nUpdated Details:");
                item.DisplayInfo();
            }
            else
            {
                Console.WriteLine("Update failed. Check the values.");
            }
        }
    }
}
