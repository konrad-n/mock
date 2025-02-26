using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("WorktimeRecord")]
    public class WorktimeRecord : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        [Column("StartTime")]
        public DateTime StartTime { get; set; }

        [Column("EndTime")]
        public DateTime EndTime { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("Department")]
        public string Department { get; set; }

        [Column("Type")]
        public WorktimeType Type { get; set; }

        [Column("IsApproved")]
        public bool IsApproved { get; set; }

        [Column("SupervisorId")]
        public int? SupervisorId { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("Activities")]
        public string Activities { get; set; }

        // Powiązania z innymi encjami
        [Ignore]
        public List<ProcedureExecution> RelatedProcedures { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public Specialization Specialization { get; set; }

        [Ignore]
        public User Supervisor { get; set; }

        // Wyliczane właściwości
        [Ignore]
        public double DurationInHours => (EndTime - StartTime).TotalHours;
    }
}
