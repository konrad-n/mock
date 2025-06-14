using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Application.Procedures.Extensions;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Handlers;

public class GetProcedureByIdHandler : IQueryHandler<GetProcedureById, ProcedureDto>
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IUserContextService _userContextService;

    public GetProcedureByIdHandler(IProcedureRepository procedureRepository, IUserContextService userContextService)
    {
        _procedureRepository = procedureRepository;
        _userContextService = userContextService;
    }

    public async Task<ProcedureDto> HandleAsync(GetProcedureById query)
    {
        var userId = _userContextService.GetUserId();
        var procedure = await _procedureRepository.GetByIdAsync(new ProcedureId(query.ProcedureId));

        if (procedure is null)
        {
            throw new ProcedureNotFoundException(query.ProcedureId);
        }

        // Verify the procedure belongs to the user's internship
        var userInternships = await _procedureRepository.GetUserInternshipIdsAsync(userId);
        if (!userInternships.Contains(procedure.InternshipId))
        {
            throw new UnauthorizedAccessException("Cannot access procedure from another user's internship");
        }

        return procedure.ToDto();
    }
}