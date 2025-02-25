using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class WorktimeRecord : BaseEntity
    {
        public int UserId { get; set; }
        public int SpecializationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public WorktimeType Type { get; set; }
        public bool IsApproved { get; set; }
        public int? SupervisorId { get; set; }
        public string Notes { get; set; }
        public string Activities { get; set; }
        
        // Powiązania z innymi encjami
        public List<ProcedureExecution> RelatedProcedures { get; set; }
        
        // Właściwości nawigacyjne
        public User User { get; set; }
        public Specialization Specialization { get; set; }
        public User Supervisor { get; set; }
        
        // Wyliczane właściwości
        public double DurationInHours => (EndTime - StartTime).TotalHours;
    }
    
    public enum WorktimeType
    {
        Regular = 1,
        Overtime = 2,
        Research = 3,
        Teaching = 4,
        Conference = 5,
        Other = 6
    }
}
