using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class NotificationInfo : BaseEntity
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsRead { get; set; }
        public string RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        
        // Właściwości nawigacyjne
        public User User { get; set; }
    }
    
    public enum NotificationType
    {
        CourseDeadline = 1,
        ProcedureReminder = 2,
        DutyUpcoming = 3,
        InternshipDeadline = 4,
        SpecializationRequirement = 5
    }
}
