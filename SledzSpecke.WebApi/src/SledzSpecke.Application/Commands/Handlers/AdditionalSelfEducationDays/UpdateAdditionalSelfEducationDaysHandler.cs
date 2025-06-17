using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Commands.Handlers.AdditionalSelfEducationDays;

public sealed class UpdateAdditionalSelfEducationDaysHandler : IResultCommandHandler<UpdateAdditionalSelfEducationDays>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAdditionalSelfEducationDaysHandler> _logger;

    public UpdateAdditionalSelfEducationDaysHandler(
        IAdditionalSelfEducationDaysRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAdditionalSelfEducationDaysHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateAdditionalSelfEducationDays command)
    {
        try
        {
            // Validate input
            if (command.DaysUsed <= 0)
            {
                return Result.Failure("Days used must be greater than 0", "INVALID_DAYS");
            }

            if (command.DaysUsed > 6)
            {
                return Result.Failure("Maximum 6 additional self-education days allowed per year", "EXCEEDED_DAYS_LIMIT");
            }

            // Get the existing record
            var existingDays = await _repository.GetByIdAsync(command.Id);
            if (existingDays == null)
            {
                return Result.Failure("Additional self-education days record not found", "NOT_FOUND");
            }

            // Check if already approved
            if (existingDays.IsApproved)
            {
                return Result.Failure("Cannot modify approved self-education days", "ALREADY_APPROVED");
            }

            // Calculate new dates based on the days used
            var newEndDate = existingDays.StartDate.AddDays(command.DaysUsed - 1);

            // Update dates
            var updateDatesResult = existingDays.UpdateDates(existingDays.StartDate, newEndDate);
            if (!updateDatesResult.IsSuccess)
            {
                return Result.Failure(updateDatesResult.Error, updateDatesResult.ErrorCode);
            }

            // Update details with new comment if provided
            if (!string.IsNullOrWhiteSpace(command.Comment))
            {
                var updateDetailsResult = existingDays.UpdateDetails(
                    command.Comment,
                    existingDays.EventName,
                    existingDays.Location,
                    existingDays.Organizer);

                if (!updateDetailsResult.IsSuccess)
                {
                    return Result.Failure(updateDetailsResult.Error, updateDetailsResult.ErrorCode);
                }
            }

            // Check total days in year after update
            var year = existingDays.StartDate.Year;
            var totalDaysInYear = await _repository.GetTotalDaysInYearAsync(
                existingDays.ModuleId.Value, 
                year);

            // Subtract the current days and add the new days to check the total
            var projectedTotal = totalDaysInYear - existingDays.NumberOfDays + command.DaysUsed;
            if (projectedTotal > 6)
            {
                return Result.Failure(
                    $"Cannot update to {command.DaysUsed} days. Total would be {projectedTotal} days for the year. Maximum is 6 days per year.",
                    "EXCEEDED_YEARLY_LIMIT");
            }

            // Update in repository
            await _repository.UpdateAsync(existingDays);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Updated additional self-education days {Id} to {DaysUsed} days",
                command.Id, command.DaysUsed);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating additional self-education days {Id}", command.Id);
            return Result.Failure("An error occurred while updating additional self-education days", "INTERNAL_ERROR");
        }
    }
}