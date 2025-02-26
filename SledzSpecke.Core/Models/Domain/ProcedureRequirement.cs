using SQLite;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("ProcedureRequirement")]
    public class ProcedureRequirement : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Description")]
        public string Description { get; set; } = string.Empty;

        [Column("RequiredCount")]
        public int RequiredCount { get; set; }

        [Column("AssistanceCount")]
        public int AssistanceCount { get; set; }

        [Column("SupervisionRequired")]
        public bool SupervisionRequired { get; set; }

        [Column("Category")]
        public string Category { get; set; } = string.Empty;

        [Column("Stage")]
        public string Stage { get; set; } = string.Empty;

        [Column("AllowSimulation")]
        public bool AllowSimulation { get; set; }

        [Column("SimulationLimit")]
        public int SimulationLimit { get; set; } // Procent procedur możliwych do wykonania na symulatorach

        // Nawigacja
        [Ignore]
        public Specialization Specialization { get; set; } = null!;
    }
}
