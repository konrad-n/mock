using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetAdditionalSelfEducationDaysById(int Id) : IQuery<AdditionalSelfEducationDaysDto>;