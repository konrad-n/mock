using SQLite;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("Specialization")]
    public class Specialization : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("DurationInWeeks")]
        public int DurationInWeeks { get; set; }

        [Column("ProgramVersion")]
        public string ProgramVersion { get; set; }

        [Column("ApprovalDate")]
        public DateTime? ApprovalDate { get; set; }

        [Column("MinimumDutyHours")]
        public decimal MinimumDutyHours { get; set; }

        [Column("Requirements")]
        public string Requirements { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        // Navigation properties - mark them to be ignored by SQLite
        [Ignore]
        public ICollection<CourseDefinition> RequiredCourses { get; set; } = new List<CourseDefinition>();

        [Ignore]
        public ICollection<InternshipDefinition> RequiredInternships { get; set; } = new List<InternshipDefinition>();

        [Ignore]
        public ICollection<ProcedureRequirement> ProcedureRequirements { get; set; } = new List<ProcedureRequirement>();

        [Ignore]
        public ICollection<DutyRequirement> DutyRequirements { get; set; } = new List<DutyRequirement>();

        [Ignore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}