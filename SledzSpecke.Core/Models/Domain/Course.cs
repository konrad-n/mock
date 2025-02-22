using SledzSpecke.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class Course : BaseEntity
    {
        public int UserId { get; set; }
        public int CourseDefinitionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public CourseStatus Status { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string CertificateNumber { get; set; }
        public string Notes { get; set; }

        // Właściwości nawigacyjne
        public User User { get; set; }
        public CourseDefinition Definition { get; set; }
        public ICollection<CourseDocument> Documents { get; set; }
    }
}
