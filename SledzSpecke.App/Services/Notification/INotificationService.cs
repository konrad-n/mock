using System;
using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Notification
{
    public interface INotificationService
    {
        Task Initialize();

        Task<bool> RequestNotificationPermissionAsync();

        Task<bool> AreNotificationsEnabledAsync();

        Task ScheduleCourseReminderAsync(int courseId, DateTime date);

        Task ScheduleShiftReminderAsync(int shiftId, DateTime date);

        Task ScheduleDeadlineReminderAsync(string title, string message, DateTime date);

        Task CancelReminderAsync(int reminderId);

        Task CancelAllRemindersAsync();

        Task ShowNotificationAsync(string title, string message);
    }
}