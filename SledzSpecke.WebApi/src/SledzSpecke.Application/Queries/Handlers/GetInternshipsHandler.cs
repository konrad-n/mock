using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetInternshipsHandler : IQueryHandler<GetInternships, IEnumerable<InternshipDto>>
{
    private readonly IInternshipRepository _internshipRepository;

    public GetInternshipsHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task<IEnumerable<InternshipDto>> HandleAsync(GetInternships query)
    {
        var internships = query.ModuleId.HasValue
            ? await _internshipRepository.GetByModuleIdAsync(query.ModuleId.Value)
            : await _internshipRepository.GetBySpecializationIdAsync(query.SpecializationId);

        return internships.Select(i => new InternshipDto
        {
            Id = i.Id,
            SpecializationId = i.SpecializationId,
            ModuleId = i.ModuleId?.Value,
            InstitutionName = i.InstitutionName,
            DepartmentName = i.DepartmentName,
            SupervisorName = i.SupervisorName,
            StartDate = i.StartDate,
            EndDate = i.EndDate,
            DaysCount = i.DaysCount,
            IsCompleted = i.IsCompleted,
            IsApproved = i.IsApproved,
            ApprovalDate = i.ApprovalDate,
            ApproverName = i.ApproverName,
            SyncStatus = i.SyncStatus.ToString(),
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            TotalShiftHours = i.GetTotalShiftHours(),
            ApprovedProceduresCount = i.GetApprovedProceduresCount()
        });
    }
}