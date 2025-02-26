using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationInfo>, INotificationRepository
    {
        public NotificationRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<NotificationInfo>> GetUpcomingNotificationsAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<NotificationInfo>()
                .Where(n => n.UserId == userId && n.ScheduledTime > DateTime.Now)
                .OrderBy(n => n.ScheduledTime)
                .ToListAsync();
        }

        public async Task<List<NotificationInfo>> GetUnreadNotificationsAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<NotificationInfo>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderBy(n => n.ScheduledTime)
                .ToListAsync();
        }

        public async Task<List<NotificationInfo>> GetNotificationsByTypeAsync(int userId, NotificationType type)
        {
            await _context.InitializeAsync();
            return await _connection.Table<NotificationInfo>()
                .Where(n => n.UserId == userId && n.Type == type)
                .OrderBy(n => n.ScheduledTime)
                .ToListAsync();
        }

        public async Task<List<NotificationInfo>> GetNotificationsForDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            await _context.InitializeAsync();
            return await _connection.Table<NotificationInfo>()
                .Where(n => n.UserId == userId && n.ScheduledTime >= startDate && n.ScheduledTime <= endDate)
                .OrderBy(n => n.ScheduledTime)
                .ToListAsync();
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.ExecuteAsync(
                "UPDATE NotificationInfo SET IsRead = 1 WHERE UserId = ? AND IsRead = 0", userId);
        }

        public async Task<int> DeleteAllForUserAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.ExecuteAsync(
                "DELETE FROM NotificationInfo WHERE UserId = ?", userId);
        }
    }
}
