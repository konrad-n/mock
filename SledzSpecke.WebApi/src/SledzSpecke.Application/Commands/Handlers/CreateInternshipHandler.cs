using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateInternshipHandler : ICommandHandler<CreateInternship, int>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;

    public CreateInternshipHandler(
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository)
    {
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<int> HandleAsync(CreateInternship command)
    {
        var specialization = await _specializationRepository.GetByIdAsync(command.SpecializationId);
        if (specialization is null)
        {
            throw new InvalidOperationException($"Specialization with ID {command.SpecializationId} not found.");
        }

        if (command.ModuleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(command.ModuleId.Value);
            if (module is null)
            {
                throw new InvalidOperationException($"Module with ID {command.ModuleId.Value} not found.");
            }
        }

        var internshipId = InternshipId.New();
        var internship = Internship.Create(
            internshipId,
            command.SpecializationId,
            command.Name,
            command.InstitutionName,
            command.DepartmentName,
            command.StartDate,
            command.EndDate,
            command.PlannedWeeks,
            command.PlannedDays);

        if (!string.IsNullOrWhiteSpace(command.SupervisorName))
        {
            internship.SetSupervisor(command.SupervisorName);
        }

        if (command.ModuleId.HasValue)
        {
            internship.AssignToModule(command.ModuleId.Value);
        }

        await _internshipRepository.AddAsync(internship);
        return internship.InternshipId.Value;
    }
}