// Zaktualizować SledzSpecke.Core/Models/Domain/ProcedureRequirement.cs
using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;

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

        [Column("Module")]
        public ModuleType Module { get; set; }

        [Column("InternshipId")]
        public int? InternshipId { get; set; }

        // Nawigacja
        [Ignore]
        public Specialization Specialization { get; set; } = null!;

        [Ignore]
        public InternshipDefinition Internship { get; set; }

        // Pomocnicza metoda do obliczania procentu ukończenia
        public double GetCompletionPercentage(int completedCount)
        {
            if (RequiredCount <= 0) return 0;
            return Math.Min((completedCount * 100.0) / RequiredCount, 100.0);
        }
    }
}
