using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IUserService _userService;
        private readonly ILogger<CourseService> _logger;

        public CourseService(
            ICourseRepository repository,
            IUserService userService,
            ILogger<CourseService> logger)
        {
            _repository = repository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<List<Course>> GetUserCoursesAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _repository.GetUserCoursesAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user courses");
                throw;
            }
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course {CourseId}", id);
                throw;
            }
        }

        public async Task<Course> RegisterForCourseAsync(Course course)
        {
            try
            {
                course.UserId = await _userService.GetCurrentUserIdAsync();
                course.Status = Core.Models.Enums.CourseStatus.Registered;
                course.CreatedAt = DateTime.UtcNow;
                
                await _repository.AddAsync(course);
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering for course");
                throw;
            }
        }

        public async Task<bool> CompleteCourseAsync(int courseId, CourseDocument certificate)
        {
            try
            {
                var course = await _repository.GetByIdAsync(courseId);
                if (course == null)
                {
                    throw new NotFoundException("Course not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (course.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot complete other user's course");
                }

                course.Status = Core.Models.Enums.CourseStatus.Completed;
                course.IsCompleted = true;
                course.CompletionDate = DateTime.Now;
                course.ModifiedAt = DateTime.UtcNow;
                
                if (certificate != null)
                {
                    certificate.CourseId = courseId;
                    certificate.CreatedAt = DateTime.UtcNow;
                }

                await _repository.UpdateAsync(course);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing course {CourseId}", courseId);
                throw;
            }
        }

        public async Task<List<CourseDefinition>> GetRequiredCoursesAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetRequiredCoursesAsync(user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required courses");
                throw;
            }
        }

        public async Task<List<CourseDefinition>> GetCoursesByYearAsync(int year)
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetCoursesByYearAsync(user.CurrentSpecializationId.Value, year);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses by year");
                throw;
            }
        }

        public async Task<List<string>> GetCourseTopicsAsync(int courseDefinitionId)
        {
            try
            {
                return await _repository.GetCourseTopicsAsync(courseDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course topics");
                throw;
            }
        }

        public async Task<double> GetCourseProgressAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetCourseProgressAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress");
                throw;
            }
        }

        public async Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                return await _repository.GetCourseProgressByYearAsync(userId, user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress by year");
                throw;
            }
        }

        public async Task<List<CourseDefinition>> GetRecommendedCoursesForCurrentYearAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    throw new NotFoundException("Current specialization not found");
                }
                
                // Oblicz aktualny rok specjalizacji na podstawie daty rozpoczęcia
                int currentYear = 1;
                if (user.SpecializationStartDate != default)
                {
                    var yearsInProgram = (DateTime.Today - user.SpecializationStartDate).Days / 365;
                    currentYear = Math.Max(1, Math.Min(6, yearsInProgram + 1)); // Zakładamy max 6 lat specjalizacji
                }
                
                return await GetCoursesByYearAsync(currentYear);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended courses for current year");
                throw;
            }
        }
    }
}
