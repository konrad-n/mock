using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Specifications;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Commands.Handlers.AdditionalSelfEducationDays;

public sealed class AddAdditionalSelfEducationDaysHandler : ICommandHandler<AddAdditionalSelfEducationDays, int>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddAdditionalSelfEducationDaysHandler> _logger;

    public AddAdditionalSelfEducationDaysHandler(
        IAdditionalSelfEducationDaysRepository repository,
        IModuleRepository moduleRepository,
        IInternshipRepository internshipRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddAdditionalSelfEducationDaysHandler> logger)
    {
        _repository = repository;
        _moduleRepository = moduleRepository;
        _internshipRepository = internshipRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> HandleAsync(AddAdditionalSelfEducationDays command)
    {
        _logger.LogInformation("Adding additional self-education days for specialization {SpecializationId}", 
            command.SpecializationId);

        // Validate input
        if (command.DaysUsed <= 0)
        {
            throw new InvalidOperationException("Days used must be greater than 0");
        }

        if (command.DaysUsed > 6)
        {
            throw new InvalidOperationException("Maximum 6 additional self-education days allowed per year");
        }

        // Get the current active module for the specialization
        var modules = await _moduleRepository.GetBySpecializationIdAsync(command.SpecializationId);
        var activeModule = modules.FirstOrDefault(m => !m.IsCompleted());
        
        if (activeModule == null)
        {
            throw new InvalidOperationException("No active module found for specialization");
        }

        // Get an active internship from the module
        var internships = await _internshipRepository.GetByModuleIdAsync(activeModule.ModuleId);
        var activeInternship = internships.FirstOrDefault(i => i.Status == "InProgress");
        
        if (activeInternship == null)
        {
            throw new InvalidOperationException("No active internship found in the current module");
        }

        // Check the yearly limit
        var existingDays = await _repository.GetTotalDaysInYearAsync(
            activeModule.ModuleId, 
            command.Year);
            
        if (existingDays + command.DaysUsed > 6)
        {
            throw new InvalidOperationException(
                $"Cannot exceed 6 additional self-education days per year. Already used: {existingDays}");
        }

        // Create the entity
        var startDate = new DateTime(command.Year, 1, 1);
        var endDate = startDate.AddDays(command.DaysUsed - 1);
        
        var result = Core.Entities.AdditionalSelfEducationDays.Create(
            new ModuleId(activeModule.ModuleId),
            new InternshipId(activeInternship.InternshipId),
            startDate,
            endDate,
            command.Comment ?? "Additional self-education days",
            "Self-education"
        );

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _repository.AddAsync(result.Value);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Successfully added {Days} additional self-education days", command.DaysUsed);
        
        return result.Value.Id;
    }
}