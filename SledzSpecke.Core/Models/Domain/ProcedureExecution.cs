using SledzSpecke.Core.Models.Enums;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureExecution : BaseEntity
    {
        public int ProcedureDefinitionId { get; set; }
        public int UserId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public ProcedureType Type { get; set; } // Wykonanie/Asysta
        public int? SupervisorId { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        public ProcedureDefinition ProcedureDefinition { get; set; }
        public User User { get; set; }
        public User Supervisor { get; set; }
    }
}
