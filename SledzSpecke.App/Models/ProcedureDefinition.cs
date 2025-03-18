namespace SledzSpecke.App.Models
{
    public class ProcedureDefinition
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int RequiredCountA { get; set; }

        public int RequiredCountB { get; set; }

        public string Group { get; set; }

        public int? InternshipTypeId { get; set; }
    }
}
