using SledzSpecke.Core.Models;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Models
{
    public class MedicalProcedure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProcedureType ProcedureType { get; set; }
        public int RequiredCount { get; set; }
        public int CompletedCount { get; set; }
        public ModuleType Module { get; set; }
        public int? InternshipId { get; set; }
        public List<ProcedureEntry> Entries { get; set; } = new List<ProcedureEntry>();



        public double CompletionPercentage => RequiredCount > 0 ? (CompletedCount * 100.0 / RequiredCount) : 0;
    }

    public enum ProcedureType
    {
        TypeA,  // wykonywanie samodzielne z asystą lub pod nadzorem
        TypeB   // jako pierwsza asysta
    }

    public class ProcedureEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PatientId { get; set; }
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string Notes { get; set; }
    }
}
