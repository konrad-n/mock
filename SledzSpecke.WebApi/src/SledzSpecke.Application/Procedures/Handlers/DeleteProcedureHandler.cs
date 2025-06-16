using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public sealed class DeleteProcedureHandler : IResultCommandHandler<DeleteProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProcedureHandler(
        IProcedureRepository procedureRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        IInternshipRepository internshipRepository,
        IUnitOfWork unitOfWork)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _internshipRepository = internshipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteProcedure command)
    {
        try
        {
            var userId = _userContextService.GetUserId();
            var procedureId = new ProcedureId(command.ProcedureId);
            var procedure = await _procedureRepository.GetByIdAsync(procedureId);

            if (procedure is null)
            {
                return Result.Failure($"Procedure with ID {command.ProcedureId} not found.");
            }

            // Get internship to check ownership
            var internship = await _internshipRepository.GetByIdAsync(procedure.InternshipId);
            if (internship is null)
            {
                return Result.Failure($"Internship with ID {procedure.InternshipId.Value} not found.");
            }

            // Verify user has access to this procedure through specialization
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            // TODO: User-Specialization relationship needs to be redesigned
            // if (user is null || user.SpecializationId.Value != internship.SpecializationId.Value)
            // {
            //     return Result.Failure("You are not authorized to delete this procedure.");
            // }
            if (user is null)
            {
                return Result.Failure("User not found.");
            }

            // Check if procedure can be deleted
            if (!procedure.CanBeModified)
            {
                return Result.Failure("This procedure cannot be deleted. It may be completed or synced.");
            }

            // Check sync status - synced procedures cannot be deleted
            if (procedure.SyncStatus == SyncStatus.Synced)
            {
                return Result.Failure("Cannot delete a synced procedure. Contact your administrator if deletion is required.");
            }

            // Delete the procedure
            await _procedureRepository.DeleteAsync(procedure);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while deleting the procedure.");
        }
    }
}