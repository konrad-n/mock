using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }
        public int DurationInWeeks { get; set; }
        public string ProgramVersion { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public decimal MinimumDutyHours { get; set; }
        public string Requirements { get; set; }
        public string Description { get; set; }

        // Navigation properties
        [SQLite.Ignore]
        public ICollection<CourseDefinition> RequiredCourses { get; set; } = new List<CourseDefinition>();

        [SQLite.Ignore]
        public ICollection<InternshipDefinition> RequiredInternships { get; set; } = new List<InternshipDefinition>();

        [SQLite.Ignore]
        public ICollection<ProcedureRequirement> ProcedureRequirements { get; set; } = new List<ProcedureRequirement>();

        [SQLite.Ignore]
        public ICollection<DutyRequirement> DutyRequirements { get; set; } = new List<DutyRequirement>();

        [SQLite.Ignore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}