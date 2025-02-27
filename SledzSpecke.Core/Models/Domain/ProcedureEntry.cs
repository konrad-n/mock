// Dodać do SledzSpecke.Core/Models/Domain/ProcedureEntry.cs
using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("ProcedureEntry")]
    public class ProcedureEntry : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("ProcedureExecutionId")]
        public int ProcedureExecutionId { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("PatientId")]
        public string PatientId { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("SupervisorName")]
        public string SupervisorName { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        // Nawigacja
        [Ignore]
        public ProcedureExecution ProcedureExecution { get; set; }
    }
}
