using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("NotificationInfo")]
    public class NotificationInfo : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Type")]
        public NotificationType Type { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("ScheduledTime")]
        public DateTime ScheduledTime { get; set; }

        [Column("IsRead")]
        public bool IsRead { get; set; }

        [Column("RelatedEntityId")]
        public string RelatedEntityId { get; set; }

        [Column("RelatedEntityType")]
        public string RelatedEntityType { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public User User { get; set; }
    }
}
