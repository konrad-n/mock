namespace SledzSpecke.Core.Models.Requirements
{
    public class RequiredProcedure
    {
        public string Name { get; set; }
        public int RequiredCount { get; set; }
        public int AssistanceCount { get; set; }
        public string Description { get; set; }
        public bool AllowSimulation { get; set; } = false;
        public int? SimulationLimit { get; set; } // procent procedur możliwych do wykonania na symulatorach
        public int SpecializationId { get; set; } // ID specjalizacji, do której należy procedura
    }
}
