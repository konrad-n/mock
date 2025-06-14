using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetSelfEducationByYear(int UserId, int Year) : IQuery<IEnumerable<SelfEducationDto>>;