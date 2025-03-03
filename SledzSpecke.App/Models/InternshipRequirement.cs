namespace SledzSpecke.App.Models
{
    public class InternshipRequirement
    {
        public string InternshipCode { get; set; }

        public string InternshipName { get; set; }

        public int DurationWeeks { get; set; }

        public int DurationDays { get; set; }

        public bool IsMandatory { get; set; }

        public List<string> ProcedureCodes { get; set; } // Powiązane procedury
    }
}
