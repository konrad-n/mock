using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetUserAbsencesHandler : IQueryHandler<GetUserAbsences, IEnumerable<AbsenceDto>>
{
    private readonly IAbsenceRepository _absenceRepository;

    public GetUserAbsencesHandler(IAbsenceRepository absenceRepository)
    {
        _absenceRepository = absenceRepository;
    }

    public async Task<IEnumerable<AbsenceDto>> HandleAsync(GetUserAbsences query)
    {
        var userId = new UserId(query.UserId);
        var absences = await _absenceRepository.GetByUserIdAsync(userId);

        if (query.SpecializationId.HasValue)
        {
            absences = absences.Where(a => a.SpecializationId.Value == query.SpecializationId.Value);
        }

        return absences
            .OrderByDescending(a => a.StartDate)
            .Select(a => new AbsenceDto
            {
                Id = a.Id.Value,
                SpecializationId = a.SpecializationId.Value,
                UserId = a.UserId.Value,
                Type = a.Type.ToString(),
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                DurationInDays = a.DurationInDays,
                Description = a.Description,
                DocumentPath = a.DocumentPath,
                IsApproved = a.IsApproved,
                ApprovedAt = a.ApprovedAt,
                ApprovedBy = a.ApprovedBy?.Value,
                ExtensionDays = a.CalculateSpecializationExtensionDays(),
                SyncStatus = a.SyncStatus.ToString(),
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            });
    }
}