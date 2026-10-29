
namespace LibraryManagementSystem.Models
{
    public abstract class LibraryItem
    {
        public int Id { get; private set; }
        public string Title { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsBorrowed { get; private set; }

        // Constructor
        public LibraryItem(int id, string title)
        {
            Id = id;
            Title = title;
            IsActive = true;
            IsBorrowed = false;
        }

        // Check if item is available
        public bool IsAvailable()
        {
            return IsActive && !IsBorrowed;
        }

        // Mark item as borrowed
        // Only accessible inside the same assembly
        internal bool MarkBorrowed()
        {
            if (!IsAvailable())
                return false;

            IsBorrowed = true;
            return true;
        }

        // Mark item as returned
        // Only accessible inside the same assembly
        internal bool MarkReturned()
        {
            if (!IsBorrowed)
                return false;

            IsBorrowed = false;
            return true;
        }

        // Disable item without deleting its history
        public void Deactivate()
        {
            IsActive = false;
        }

        // Update item title
        public void UpdateTitle(string newTitle)
        {
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                Title = newTitle.Trim();
            }
        }

        // Abstract methods
        public abstract int GetLoanPeriod();

        public abstract decimal CalculateLateFee(int lateDays);

        // Display common item information
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Title: {Title}");
            Console.WriteLine($"Active: {IsActive}");
            Console.WriteLine($"Borrowed: {IsBorrowed}");
            Console.WriteLine($"Available: {IsAvailable()}");
        }
    }
}
