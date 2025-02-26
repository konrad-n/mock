using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("ProcedureCategory")]
    public class ProcedureCategory : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public Specialization Specialization { get; set; }

        [Ignore]
        public ICollection<ProcedureDefinition> Procedures { get; set; }
    }
}
