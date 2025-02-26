using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("ProcedureExecution")]
    public class ProcedureExecution : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("ExecutionDate")]
        public DateTime ExecutionDate { get; set; }

        [Column("Type")]
        public ProcedureType Type { get; set; } // Wykonanie/Asysta

        [Column("Location")]
        public string Location { get; set; }

        [Column("SupervisorId")]
        public int? SupervisorId { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("IsSimulation")]
        public bool IsSimulation { get; set; }

        [Column("Category")]
        public string Category { get; set; }

        [Column("Stage")]
        public string Stage { get; set; }

        [Column("ProcedureRequirementId")]
        public int? ProcedureRequirementId { get; set; } // Powiązanie z wymaganiem programu specjalizacji

        // Nawigacja
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public User Supervisor { get; set; }

        [Ignore]
        public ProcedureRequirement ProcedureRequirement { get; set; }
    }
}