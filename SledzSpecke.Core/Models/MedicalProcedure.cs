using SledzSpecke.Core.Models.Enums;
using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models
{
    [Table("MedicalProcedures")]
    public class MedicalProcedure
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(255), Indexed]
        public string Name { get; set; }

        public string Description { get; set; }

        public ProcedureType ProcedureType { get; set; }

        public int RequiredCount { get; set; }

        public int CompletedCount { get; set; }

        public ModuleType Module { get; set; }

        [Indexed]
        public int? InternshipId { get; set; }

        public int SpecializationId { get; set; }

        [Ignore]
        public List<ProcedureEntry> Entries { get; set; } = new List<ProcedureEntry>();

        public double CompletionPercentage => RequiredCount > 0 ? (CompletedCount * 100.0 / RequiredCount) : 0;
    }
}