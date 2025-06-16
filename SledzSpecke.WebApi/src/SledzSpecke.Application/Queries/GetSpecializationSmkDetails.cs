using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetSpecializationSmkDetails(int SpecializationId) : IQuery<SpecializationSmkDto>;