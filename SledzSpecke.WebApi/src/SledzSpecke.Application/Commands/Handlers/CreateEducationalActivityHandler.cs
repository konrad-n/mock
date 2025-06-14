using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateEducationalActivityHandler : IResultCommandHandler<CreateEducationalActivity, int>
{
    private readonly IEducationalActivityRepository _repository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEducationalActivityHandler(
        IEducationalActivityRepository repository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(CreateEducationalActivity command)
    {
        // Validate specialization exists
        var specialization = await _specializationRepository.GetByIdAsync(command.SpecializationId);
        if (specialization is null)
        {
            return Result.Failure<int>($"Specialization with ID {command.SpecializationId} not found.");
        }

        // Validate module exists if provided
        if (command.ModuleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(command.ModuleId.Value);
            if (module is null)
            {
                return Result.Failure<int>($"Module with ID {command.ModuleId} not found.");
            }

            // Ensure module belongs to the specialization
            if (module.SpecializationId != new SpecializationId(command.SpecializationId))
            {
                return Result.Failure<int>("Module does not belong to the specified specialization.");
            }
        }

        // Parse activity type
        if (!Enum.TryParse<EducationalActivityType>(command.Type, true, out var activityType))
        {
            return Result.Failure<int>($"Invalid activity type: {command.Type}");
        }

        try
        {
            // Create the educational activity
            var activity = EducationalActivity.Create(
                new EducationalActivityId(0), // Will be assigned by database
                new SpecializationId(command.SpecializationId),
                activityType,
                command.Title,
                command.StartDate,
                command.EndDate);

            if (command.ModuleId.HasValue)
            {
                activity.AssignToModule(new ModuleId(command.ModuleId.Value));
            }

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                activity.SetDescription(command.Description);
            }

            await _repository.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(activity.Id.Value);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            return Result.Failure<int>($"Failed to create educational activity: {ex.Message}");
        }
    }
}