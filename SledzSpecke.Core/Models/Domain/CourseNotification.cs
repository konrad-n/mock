using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("CourseNotification")]
    public class CourseNotification : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("CourseDefinitionId")]
        public int CourseDefinitionId { get; set; }

        [Column("NotificationDate")]
        public DateTime NotificationDate { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("RegistrationLink")]
        public string RegistrationLink { get; set; }

        [Column("RegistrationDeadline")]
        public DateTime? RegistrationDeadline { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
