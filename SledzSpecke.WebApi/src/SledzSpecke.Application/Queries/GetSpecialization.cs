using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetSpecialization(int SpecializationId) : IQuery<SpecializationDto>;