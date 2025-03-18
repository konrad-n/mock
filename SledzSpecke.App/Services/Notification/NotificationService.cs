using System.Security.Cryptography;
using Plugin.LocalNotification;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IDatabaseService databaseService;
        private bool isInitialized = false;

        public NotificationService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            await LocalNotificationCenter.Current.RequestNotificationPermission();

            this.isInitialized = true;
        }

        public async Task<bool> RequestNotificationPermission(object permission = null)
        {
            try
            {
                return await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AreNotificationsEnabled(object permission = null)
        {
            var notificationsEnabled = await Helpers.SettingsHelper.GetNotificationsEnabledAsync();
            var status = await LocalNotificationCenter.Current.AreNotificationsEnabled();

            return notificationsEnabled && status;
        }

        public async Task ScheduleCourseReminderAsync(int courseId, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            var course = await this.databaseService.GetCourseAsync(courseId);
            if (course == null)
            {
                return;
            }

            string title = "Przypomnienie o kursie";
            string message = $"Kurs \"{course.CourseName}\" rozpoczyna się jutro.";
            string notificationId = $"course_{courseId}";
            DateTime notificationTime = date.AddDays(-1).Date.AddHours(9);

            await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);

        }

        public async Task ScheduleShiftReminderAsync(int shiftId, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
            if (shift == null)
            {
                return;
            }

            string title = "Przypomnienie o dyżurze";
            string message = $"Masz zaplanowany dyżur jutro w {shift.Location}.";
            string notificationId = $"shift_{shiftId}";
            DateTime notificationTime = date.AddDays(-1).Date.AddHours(18);

            await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
        }

        public async Task ScheduleDeadlineReminderAsync(string title, string message, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            string notificationId = $"deadline_{Guid.NewGuid().ToString()}";
            var days = await Helpers.SettingsHelper.GetReminderDaysInAdvanceAsync();
            int daysInAdvance = days ?? Constants.DefaultReminderDaysInAdvance;
            DateTime notificationTime = date.AddDays(-daysInAdvance).Date.AddHours(9);

            await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
        }

        public async Task CancelReminderAsync(int reminderId)
        {
            LocalNotificationCenter.Current.Cancel(reminderId);
        }

        public async Task CancelAllRemindersAsync()
        {
            LocalNotificationCenter.Current.CancelAll();
        }

        public async Task ShowNotificationAsync(string title, string message)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }
            int notificationId = this.GenerateSecureRandomId();

            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1),
                },
            };

            await LocalNotificationCenter.Current.Show(notification);
        }

        private int GenerateSecureRandomId()
        {
            byte[] randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Math.Abs(BitConverter.ToInt32(randomBytes, 0));
        }

        private async Task ScheduleLocalNotificationAsync(string notificationId, string title, string message, DateTime scheduleTime)
        {
            int notificationIdInt = Math.Abs(notificationId.GetHashCode());

            var notification = new NotificationRequest
            {
                NotificationId = notificationIdInt,
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = scheduleTime,
                },
            };

            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}