using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task ScheduleNotificationAsync(NotificationType type, string title, string message, DateTime scheduledTime);
        Task<List<NotificationInfo>> GetUpcomingNotificationsAsync();
        Task GenerateRequirementNotificationsAsync(int specializationId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
    }
}
