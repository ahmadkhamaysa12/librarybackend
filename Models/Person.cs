
namespace LibraryManagementSystem.Models
{
    public abstract class Person
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Username { get; private set; }

        public Person(int id, string name, string username)
        {
            Id = id;
            Name = name;
            Username = username;
        }

        public void UpdateName(string newName)
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                Name = newName;
            }
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Username: {Username}");
        }
    }
}
