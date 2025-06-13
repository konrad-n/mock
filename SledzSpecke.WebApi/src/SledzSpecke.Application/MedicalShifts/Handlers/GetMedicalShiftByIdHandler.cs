using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class GetMedicalShiftByIdHandler : IQueryHandler<GetMedicalShiftById, MedicalShiftDto>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;

    public GetMedicalShiftByIdHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        IUserRepository userRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
    }

    public async Task<MedicalShiftDto> HandleAsync(GetMedicalShiftById query)
    {
        var userId = _userContextService.GetUserId();
        var shift = await _medicalShiftRepository.GetByIdAsync(query.ShiftId);

        if (shift == null)
        {
            throw new NotFoundException($"Medical shift with ID {query.ShiftId} not found.");
        }

        // Get internship to check ownership
        var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
        if (internship == null)
        {
            throw new NotFoundException($"Internship with ID {shift.InternshipId} not found.");
        }
        
        // Get user to verify ownership through specialization
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null || user.SpecializationId != internship.SpecializationId)
        {
            throw new UnauthorizedException("You are not authorized to view this medical shift.");
        }

        var canBeDeleted = !shift.IsApproved && shift.SyncStatus != SyncStatus.Synced;

        return new MedicalShiftDto(
            shift.Id,
            shift.InternshipId,
            shift.Date,
            shift.Hours,
            shift.Minutes,
            shift.Location,
            shift.Year,
            shift.SyncStatus,
            shift.AdditionalFields,
            shift.ApprovalDate,
            shift.ApproverName,
            shift.ApproverRole,
            shift.IsApproved,
            canBeDeleted,
            new TimeSpan(shift.Hours, shift.Minutes, 0)
        );
    }
}