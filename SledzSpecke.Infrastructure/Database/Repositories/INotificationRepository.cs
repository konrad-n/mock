using SledzSpecke.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface INotificationRepository : IBaseRepository<NotificationInfo>
    {
        Task<List<NotificationInfo>> GetUpcomingNotificationsAsync(int userId);
        Task<List<NotificationInfo>> GetUnreadNotificationsAsync(int userId);
        Task<List<NotificationInfo>> GetNotificationsByTypeAsync(int userId, NotificationType type);
        Task<List<NotificationInfo>> GetNotificationsForDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<int> MarkAllAsReadAsync(int userId);
        Task<int> DeleteAllForUserAsync(int userId);
    }
}
