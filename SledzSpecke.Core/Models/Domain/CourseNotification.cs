using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class CourseNotification : BaseEntity
    {
        public int CourseDefinitionId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string Message { get; set; }
        public string RegistrationLink { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public bool IsActive { get; set; }
    }
}
