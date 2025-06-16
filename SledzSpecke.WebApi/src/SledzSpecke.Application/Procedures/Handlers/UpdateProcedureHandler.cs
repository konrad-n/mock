using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public class UpdateProcedureHandler : ICommandHandler<UpdateProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;

    public UpdateProcedureHandler(IProcedureRepository procedureRepository, IUserContextService userContextService)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdateProcedure command)
    {
        var userId = _userContextService.GetUserId();
        var procedure = await _procedureRepository.GetByIdAsync(new ProcedureId(command.ProcedureId));

        if (procedure is null)
        {
            throw new ProcedureNotFoundException(command.ProcedureId);
        }

        // Verify the procedure belongs to the user's internship
        var userInternships = await _procedureRepository.GetUserInternshipIdsAsync(userId);
        if (!userInternships.Contains(procedure.InternshipId))
        {
            throw new UnauthorizedAccessException("Cannot update procedure from another user's internship");
        }

        // Check if procedure can be modified (e.g., not completed/synced)
        if (!procedure.CanBeModified)
        {
            throw new InvalidOperationException("This procedure cannot be modified");
        }

        // Update procedure fields if provided
        if (!string.IsNullOrEmpty(command.Status))
        {
            if (Enum.TryParse<ProcedureStatus>(command.Status, out var status))
            {
                procedure.ChangeStatus(status);
            }
        }

        // Update procedure details
        var executionType = command.ExecutionType != null 
            ? Enum.Parse<ProcedureExecutionType>(command.ExecutionType) 
            : procedure.ExecutionType;
            
        procedure.UpdateProcedureDetails(
            executionType,
            command.PerformingPerson,
            command.PatientInfo,
            command.PatientInitials,
            command.PatientGender ?? procedure.PatientGender);

        // Note: Date, Code, and Location updates are not supported in the base model
        // These fields are typically immutable after creation

        await _procedureRepository.UpdateAsync(procedure);
    }
}