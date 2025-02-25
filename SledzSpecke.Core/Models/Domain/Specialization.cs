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
        
        // Nawigacja
        public ICollection<CourseDefinition> CourseDefinitions { get; set; }
        public ICollection<InternshipDefinition> InternshipDefinitions { get; set; }
        public ICollection<ProcedureRequirement> ProcedureRequirements { get; set; }
        public ICollection<DutyRequirement> DutyRequirements { get; set; }
        public ICollection<User> Users { get; set; }
    }
}