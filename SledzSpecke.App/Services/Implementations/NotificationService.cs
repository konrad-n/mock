using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(
            IDatabaseService databaseService,
            ILogger<NotificationService> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task CheckAndScheduleNotificationsAsync()
        {
            try
            {
                var settings = await this.databaseService.GetUserSettingsAsync();
                if (!settings.EnableNotifications)
                {
                    this.logger.LogInformation("Notifications are disabled. Skipping notification check.");
                    return;
                }

                var specialization = await this.databaseService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    this.logger.LogWarning("No active specialization found. Cannot schedule notifications.");
                    return;
                }

                var courses = await this.databaseService.QueryAsync<Course>(
                    "SELECT * FROM Courses WHERE SpecializationId = ? AND IsCompleted = 0",
                    specialization.Id);

                var internships = await this.databaseService.QueryAsync<Internship>(
                    "SELECT * FROM Internships WHERE SpecializationId = ? AND IsCompleted = 0",
                    specialization.Id);

                var today = DateTime.Now.Date;

                foreach (var course in courses.Where(c => c.ScheduledDate.HasValue))
                {
                    int daysUntil = (course.ScheduledDate!.Value.Date - today).Days;

                    if (daysUntil == 7)
                    {
                        await this.ScheduleNotificationAsync(
                            $"Zbliza sie kurs: {course.Name}",
                            $"Kurs rozpoczyna sie za tydzien ({course.ScheduledDate.Value:dd.MM.yyyy})",
                            course.Id,
                            NotificationType.Course);
                    }
                    else if (daysUntil == 1)
                    {
                        await this.ScheduleNotificationAsync(
                            $"Jutro rozpoczyna sie kurs: {course.Name}",
                            $"Kurs rozpoczyna sie jutro ({course.ScheduledDate.Value:dd.MM.yyyy})",
                            course.Id,
                            NotificationType.Course);
                    }
                }

                foreach (var internship in internships.Where(i => i.StartDate.HasValue))
                {
                    int daysUntil = (internship.StartDate!.Value.Date - today).Days;

                    if (daysUntil == 7)
                    {
                        await this.ScheduleNotificationAsync(
                            $"Zbliza sie staz: {internship.Name}",
                            $"Staz rozpoczyna sie za tydzien ({internship.StartDate.Value:dd.MM.yyyy})",
                            internship.Id,
                            NotificationType.Internship);
                    }
                    else if (daysUntil == 1)
                    {
                        await this.ScheduleNotificationAsync(
                            $"Jutro rozpoczyna sie staz: {internship.Name}",
                            $"Staz rozpoczyna sie jutro ({internship.StartDate.Value:dd.MM.yyyy})",
                            internship.Id,
                            NotificationType.Internship);
                    }
                }

                var endOfBasicModule = specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7);
                if (endOfBasicModule > today)
                {
                    int daysUntilEnd = (endOfBasicModule - today).Days;

                    if (daysUntilEnd == 30)
                    {
                        await this.ScheduleNotificationAsync(
                            "Zbliza sie koniec modulu podstawowego",
                            $"Modul podstawowy konczy sie za miesiac ({endOfBasicModule:dd.MM.yyyy})",
                            0,
                            NotificationType.ModuleEnd);
                    }
                    else if (daysUntilEnd == 7)
                    {
                        await this.ScheduleNotificationAsync(
                            "Zbliza sie koniec modulu podstawowego",
                            $"Modul podstawowy konczy sie za tydzien ({endOfBasicModule:dd.MM.yyyy})",
                            0,
                            NotificationType.ModuleEnd);
                    }
                }

                this.logger.LogInformation("Notification check completed successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error checking and scheduling notifications");
            }
        }

        private async Task ScheduleNotificationAsync(string title, string message, int itemId, NotificationType type)
        {
            try
            {
                this.logger.LogInformation(
                    "Notification scheduled: {Title} - {Message} (Type: {Type}, ItemId: {ItemId})",
                    title,
                    message,
                    type,
                    itemId);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error scheduling notification");
            }
        }
    }
}
