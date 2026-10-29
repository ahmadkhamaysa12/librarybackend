
namespace LibraryManagementSystem.Models
{
    public class Member : Person
    {
        public int MaxLoans { get; private set; }
        public bool IsActive { get; private set; }

        public Member(int id, string name,
                      string username, int maxLoans)
            : base(id, name, username)
        {
            MaxLoans = maxLoans > 0 ? maxLoans : 3;
            IsActive = true;
        }

        public void SetMaxLoans(int newLimit)
        {
            if (newLimit > 0)
            {
                MaxLoans = newLimit;
            }
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();

            Console.WriteLine($"Max Loans: {MaxLoans}");
            Console.WriteLine($"Active: {IsActive}");
        }
    }
}
