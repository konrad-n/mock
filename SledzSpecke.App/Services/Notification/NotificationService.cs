using System;
using System.Threading.Tasks;
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

            // Check notification permissions
            try
            {
                // Use the correct method name from the plugin
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error requesting notification permission: {ex.Message}");
            }

            this.isInitialized = true;
        }

        // Updated method name to match interface
        public async Task<bool> RequestNotificationPermission(object permission = null)
        {
            try
            {
                // Use the plugin's method
                return await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error requesting notification permission: {ex.Message}");
                return false;
            }
        }

        // Updated method name to match interface
        public async Task<bool> AreNotificationsEnabled(object permission = null)
        {
            try
            {
                // Check app settings
                var notificationsEnabled = await Helpers.Settings.GetNotificationsEnabledAsync();

                // Check system permissions
                var status = await LocalNotificationCenter.Current.AreNotificationsEnabled();

                return notificationsEnabled && status;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking notification status: {ex.Message}");
                return false;
            }
        }

        public async Task ScheduleCourseReminderAsync(int courseId, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            try
            {
                // Get course data
                var course = await this.databaseService.GetCourseAsync(courseId);
                if (course == null)
                {
                    return;
                }

                // Create notification
                string title = "Przypomnienie o kursie";
                string message = $"Kurs \"{course.CourseName}\" rozpoczyna się jutro.";

                // Create notification ID
                string notificationId = $"course_{courseId}";

                // Schedule notification one day before course date
                DateTime notificationTime = date.AddDays(-1).Date.AddHours(9); // 9:00 AM the day before

                await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling course reminder: {ex.Message}");
            }
        }

        public async Task ScheduleShiftReminderAsync(int shiftId, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            try
            {
                // Get shift data
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    return;
                }

                // Create notification
                string title = "Przypomnienie o dyżurze";
                string message = $"Masz zaplanowany dyżur jutro w {shift.Location}.";

                // Create notification ID
                string notificationId = $"shift_{shiftId}";

                // Schedule notification one day before shift date
                DateTime notificationTime = date.AddDays(-1).Date.AddHours(18); // 6:00 PM the day before

                await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling shift reminder: {ex.Message}");
            }
        }

        public async Task ScheduleDeadlineReminderAsync(string title, string message, DateTime date)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            try
            {
                // Create notification ID
                string notificationId = $"deadline_{Guid.NewGuid().ToString()}";

                // Get days in advance from settings
                var days = await Helpers.Settings.GetReminderDaysInAdvanceAsync();
                int daysInAdvance = days ?? Constants.DefaultReminderDaysInAdvance;

                // Schedule notification specific number of days before deadline
                DateTime notificationTime = date.AddDays(-daysInAdvance).Date.AddHours(9); // 9:00 AM on appropriate day

                await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling deadline reminder: {ex.Message}");
            }
        }

        public async Task CancelReminderAsync(int reminderId)
        {
            try
            {
                // Cancel notification by ID
                LocalNotificationCenter.Current.Cancel(reminderId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling reminder: {ex.Message}");
            }
        }

        public async Task CancelAllRemindersAsync()
        {
            try
            {
                // Cancel all notifications
                LocalNotificationCenter.Current.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling all reminders: {ex.Message}");
            }
        }

        public async Task ShowNotificationAsync(string title, string message)
        {
            if (!await this.AreNotificationsEnabled())
            {
                return;
            }

            try
            {
                // Show immediate notification
                var notification = new NotificationRequest
                {
                    NotificationId = new Random().Next(1000, 10000),
                    Title = title,
                    Description = message,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1),
                    },
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing notification: {ex.Message}");
            }
        }

        // Helper method for scheduling local notifications
        private async Task ScheduleLocalNotificationAsync(string notificationId, string title, string message, DateTime scheduleTime)
        {
            try
            {
                // Convert notificationId to int (required by LocalNotificationCenter)
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

                // Use the correct method - Show instead of Schedule
                await LocalNotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling local notification: {ex.Message}");
            }
        }
    }
}