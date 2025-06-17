using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetAdditionalSelfEducationDaysByModule(int ModuleId) : IQuery<IEnumerable<AdditionalSelfEducationDaysDto>>;