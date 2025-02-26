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
    }
}
