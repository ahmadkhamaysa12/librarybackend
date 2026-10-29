
namespace LibraryManagementSystem.Models
{
    public class Magazine : LibraryItem
    {
        public int IssueNumber { get; private set; }

        public Magazine(int id, string title,
                        int issueNumber)
            : base(id, title)
        {
            IssueNumber = issueNumber;
        }

        public override int GetLoanPeriod()
        {
            return 7;
        }

        public override decimal CalculateLateFee(int lateDays)
        {
            if (lateDays <= 0)
                return 0;

            return lateDays * 2.0m;
        }


public bool UpdateIssueNumber(int issueNumber)
{
    if (issueNumber <= 0)
        return false;

    IssueNumber = issueNumber;

    return true;
}

public override void DisplayInfo()
{
    base.DisplayInfo();

    Console.WriteLine("Type: Magazine");
    Console.WriteLine($"Issue Number: {IssueNumber}");
}

    }
}
