using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserMedicalShiftsHandler : IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ILogger<GetUserMedicalShiftsHandler> _logger;

    public GetUserMedicalShiftsHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ILogger<GetUserMedicalShiftsHandler> logger)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicalShiftDto>> HandleAsync(GetUserMedicalShifts query)
    {
        try
        {
            _logger.LogInformation("Getting medical shifts for user {UserId}", query.UserId);
            
            // Get user's specializations (may have multiple)
            var specializations = await _specializationRepository.GetByUserIdAsync(new UserId(query.UserId));
            if (!specializations.Any())
            {
                _logger.LogWarning("No specialization found for user {UserId}", query.UserId);
                return Enumerable.Empty<MedicalShiftDto>();
            }

            // Get all internships for all user's specializations
            var internshipIds = new List<InternshipId>();
            foreach (var spec in specializations)
            {
                var internships = await _internshipRepository.GetBySpecializationIdAsync(spec.Id);
                internshipIds.AddRange(internships.Select(i => i.InternshipId));
            }

            if (query.InternshipId.HasValue)
            {
                // Filter to specific internship if provided
                internshipIds = internshipIds.Where(id => id.Value == query.InternshipId.Value).ToList();
            }

            var allShifts = new List<MedicalShiftDto>();

            foreach (var internshipId in internshipIds)
            {
                var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(internshipId);
                
                // Apply date filters if provided
                if (query.StartDate.HasValue)
                {
                    shifts = shifts.Where(s => s.Date >= query.StartDate.Value);
                }
                
                if (query.EndDate.HasValue)
                {
                    shifts = shifts.Where(s => s.Date <= query.EndDate.Value);
                }

                allShifts.AddRange(shifts.Select(shift => new MedicalShiftDto(
                    Id: shift.Id.Value,
                    InternshipId: shift.InternshipId.Value,
                    Date: shift.Date,
                    Hours: shift.Duration.Hours,
                    Minutes: shift.Duration.Minutes,
                    Location: shift.Location,
                    Year: shift.Year,
                    SyncStatus: shift.SyncStatus,
                    AdditionalFields: null,
                    ApprovalDate: shift.ApprovalDate,
                    ApproverName: shift.ApproverName,
                    ApproverRole: shift.ApproverRole,
                    IsApproved: shift.ApprovalDate.HasValue,
                    CanBeDeleted: true, // Business logic to determine this
                    Duration: new TimeSpan(shift.Duration.Hours, shift.Duration.Minutes, 0)
                )));
            }

            _logger.LogInformation("Found {Count} medical shifts for user {UserId}", allShifts.Count, query.UserId);
            return allShifts.OrderByDescending(s => s.Date);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting medical shifts for user {UserId}", query.UserId);
            throw;
        }
    }
}