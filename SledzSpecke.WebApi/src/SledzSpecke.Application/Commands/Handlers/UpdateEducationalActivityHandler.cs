using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateEducationalActivityHandler : IResultCommandHandler<UpdateEducationalActivity>
{
    private readonly IEducationalActivityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEducationalActivityHandler(
        IEducationalActivityRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateEducationalActivity command, CancellationToken cancellationToken = default)
    {
        var activity = await _repository.GetByIdAsync(new EducationalActivityId(command.Id));
        if (activity is null)
        {
            return Result.Failure($"Educational activity with ID {command.Id} not found.");
        }

        // Parse activity type
        if (!Enum.TryParse<EducationalActivityType>(command.Type, true, out var activityType))
        {
            return Result.Failure($"Invalid activity type: {command.Type}");
        }

        try
        {
            activity.UpdateDetails(
                activityType,
                command.Title,
                command.Description,
                command.StartDate,
                command.EndDate);

            await _repository.UpdateAsync(activity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update educational activity: {ex.Message}");
        }
    }
}