namespace SledzSpecke.App.Models
{
    public class InternshipRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weeks { get; set; }
        public int WorkingDays { get; set; }
        public bool IsBasic { get; set; }
        public string Location { get; set; }
        public List<string> ProcedureCodes { get; set; }
    }
}
