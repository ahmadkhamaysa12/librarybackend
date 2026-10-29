
using LibraryManagementSystem.Models;

List<LibraryItem> items = new List<LibraryItem>();

items.Add(new Book(
    1, "Clean Code", "Robert Martin", 464
));

items.Add(new Magazine(
    2, "Science Today", 15
));

items.Add(new ResearchPaper(
    3, "Artificial Intelligence",
    "Ahmad", "Computer Science"
));

foreach (LibraryItem item in items)
{
    item.DisplayInfo();

    Console.WriteLine("----------------------");
}
