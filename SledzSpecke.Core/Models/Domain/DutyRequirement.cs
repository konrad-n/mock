namespace SledzSpecke.Core.Models.Domain
{
    public class DutyRequirement : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Type { get; set; }
        public decimal RequiredHours { get; set; }
        public string Description { get; set; }
        public int RequiredYear { get; set; } // Rok specjalizacji

        // Właściwości nawigacyjne
        public Specialization Specialization { get; set; }
    }
}
