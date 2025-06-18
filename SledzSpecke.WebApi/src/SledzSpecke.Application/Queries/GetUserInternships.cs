using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserInternships(int UserId, int? ModuleId = null, bool IncludeCompleted = true) 
    : IQuery<IEnumerable<InternshipDto>>;