namespace SledzSpecke.App.Models
{
    public class ProcedureRequirement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int RequiredCountA { get; set; }
        public int RequiredCountB { get; set; }
        public int? InternshipId { get; set; }
    }
}