using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("Course")]
    public class Course : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("CourseDefinitionId")]
        public int CourseDefinitionId { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("Organizer")]
        public string Organizer { get; set; }

        [Column("Status")]
        public CourseStatus Status { get; set; }

        [Column("IsCompleted")]
        public bool IsCompleted { get; set; }

        [Column("CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Column("CertificateNumber")]
        public string CertificateNumber { get; set; }

        [Column("Notes")]
        public string Notes { get; set; }

        [Column("SpecializationId")]
        public int SpecializationId { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public CourseDefinition Definition { get; set; }

        [Ignore]
        public ICollection<CourseDocument> Documents { get; set; }
    }
}
