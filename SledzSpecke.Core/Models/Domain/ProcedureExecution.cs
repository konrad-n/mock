using SledzSpecke.Core.Models.Enums;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureExecution : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime ExecutionDate { get; set; }
        public ProcedureType Type { get; set; } // Wykonanie/Asysta
        public string Location { get; set; }
        public int? SupervisorId { get; set; }
        public string Notes { get; set; }
        public bool IsSimulation { get; set; }
        public string Category { get; set; }
        public string Stage { get; set; }
        public int? ProcedureRequirementId { get; set; } // Powiązanie z wymaganiem programu specjalizacji
        
        // Nawigacja
        public User User { get; set; }
        public User Supervisor { get; set; }
        public ProcedureRequirement ProcedureRequirement { get; set; }
    }
}