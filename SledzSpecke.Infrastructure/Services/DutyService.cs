using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class DutyService : IDutyService
    {
        private readonly IDutyRepository _repository;
        private readonly IUserService _userService;
        private readonly ILogger<DutyService> _logger;

        public DutyService(
            IDutyRepository repository,
            IUserService userService,
            ILogger<DutyService> logger)
        {
            _repository = repository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<List<Duty>> GetUserDutiesAsync(DateTime? fromDate = null)
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var startDate = fromDate ?? DateTime.Today.AddMonths(-1);
                return await _repository.GetUserDutiesInRangeAsync(userId, startDate, DateTime.Today.AddYears(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user duties");
                throw;
            }
        }

        public async Task<Duty> GetDutyAsync(int id)
        {
            try
            {
                var duty = await _repository.GetByIdAsync(id);
                if (duty == null)
                {
                    throw new NotFoundException("Duty not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (duty.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot access other user's duty");
                }

                return duty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting duty {DutyId}", id);
                throw;
            }
        }

        public async Task<Duty> AddDutyAsync(Duty duty)
        {
            try
            {
                duty.UserId = await _userService.GetCurrentUserIdAsync();
                duty.CreatedAt = DateTime.UtcNow;

                await ValidateDutyAsync(duty);
                await _repository.AddAsync(duty);
                return duty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding duty");
                throw;
            }
        }

        public async Task<bool> UpdateDutyAsync(Duty duty)
        {
            try
            {
                var existingDuty = await _repository.GetByIdAsync(duty.Id);
                if (existingDuty == null)
                {
                    throw new NotFoundException("Duty not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (existingDuty.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot update other user's duty");
                }

                duty.ModifiedAt = DateTime.UtcNow;
                await ValidateDutyAsync(duty, duty.Id);
                await _repository.UpdateAsync(duty);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duty {DutyId}", duty.Id);
                throw;
            }
        }

        public async Task<bool> DeleteDutyAsync(int id)
        {
            try
            {
                var duty = await _repository.GetByIdAsync(id);
                if (duty == null)
                {
                    throw new NotFoundException("Duty not found");
                }

                var currentUserId = await _userService.GetCurrentUserIdAsync();
                if (duty.UserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("Cannot delete other user's duty");
                }

                await _repository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting duty {DutyId}", id);
                throw;
            }
        }

        public async Task<DutyStatistics> GetDutyStatisticsAsync()
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                return await _repository.GetDutyStatisticsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting duty statistics");
                throw;
            }
        }

        private async Task ValidateDutyAsync(Duty duty, int? excludeDutyId = null)
        {
            if (string.IsNullOrWhiteSpace(duty.Location))
            {
                throw new ValidationException("Duty location is required");
            }

            if (duty.StartTime >= duty.EndTime)
            {
                throw new ValidationException("End time must be after start time");
            }

            var hasOverlap = await _repository.HasOverlappingDutyAsync(
                duty.UserId,
                duty.StartTime,
                duty.EndTime,
                excludeDutyId);

            if (hasOverlap)
            {
                throw new ValidationException("This duty overlaps with another duty");
            }

            // Sprawdź czy nie przekroczono limitu godzin w miesiącu
            var monthStart = new DateTime(duty.StartTime.Year, duty.StartTime.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthlyHours = await _repository.GetMonthlyHoursAsync(duty.UserId, monthStart, monthEnd);

            var newDutyHours = (decimal)(duty.EndTime - duty.StartTime).TotalHours;
            var totalMonthlyHours = monthlyHours.Values.Sum() + newDutyHours;

            if (totalMonthlyHours > 240) // Przykładowy limit miesięczny
            {
                throw new ValidationException("Monthly duty hours limit exceeded");
            }
        }
    }
}
