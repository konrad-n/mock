using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;

namespace SledzSpecke.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserService _userService;
        private readonly ISpecializationService _specializationService;
        private readonly ICourseService _courseService;
        private readonly IInternshipService _internshipService;
        private readonly IDutyService _dutyService;
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationRepository _notificationRepository;
        
        public NotificationService(
            IUserService userService,
            ISpecializationService specializationService,
            ICourseService courseService,
            IInternshipService internshipService,
            IDutyService dutyService,
            INotificationRepository notificationRepository,
            ILogger<NotificationService> logger)
        {
            _userService = userService;
            _specializationService = specializationService;
            _courseService = courseService;
            _internshipService = internshipService;
            _dutyService = dutyService;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }
        
        public async Task ScheduleNotificationAsync(NotificationType type, string title, string message, DateTime scheduledTime)
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                
                var notification = new NotificationInfo
                {
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Message = message,
                    ScheduledTime = scheduledTime,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                
                await _notificationRepository.AddAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling notification");
                throw;
            }
        }
        
        public async Task<List<NotificationInfo>> GetUpcomingNotificationsAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _notificationRepository.GetUpcomingNotificationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming notifications");
                throw;
            }
        }
        
        public async Task GenerateRequirementNotificationsAsync(int specializationId)
        {
            try
            {
                var specialization = await _specializationService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    return;
                }
                
                var progress = await _specializationService.GetProgressStatisticsAsync(specializationId);
                
                // Powiadomienia o kursach
                var courses = await _courseService.GetRequiredCoursesAsync();
                foreach (var course in courses)
                {
                    // Przykład: Planowanie powiadomienia o niezarejestrowanych kursach
                    var title = $"Kurs: {course.Name}";
                    var message = $"Zalecany termin rejestracji na kurs '{course.Name}' się zbliża. Sprawdź dostępne terminy.";
                    await ScheduleNotificationAsync(
                        NotificationType.CourseDeadline,
                        title,
                        message,
                        DateTime.Now.AddDays(7));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating requirement notifications");
                throw;
            }
        }
        
        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    await _notificationRepository.UpdateAsync(notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                throw;
            }
        }
        
        public async Task DeleteNotificationAsync(int notificationId)
        {
            try
            {
                await _notificationRepository.DeleteAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                throw;
            }
        }
    }
}
