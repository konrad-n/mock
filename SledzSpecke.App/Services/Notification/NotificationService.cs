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

            // Sprawdzenie, czy mamy uprawnienia do wysyłania powiadomień
            var permissionStatus = await this.RequestNotificationPermissionAsync();

            // Inicjalizacja lokalna
            this.isInitialized = true;
        }

        public async Task<bool> RequestNotificationPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Notifications>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Notifications>();
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error requesting notification permission: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AreNotificationsEnabledAsync()
        {
            try
            {
                // Sprawdzenie ustawień aplikacji
                var notificationsEnabled = await Helpers.Settings.GetNotificationsEnabledAsync();

                // Sprawdzenie uprawnień systemowych
                var status = await Permissions.CheckStatusAsync<Permissions.Notifications>();

                return notificationsEnabled && status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking notification status: {ex.Message}");
                return false;
            }
        }

        public async Task ScheduleCourseReminderAsync(int courseId, DateTime date)
        {
            if (!await this.AreNotificationsEnabledAsync())
            {
                return;
            }

            try
            {
                // Pobierz dane kursu
                var course = await this.databaseService.GetCourseAsync(courseId);
                if (course == null)
                {
                    return;
                }

                // Utwórz powiadomienie
                string title = "Przypomnienie o kursie";
                string message = $"Kurs \"{course.CourseName}\" rozpoczyna się jutro.";

                // Utwórz identyfikator dla powiadomienia
                string notificationId = $"course_{courseId}";

                // Zaplanuj powiadomienie jeden dzień przed datą kursu
                DateTime notificationTime = date.AddDays(-1).Date.AddHours(9); // 9:00 rano dzień przed kursem

                await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling course reminder: {ex.Message}");
            }
        }

        public async Task ScheduleShiftReminderAsync(int shiftId, DateTime date)
        {
            if (!await this.AreNotificationsEnabledAsync())
            {
                return;
            }

            try
            {
                // Pobierz dane dyżuru
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    return;
                }

                // Utwórz powiadomienie
                string title = "Przypomnienie o dyżurze";
                string message = $"Masz zaplanowany dyżur jutro w {shift.Location}.";

                // Utwórz identyfikator dla powiadomienia
                string notificationId = $"shift_{shiftId}";

                // Zaplanuj powiadomienie jeden dzień przed datą dyżuru
                DateTime notificationTime = date.AddDays(-1).Date.AddHours(18); // 18:00 wieczorem dzień przed dyżurem

                await this.ScheduleLocalNotificationAsync(notificationId, title, message, notificationTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling shift reminder: {ex.Message}");
            }
        }

        public async Task ScheduleDeadlineReminderAsync(string title, string message, DateTime date)
        {
            if (!await this.AreNotificationsEnabledAsync())
            {
                return;
            }

            try
            {
                // Utwórz identyfikator dla powiadomienia
                string notificationId = $"deadline_{Guid.NewGuid().ToString()}";

                // Pobierz liczbę dni wyprzedzenia z ustawień
                var settings = await Helpers.Settings.GetReminderDaysInAdvanceAsync();
                int daysInAdvance = settings ?? Constants.DefaultReminderDaysInAdvance;

                // Zaplanuj powiadomienie określoną liczbę dni przed terminem
                DateTime notificationTime = date.AddDays(-daysInAdvance).Date.AddHours(9); // 9:00 rano w odpowiednim dniu

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
                // Anulowanie powiadomienia na podstawie ID
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
                // Anulowanie wszystkich powiadomień
                LocalNotificationCenter.Current.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error canceling all reminders: {ex.Message}");
            }
        }

        public async Task ShowNotificationAsync(string title, string message)
        {
            if (!await this.AreNotificationsEnabledAsync())
            {
                return;
            }

            try
            {
                // Pokaż natychmiastowe powiadomienie
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

        // Metoda pomocnicza do planowania lokalnych powiadomień
        private async Task ScheduleLocalNotificationAsync(string notificationId, string title, string message, DateTime scheduleTime)
        {
            try
            {
                // Konwersja notificationId na int (wymagane przez LocalNotificationCenter)
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

                await LocalNotificationCenter.Current.Schedule(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scheduling local notification: {ex.Message}");
            }
        }
    }
}