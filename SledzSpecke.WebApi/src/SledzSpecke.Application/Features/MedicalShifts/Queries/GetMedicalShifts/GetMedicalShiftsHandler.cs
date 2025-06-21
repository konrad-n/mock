using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Constants;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShifts;

public sealed class GetMedicalShiftsHandler : IResultQueryHandler<GetMedicalShifts, IEnumerable<MedicalShiftDto>>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserRepository _userRepository;

    public GetMedicalShiftsHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserRepository userRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IEnumerable<MedicalShiftDto>>> HandleAsync(
        GetMedicalShifts query, 
        CancellationToken cancellationToken = default)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(query.UserId);
        if (user is null)
        {
            return Result<IEnumerable<MedicalShiftDto>>.Failure(
                $"User with ID {query.UserId} not found",
                Core.Constants.ErrorCodes.USER_NOT_FOUND);
        }

        // Get medical shifts
        IEnumerable<MedicalShift> shifts;
        if (query.FromDate != default && query.ToDate != default)
        {
            shifts = await _medicalShiftRepository.GetByUserIdAndDateRangeAsync(
                new Core.ValueObjects.UserId(query.UserId),
                query.FromDate,
                query.ToDate);
        }
        else
        {
            shifts = await _medicalShiftRepository.GetByUserIdAsync(query.UserId);
        }
        
        // Filter by internship if specified
        if (query.InternshipId.HasValue)
        {
            shifts = shifts.Where(s => s.InternshipId == query.InternshipId.Value);
        }

        // Map to DTOs
        var shiftDtos = shifts.Select(shift => new MedicalShiftDto(
            Id: shift.ShiftId,
            InternshipId: shift.InternshipId,
            Date: shift.Date,
            Hours: shift.Hours,
            Minutes: shift.Minutes,
            Location: shift.Location,
            Year: shift.Year,
            SyncStatus: shift.SyncStatus,
            AdditionalFields: null,
            ApprovalDate: shift.ApprovalDate,
            ApproverName: null,
            ApproverRole: null,
            IsApproved: shift.IsApproved,
            CanBeDeleted: !shift.IsApproved && shift.SyncStatus != Core.Enums.SyncStatus.Synced,
            Duration: TimeSpan.FromHours(shift.Hours) + TimeSpan.FromMinutes(shift.Minutes)
        ));

        return Result<IEnumerable<MedicalShiftDto>>.Success(shiftDtos);
    }
}