using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.Procedures.Handlers;

public class UpdateProcedureResultHandler : IResultCommandHandler<UpdateProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProcedureResultHandler(
        IProcedureRepository procedureRepository, 
        IUserContextService userContextService,
        IUnitOfWork unitOfWork)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateProcedure command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userContextService.GetUserId();
            var procedure = await _procedureRepository.GetByIdAsync(new ProcedureId(command.ProcedureId));

            if (procedure is null)
            {
                return Result.Failure($"Procedure with ID {command.ProcedureId} not found.", "PROCEDURE_NOT_FOUND");
            }

            // Verify the procedure belongs to the user's internship
            var userInternships = await _procedureRepository.GetUserInternshipIdsAsync(userId);
            if (!userInternships.Contains(procedure.InternshipId))
            {
                return Result.Failure("Cannot update procedure from another user's internship", "UNAUTHORIZED");
            }

            // Check if procedure can be modified (e.g., not completed/synced)
            if (!procedure.CanBeModified)
            {
                return Result.Failure("This procedure cannot be modified", "PROCEDURE_LOCKED");
            }

            // Update procedure fields if provided
            if (!string.IsNullOrEmpty(command.Status))
            {
                if (Enum.TryParse<ProcedureStatus>(command.Status, out var status))
                {
                    procedure.ChangeStatus(status);
                }
                else
                {
                    return Result.Failure($"Invalid status: {command.Status}", "INVALID_STATUS");
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
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (ProcedureNotFoundException ex)
        {
            return Result.Failure(ex.Message, "PROCEDURE_NOT_FOUND");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result.Failure(ex.Message, "UNAUTHORIZED");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message, "INVALID_OPERATION");
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while updating the procedure: {ex.Message}", "UPDATE_FAILED");
        }
    }
}