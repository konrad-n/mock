using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class ProcedureCategory : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SpecializationId { get; set; }

        // Właściwości nawigacyjne
        public Specialization Specialization { get; set; }
        public ICollection<ProcedureDefinition> Procedures { get; set; }
    }
}
