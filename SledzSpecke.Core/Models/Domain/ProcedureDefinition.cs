using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("ProcedureDefinition")]
    public class ProcedureDefinition : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("RequiredCount")]
        public int RequiredCount { get; set; }

        [Column("RequiredAssistCount")]
        public int RequiredAssistCount { get; set; }

        [Column("SupervisionRequired")]
        public bool SupervisionRequired { get; set; }

        [Column("Category")]
        public string Category { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public Specialization Specialization { get; set; }

        [Ignore]
        public ICollection<ProcedureExecution> Executions { get; set; }
    }
}
