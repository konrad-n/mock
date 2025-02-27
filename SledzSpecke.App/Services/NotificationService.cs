namespace SledzSpecke.App.Services
{
    public class NotificationService
    {
        private readonly DataManager _dataManager;

        public NotificationService(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public async Task CheckAndScheduleNotificationsAsync()
        {
            var specialization = await _dataManager.LoadSpecializationAsync();
            var today = DateTime.Now.Date;

            // Przykładowe sprawdzanie zbliżających się terminów

            // Sprawdź kursy
            foreach (var course in specialization.RequiredCourses.Where(c => !c.IsCompleted && c.ScheduledDate.HasValue))
            {
                int daysUntil = (course.ScheduledDate.Value.Date - today).Days;

                if (daysUntil == 7) // Powiadomienie tydzień przed
                {
                    await ScheduleNotificationAsync(
                        $"Zbliża się kurs: {course.Name}",
                        $"Kurs rozpoczyna się za tydzień ({course.ScheduledDate.Value.ToString("dd.MM.yyyy")})",
                        course.Id,
                        NotificationType.Course
                    );
                }
                else if (daysUntil == 1) // Powiadomienie dzień przed
                {
                    await ScheduleNotificationAsync(
                        $"Jutro rozpoczyna się kurs: {course.Name}",
                        $"Kurs rozpoczyna się jutro ({course.ScheduledDate.Value.ToString("dd.MM.yyyy")})",
                        course.Id,
                        NotificationType.Course
                    );
                }
            }

            // Sprawdź staże
            foreach (var internship in specialization.RequiredInternships.Where(i => !i.IsCompleted && i.StartDate.HasValue))
            {
                int daysUntil = (internship.StartDate.Value.Date - today).Days;

                if (daysUntil == 7) // Powiadomienie tydzień przed
                {
                    await ScheduleNotificationAsync(
                        $"Zbliża się staż: {internship.Name}",
                        $"Staż rozpoczyna się za tydzień ({internship.StartDate.Value.ToString("dd.MM.yyyy")})",
                        internship.Id,
                        NotificationType.Internship
                    );
                }
                else if (daysUntil == 1) // Powiadomienie dzień przed
                {
                    await ScheduleNotificationAsync(
                        $"Jutro rozpoczyna się staż: {internship.Name}",
                        $"Staż rozpoczyna się jutro ({internship.StartDate.Value.ToString("dd.MM.yyyy")})",
                        internship.Id,
                        NotificationType.Internship
                    );
                }
            }

            // Sprawdź dyżury (przykładowa implementacja)
            // Ta część wymagałaby faktycznych danych o dyżurach

            // Sprawdź, czy zbliża się koniec modułu
            if (specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7) > today)
            {
                var endOfBasicModule = specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7);
                int daysUntilEnd = (endOfBasicModule - today).Days;

                if (daysUntilEnd == 30) // Powiadomienie miesiąc przed
                {
                    await ScheduleNotificationAsync(
                        "Zbliża się koniec modułu podstawowego",
                        $"Moduł podstawowy kończy się za miesiąc ({endOfBasicModule.ToString("dd.MM.yyyy")})",
                        0,
                        NotificationType.ModuleEnd
                    );
                }
                else if (daysUntilEnd == 7) // Powiadomienie tydzień przed
                {
                    await ScheduleNotificationAsync(
                        "Zbliża się koniec modułu podstawowego",
                        $"Moduł podstawowy kończy się za tydzień ({endOfBasicModule.ToString("dd.MM.yyyy")})",
                        0,
                        NotificationType.ModuleEnd
                    );
                }
            }
        }

        private async Task ScheduleNotificationAsync(string title, string message, int itemId, NotificationType type)
        {
            // W rzeczywistej implementacji tutaj byłoby planowanie lokalnych powiadomień
            // przy użyciu np. Plugin.LocalNotification

            // Dla celów demonstracyjnych tylko logujemy
            Console.WriteLine($"Scheduled notification: {title} - {message}");

            // Przykładowa implementacja dla Android:
            /*
            var notification = new NotificationRequest
            {
                NotificationId = itemId,
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5)
                }
            };

            await LocalNotificationCenter.Current.Show(notification);
            */

            await Task.CompletedTask; // Placeholder
        }
    }

    public enum NotificationType
    {
        Course,
        Internship,
        Duty,
        Procedure,
        ModuleEnd
    }
}