namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

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