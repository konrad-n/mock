using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;
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

    [Table("ProcedureEntries")]
    public class ProcedureEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string PatientId { get; set; }

        public string Location { get; set; }

        public string SupervisorName { get; set; }

        public string Notes { get; set; }

        [Indexed]
        public int ProcedureId { get; set; }
    }
}