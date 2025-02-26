using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("Internship")]
    public class Internship : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("InternshipDefinitionId")]
        public int InternshipDefinitionId { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("SupervisorId")]
        public int? SupervisorId { get; set; }

        [Column("Status")]
        public InternshipStatus Status { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("IsCompleted")]
        public bool IsCompleted { get; set; }

        [Column("CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public User Supervisor { get; set; }

        [Ignore]
        public InternshipDefinition Definition { get; set; }

        [Ignore]
        public ICollection<InternshipDocument> Documents { get; set; }
    }
}
