
namespace LibraryManagementSystem.Models
{
    public class ResearchPaper : LibraryItem
    {
        public string Researcher { get; private set; }
        public string ResearchField { get; private set; }

        public ResearchPaper(int id, string title,
                             string researcher,
                             string researchField)
            : base(id, title)
        {
            Researcher = researcher;
            ResearchField = researchField;
        }

        public override int GetLoanPeriod()
        {
            return 21;
        }

        public override decimal CalculateLateFee(int lateDays)
        {
            if (lateDays <= 0)
                return 0;

            return lateDays * 0.5m;
        }


public bool UpdateResearchDetails(
    string researcher,
    string researchField)
{
    if (string.IsNullOrWhiteSpace(researcher) ||
        string.IsNullOrWhiteSpace(researchField))
        return false;

    Researcher = researcher.Trim();
    ResearchField = researchField.Trim();

    return true;
}

public override void DisplayInfo()
{
    base.DisplayInfo();

    Console.WriteLine("Type: Research Paper");
    Console.WriteLine($"Researcher: {Researcher}");
    Console.WriteLine($"Research Field: {ResearchField}");
}

    }
}
