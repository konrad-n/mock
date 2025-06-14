using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class GetMedicalShiftStatisticsHandler : IQueryHandler<GetMedicalShiftStatistics, MedicalShiftSummaryDto>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;
    private readonly ISpecializationRepository _specializationRepository;

    public GetMedicalShiftStatisticsHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserRepository userRepository,
        IUserContextService userContextService,
        ISpecializationRepository specializationRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userRepository = userRepository;
        _userContextService = userContextService;
        _specializationRepository = specializationRepository;
    }

    public async Task<MedicalShiftSummaryDto> HandleAsync(GetMedicalShiftStatistics query)
    {
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        if (specialization is null)
        {
            throw new InvalidOperationException("User specialization not found");
        }

        var summary = new MedicalShiftSummaryDto();
        IEnumerable<MedicalShift> shifts;

        if (specialization.SmkVersion == SmkVersion.Old)
        {
            // For Old SMK, filter by year
            if (query.Year.HasValue)
            {
                var allShifts = await _medicalShiftRepository.GetByUserAsync(new UserId(userId));
                shifts = allShifts.Where(s => s.Year == query.Year.Value);
            }
            else
            {
                shifts = await _medicalShiftRepository.GetByUserAsync(new UserId(userId));
            }
        }
        else
        {
            // For New SMK, filter by internship requirement ID
            if (query.InternshipRequirementId.HasValue)
            {
                // TODO: Need to implement filtering by internship requirement
                // For now, get all shifts for the user
                shifts = await _medicalShiftRepository.GetByUserAsync(new UserId(userId));
            }
            else
            {
                shifts = await _medicalShiftRepository.GetByUserAsync(new UserId(userId));
            }
        }

        // Calculate totals
        foreach (var shift in shifts)
        {
            summary.TotalHours += shift.Hours;
            summary.TotalMinutes += shift.Minutes;

            if (shift.SyncStatus == SyncStatus.Synced || shift.IsApproved)
            {
                summary.ApprovedHours += shift.Hours;
                summary.ApprovedMinutes += shift.Minutes;
            }
        }

        // Normalize time (convert excess minutes to hours)
        summary.NormalizeTime();

        return summary;
    }
}