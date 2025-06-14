using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public class DeleteProcedureHandler : ICommandHandler<DeleteProcedure>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;

    public DeleteProcedureHandler(IProcedureRepository procedureRepository, IUserContextService userContextService)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeleteProcedure command)
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
            throw new UnauthorizedAccessException("Cannot delete procedure from another user's internship");
        }

        // Check if procedure can be deleted (e.g., not synced)
        if (!procedure.CanBeModified)
        {
            throw new InvalidOperationException("This procedure cannot be deleted");
        }

        await _procedureRepository.DeleteAsync(procedure);
    }
}