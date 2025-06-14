using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeleteEducationalActivityHandler : IResultCommandHandler<DeleteEducationalActivity>
{
    private readonly IEducationalActivityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEducationalActivityHandler(
        IEducationalActivityRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteEducationalActivity command)
    {
        var activity = await _repository.GetByIdAsync(new EducationalActivityId(command.Id));
        if (activity is null)
        {
            return Result.Failure($"Educational activity with ID {command.Id} not found.");
        }

        if (!activity.CanBeDeleted())
        {
            return Result.Failure("Cannot delete synced educational activity.");
        }

        try
        {
            await _repository.DeleteAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete educational activity: {ex.Message}");
        }
    }
}