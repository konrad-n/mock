using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public sealed record GetEducationalActivityById(int Id) : IQuery<EducationalActivityDto?>;