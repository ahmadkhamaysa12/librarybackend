
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class Library
    {
        private List<LibraryItem> items;
        private List<Member> members;

        public Library()
        {
            items = new List<LibraryItem>();
            members = new List<Member>();
        }

        // ========== ITEMS ==========

        public bool AddItem(LibraryItem item)
        {
            if (item.Id <= 0 || SearchItem(item.Id) != null)
                return false;

            items.Add(item);
            return true;
        }

        public List<LibraryItem> GetAllItems()
        {
            return new List<LibraryItem>(items);
        }

        // Search by ID
        public LibraryItem? SearchItem(int id)
        {
            foreach (LibraryItem item in items)
            {
                if (item.Id == id)
                    return item;
            }

            return null;
        }

        // Search by title
        public List<LibraryItem> SearchItem(string title)
        {
            List<LibraryItem> results =
                new List<LibraryItem>();

            if (string.IsNullOrWhiteSpace(title))
                return results;

            foreach (LibraryItem item in items)
            {
                if (item.Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public List<LibraryItem> GetAvailableItems()
        {
            List<LibraryItem> available =
                new List<LibraryItem>();

            foreach (LibraryItem item in items)
            {
                if (item.IsAvailable())
                    available.Add(item);
            }

            return available;
        }

        public bool UpdateItemTitle(int id, string title)
        {
            LibraryItem? item = SearchItem(id);

            if (item == null ||
                string.IsNullOrWhiteSpace(title))
                return false;

            item.UpdateTitle(title);
            return true;
        }

        public bool DeactivateItem(int id)
        {
            LibraryItem? item = SearchItem(id);

            if (item == null || !item.IsActive)
                return false;

            item.Deactivate();
            return true;
        }

        // ========== MEMBERS ==========

        public bool AddMember(Member member)
        {
            if (member.Id <= 0 ||
                SearchMember(member.Id) != null)
                return false;

            foreach (Member m in members)
            {
                if (m.Username.Equals(
                    member.Username,
                    StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            members.Add(member);
            return true;
        }

        public List<Member> GetAllMembers()
        {
            return new List<Member>(members);
        }

        public Member? SearchMember(int id)
        {
            foreach (Member member in members)
            {
                if (member.Id == id)
                    return member;
            }

            return null;
        }

        public bool DisableMember(int id)
        {
            Member? member = SearchMember(id);

            if (member == null || !member.IsActive)
                return false;

            member.Deactivate();
            return true;
        }

        public bool ChangeBorrowLimit(int id, int limit)
        {
            Member? member = SearchMember(id);

            if (member == null || limit <= 0)
                return false;

            member.SetMaxLoans(limit);
            return true;
        }

        // Update book details
        public bool UpdateBookDetails(
            int id, string author, int pages)
        {
            LibraryItem? item = SearchItem(id);

            if (item is Book book)
            {
                return book.UpdateBookDetails(author, pages);
            }

            return false;
        }

        // Update magazine details
        public bool UpdateMagazineDetails(int id, int issueNumber)
        {
            LibraryItem? item = SearchItem(id);

            if (item is Magazine magazine)
            {
                return magazine.UpdateIssueNumber(issueNumber);
            }

            return false;
        }

        // Update research paper details
        public bool UpdateResearchPaperDetails(
            int id, string researcher, string field)
        {
            LibraryItem? item = SearchItem(id);

            if (item is ResearchPaper paper)
            {
                return paper.UpdateResearchDetails(researcher, field);
            }

            return false;
        }

        // Display item details
        public bool ShowItemDetails(int id)
        {
            LibraryItem? item = SearchItem(id);

            if (item == null)
                return false;

            item.DisplayInfo();

            return true;
        }

    }
}
