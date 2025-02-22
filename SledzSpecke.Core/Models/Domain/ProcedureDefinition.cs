using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureDefinition : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredCount { get; set; }
        public int RequiredAssistCount { get; set; }
        public bool SupervisionRequired { get; set; }
        public string Category { get; set; }

        // Właściwości nawigacyjne
        public Specialization Specialization { get; set; }
        public ICollection<ProcedureExecution> Executions { get; set; }
    }
}
