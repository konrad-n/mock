using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetInternshipByIdHandler : IQueryHandler<GetInternshipById, InternshipDto>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public GetInternshipByIdHandler(
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        IUserContextService userContextService)
    {
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _userContextService = userContextService;
    }

    public async Task<InternshipDto> HandleAsync(GetInternshipById query)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(query.InternshipId));
        if (internship is null)
        {
            throw new InternshipNotFoundException(query.InternshipId);
        }

        // Check if the internship belongs to the current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        // TODO: User-Specialization relationship needs to be redesigned
        // if (user is null || user.SpecializationId != internship.SpecializationId)
        // {
        //     throw new UnauthorizedAccessException("You can only view your own internships.");
        // }
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        return new InternshipDto
        {
            Id = internship.InternshipId.Value,
            SpecializationId = internship.SpecializationId.Value,
            ModuleId = internship.ModuleId?.Value,
            InstitutionName = internship.InstitutionName,
            DepartmentName = internship.DepartmentName,
            SupervisorName = internship.SupervisorName,
            StartDate = internship.StartDate,
            EndDate = internship.EndDate,
            DaysCount = internship.DaysCount,
            IsCompleted = internship.IsCompleted,
            IsApproved = internship.IsApproved,
            ApprovalDate = internship.ApprovalDate,
            ApproverName = internship.ApproverName,
            CreatedAt = internship.CreatedAt,
            UpdatedAt = internship.UpdatedAt,
            SyncStatus = internship.SyncStatus.ToString(),
            TotalShiftHours = internship.GetTotalShiftHours(),
            ApprovedProceduresCount = internship.GetApprovedProceduresCount()
        };
    }
}