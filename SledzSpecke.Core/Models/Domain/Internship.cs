using SledzSpecke.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class Internship : BaseEntity
    {
        public int UserId { get; set; }
        public int InternshipDefinitionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public int? SupervisorId { get; set; }
        public InternshipStatus Status { get; set; }
        public string Notes { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }

        public int SpecializationId { get; set; }

        // Właściwości nawigacyjne
        public User User { get; set; }
        public User Supervisor { get; set; }
        public InternshipDefinition Definition { get; set; }
        public ICollection<InternshipDocument> Documents { get; set; }
    }
}
