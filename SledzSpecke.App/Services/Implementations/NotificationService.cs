﻿using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IDatabaseService databaseService,
            ILogger<NotificationService> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public async Task CheckAndScheduleNotificationsAsync()
        {
            try
            {
                // Check if notifications are enabled
                var settings = await this._databaseService.GetUserSettingsAsync();
                if (!settings.EnableNotifications)
                {
                    this._logger.LogInformation("Notifications are disabled. Skipping notification check.");
                    return;
                }

                // Get current specialization
                var specialization = await this._databaseService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    this._logger.LogWarning("No active specialization found. Cannot schedule notifications.");
                    return;
                }

                // Load related data
                var courses = await this._databaseService.QueryAsync<Course>(
                    "SELECT * FROM Courses WHERE SpecializationId = ? AND IsCompleted = 0",
                    specialization.Id);

                var internships = await this._databaseService.QueryAsync<Internship>(
                    "SELECT * FROM Internships WHERE SpecializationId = ? AND IsCompleted = 0",
                    specialization.Id);

                var today = DateTime.Now.Date;

                // Check courses
                foreach (var course in courses.Where(c => c.ScheduledDate.HasValue))
                {
                    int daysUntil = (course.ScheduledDate.Value.Date - today).Days;

                    if (daysUntil == 7) // 1 week before
                    {
                        await this.ScheduleNotificationAsync(
                            $"Zbliża się kurs: {course.Name}",
                            $"Kurs rozpoczyna się za tydzień ({course.ScheduledDate.Value:dd.MM.yyyy})",
                            course.Id,
                            NotificationType.Course
                        );
                    }
                    else if (daysUntil == 1) // 1 day before
                    {
                        await this.ScheduleNotificationAsync(
                            $"Jutro rozpoczyna się kurs: {course.Name}",
                            $"Kurs rozpoczyna się jutro ({course.ScheduledDate.Value:dd.MM.yyyy})",
                            course.Id,
                            NotificationType.Course
                        );
                    }
                }

                // Check internships
                foreach (var internship in internships.Where(i => i.StartDate.HasValue))
                {
                    int daysUntil = (internship.StartDate.Value.Date - today).Days;

                    if (daysUntil == 7) // 1 week before
                    {
                        await this.ScheduleNotificationAsync(
                            $"Zbliża się staż: {internship.Name}",
                            $"Staż rozpoczyna się za tydzień ({internship.StartDate.Value:dd.MM.yyyy})",
                            internship.Id,
                            NotificationType.Internship
                        );
                    }
                    else if (daysUntil == 1) // 1 day before
                    {
                        await this.ScheduleNotificationAsync(
                            $"Jutro rozpoczyna się staż: {internship.Name}",
                            $"Staż rozpoczyna się jutro ({internship.StartDate.Value:dd.MM.yyyy})",
                            internship.Id,
                            NotificationType.Internship
                        );
                    }
                }

                // Check end of basic module
                var endOfBasicModule = specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7);
                if (endOfBasicModule > today)
                {
                    int daysUntilEnd = (endOfBasicModule - today).Days;

                    if (daysUntilEnd == 30) // 1 month before
                    {
                        await this.ScheduleNotificationAsync(
                            "Zbliża się koniec modułu podstawowego",
                            $"Moduł podstawowy kończy się za miesiąc ({endOfBasicModule:dd.MM.yyyy})",
                            0,
                            NotificationType.ModuleEnd
                        );
                    }
                    else if (daysUntilEnd == 7) // 1 week before
                    {
                        await this.ScheduleNotificationAsync(
                            "Zbliża się koniec modułu podstawowego",
                            $"Moduł podstawowy kończy się za tydzień ({endOfBasicModule:dd.MM.yyyy})",
                            0,
                            NotificationType.ModuleEnd
                        );
                    }
                }

                this._logger.LogInformation("Notification check completed successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error checking and scheduling notifications");
            }
        }

        private async Task ScheduleNotificationAsync(string title, string message, int itemId, NotificationType type)
        {
            try
            {
                // In a real implementation, this would use a platform-specific notification API
                // For MAUI, you could use Plugin.LocalNotification or similar

                // For now, just log the notification
                this._logger.LogInformation("Notification scheduled: {Title} - {Message} (Type: {Type}, ItemId: {ItemId})",
                    title, message, type, itemId);

                // Example implementation for Android/iOS would go here
                // This is where you would use the notification plugin to schedule the actual notification

                await Task.CompletedTask; // Placeholder for async implementation
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error scheduling notification");
            }
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