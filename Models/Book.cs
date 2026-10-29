
namespace LibraryManagementSystem.Models
{
    public class Book : LibraryItem
    {
        public string Author { get; private set; }
        public int Pages { get; private set; }

        public Book(int id, string title,
                    string author, int pages)
            : base(id, title)
        {
            Author = author;
            Pages = pages;
        }

        public override int GetLoanPeriod()
        {
            return 14;
        }

        public override decimal CalculateLateFee(int lateDays)
        {
            if (lateDays <= 0)
                return 0;

            return lateDays * 1.0m;
        }


        public bool UpdateBookDetails(string author, int pages)
        {
            if (string.IsNullOrWhiteSpace(author) || pages <= 0)
                return false;

            Author = author.Trim();
            Pages = pages;

            return true;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();

            Console.WriteLine($"Type: Book");
            Console.WriteLine($"Author: {Author}");
            Console.WriteLine($"Pages: {Pages}");
        }

    }
}
