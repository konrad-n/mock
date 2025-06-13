using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserProcedures(
    int UserId, 
    int? InternshipId = null,
    string? Status = null,
    DateTime? StartDate = null, 
    DateTime? EndDate = null
) : IQuery<IEnumerable<ProcedureDto>>;