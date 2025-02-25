namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureRequirement : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredCount { get; set; }
        public int AssistanceCount { get; set; }
        public bool SupervisionRequired { get; set; }
        public string Category { get; set; }
        public string Stage { get; set; }
        public bool AllowSimulation { get; set; }
        public int SimulationLimit { get; set; } // Procent procedur możliwych do wykonania na symulatorach
        
        // Nawigacja
        public Specialization Specialization { get; set; }
    }
}
