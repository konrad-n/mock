using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public sealed class GetUserMedicalShiftsHandler : IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public GetUserMedicalShiftsHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<IEnumerable<MedicalShiftDto>> HandleAsync(GetUserMedicalShifts query)
    {
        // Get user to determine SMK version
        var userId = new UserId(query.UserId);
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return Enumerable.Empty<MedicalShiftDto>();
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // // Get user's specialization to check SMK version
        // var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        //
        // if (specialization == null)
        // {
        //     return Enumerable.Empty<MedicalShiftDto>();
        // }
        // For now, return empty list as we cannot access user's specialization
        return Enumerable.Empty<MedicalShiftDto>();

        // Get medical shifts based on filters
        IEnumerable<MedicalShift> medicalShifts;

        // If specific internship ID is provided, get shifts for that internship only
        if (query.InternshipId.HasValue)
        {
            medicalShifts = await _medicalShiftRepository.GetByInternshipIdAsync(query.InternshipId.Value);

            // Apply date range filter if provided
            if (query.StartDate.HasValue && query.EndDate.HasValue)
            {
                medicalShifts = medicalShifts.Where(s =>
                    s.Date >= query.StartDate.Value &&
                    s.Date <= query.EndDate.Value);
            }
        }
        // Otherwise, get shifts by date range or all user shifts
        else if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            medicalShifts = await _medicalShiftRepository.GetByDateRangeAsync(
                query.StartDate.Value,
                query.EndDate.Value,
                query.UserId);
        }
        else
        {
            medicalShifts = await _medicalShiftRepository.GetByUserIdAsync(query.UserId);
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // // Get all internships for the user's specialization
        // var internships = await _internshipRepository.GetByUserAndSpecializationAsync(userId, user.SpecializationId);
        var internships = Enumerable.Empty<Internship>();
        var internshipDict = internships.ToDictionary(i => i.InternshipId.Value);

        // Filter shifts based on SMK version and valid internships
        var filteredShifts = medicalShifts.Where(shift =>
        {
            // Ensure the shift belongs to a valid internship for this user
            if (!internshipDict.TryGetValue(shift.InternshipId.Value, out var internship))
                return false;

            // The MedicalShift entity supports both Old and New SMK shifts:
            // - Old SMK: Uses Year and Location fields (already in the base entity)
            // - New SMK: Uses AdditionalFields to store extra data like ModuleId, EndDate, etc.
            // 
            // The specialization's SMK version determines which shifts are relevant for the user
            // All shifts from valid internships are included since the internships themselves
            // are already filtered by the user's specialization
            return true;
        });

        // Map to DTOs and order by date descending
        return filteredShifts
            .OrderByDescending(s => s.Date)
            .Select(shift => MapToDto(shift))
            .ToList();
    }

    private static MedicalShiftDto MapToDto(MedicalShift shift)
    {
        return new MedicalShiftDto(
            Id: shift.Id.Value,
            InternshipId: shift.InternshipId.Value,
            Date: shift.Date,
            Hours: shift.Hours,
            Minutes: shift.Minutes,
            Location: shift.Location,
            Year: shift.Year,
            SyncStatus: shift.SyncStatus,
            AdditionalFields: shift.AdditionalFields,
            ApprovalDate: shift.ApprovalDate,
            ApproverName: shift.ApproverName,
            ApproverRole: shift.ApproverRole,
            IsApproved: shift.IsApproved,
            CanBeDeleted: shift.CanBeDeleted,
            Duration: TimeSpan.FromMinutes(shift.Duration.TotalMinutes)
        );
    }
}