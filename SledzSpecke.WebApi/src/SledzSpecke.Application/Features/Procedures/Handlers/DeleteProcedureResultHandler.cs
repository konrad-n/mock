using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.Procedures.Handlers;

public class DeleteProcedureResultHandler : IResultCommandHandler<DeleteProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProcedureResultHandler(
        IProcedureRepository procedureRepository,
        IUserContextService userContextService,
        IUnitOfWork unitOfWork)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteProcedure command, CancellationToken cancellationToken = default)
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
                return Result.Failure("Cannot delete procedure from another user's internship", "UNAUTHORIZED");
            }

            // Check if procedure can be deleted (e.g., not synced with SMK)
            if (!procedure.CanBeModified)
            {
                return Result.Failure("This procedure cannot be deleted because it has been synced with SMK", "PROCEDURE_LOCKED");
            }

            // Hard delete the procedure (remove from database)
            await _procedureRepository.DeleteAsync(procedure);
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
            return Result.Failure($"An error occurred while deleting the procedure: {ex.Message}", "DELETE_FAILED");
        }
    }
}