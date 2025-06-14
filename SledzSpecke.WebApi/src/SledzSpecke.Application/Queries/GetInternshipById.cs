using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetInternshipById(int InternshipId) : IQuery<InternshipDto>;