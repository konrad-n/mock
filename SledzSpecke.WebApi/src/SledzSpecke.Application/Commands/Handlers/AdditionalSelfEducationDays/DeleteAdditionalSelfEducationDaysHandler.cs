using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Commands.Handlers.AdditionalSelfEducationDays;

public sealed class DeleteAdditionalSelfEducationDaysHandler : IResultCommandHandler<DeleteAdditionalSelfEducationDays>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAdditionalSelfEducationDaysHandler> _logger;

    public DeleteAdditionalSelfEducationDaysHandler(
        IAdditionalSelfEducationDaysRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAdditionalSelfEducationDaysHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(DeleteAdditionalSelfEducationDays command)
    {
        try
        {
            // Get the existing record
            var existingDays = await _repository.GetByIdAsync(command.Id);
            if (existingDays == null)
            {
                return Result.Failure("Additional self-education days record not found", "NOT_FOUND");
            }

            // Check if already approved
            if (existingDays.IsApproved)
            {
                return Result.Failure("Cannot delete approved self-education days", "ALREADY_APPROVED");
            }

            // Delete from repository
            await _repository.DeleteAsync(existingDays);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Deleted additional self-education days {Id} for module {ModuleId}",
                command.Id, existingDays.ModuleId.Value);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting additional self-education days {Id}", command.Id);
            return Result.Failure("An error occurred while deleting additional self-education days", "INTERNAL_ERROR");
        }
    }
}