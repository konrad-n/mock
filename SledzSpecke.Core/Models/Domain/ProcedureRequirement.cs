namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureRequirement : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequiredCount { get; set; }
        public int AssistanceCount { get; set; }
        public bool SupervisionRequired { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public bool AllowSimulation { get; set; }
        public int SimulationLimit { get; set; } // Procent procedur możliwych do wykonania na symulatorach
        
        // Nawigacja
        public Specialization Specialization { get; set; } = null!;
    }
}
