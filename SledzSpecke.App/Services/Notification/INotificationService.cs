namespace SledzSpecke.App.Services.Notification
{
    public interface INotificationService
    {
        Task Initialize();

        // Change to match plugin method name
        Task<bool> RequestNotificationPermission(object permission = null);

        // Change to match plugin method name
        Task<bool> AreNotificationsEnabled(object permission = null);

        Task ScheduleCourseReminderAsync(int courseId, DateTime date);

        Task ScheduleShiftReminderAsync(int shiftId, DateTime date);

        Task ScheduleDeadlineReminderAsync(string title, string message, DateTime date);

        Task CancelReminderAsync(int reminderId);

        Task CancelAllRemindersAsync();

        Task ShowNotificationAsync(string title, string message);
    }
}