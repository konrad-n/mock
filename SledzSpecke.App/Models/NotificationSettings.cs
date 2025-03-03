namespace SledzSpecke.App.Models
{
    public class NotificationSettings
    {
        public bool NotificationsEnabled { get; set; }

        public bool ShiftRemindersEnabled { get; set; }

        public bool CourseRemindersEnabled { get; set; }

        public bool DeadlineRemindersEnabled { get; set; }

        public int ReminderDaysInAdvance { get; set; } = 7;
    }
}
