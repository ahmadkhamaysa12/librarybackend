
namespace LibraryManagementSystem.Models
{
    public class Librarian : Person
    {
        public string EmployeeNumber { get; private set; }

        public Librarian(int id, string name,
                         string username, string employeeNumber)
            : base(id, name, username)
        {
            EmployeeNumber = employeeNumber;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();

            Console.WriteLine($"Employee Number: {EmployeeNumber}");
        }
    }
}
