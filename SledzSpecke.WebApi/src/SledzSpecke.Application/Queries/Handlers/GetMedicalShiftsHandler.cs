using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Constants;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

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
        var user = await _userRepository.GetByIdAsync(new UserId(query.UserId), cancellationToken);
        if (user is null)
        {
            return Result<IEnumerable<MedicalShiftDto>>.Failure(
                $"User with ID {query.UserId} not found",
                ErrorCodes.USER_NOT_FOUND);
        }

        // Get medical shifts
        var shifts = await _medicalShiftRepository.GetByUserIdAsync(
            new UserId(query.UserId),
            query.FromDate,
            query.ToDate,
            query.InternshipId.HasValue ? new InternshipId(query.InternshipId.Value) : null,
            cancellationToken);

        // Map to DTOs
        var shiftDtos = shifts.Select(shift => new MedicalShiftDto(
            Id: shift.Id.Value,
            InternshipId: shift.InternshipId.Value,
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
            CanBeDeleted: !shift.IsApproved && shift.SyncStatus != SyncStatus.Synced,
            Duration: TimeSpan.FromHours(shift.Hours) + TimeSpan.FromMinutes(shift.Minutes)
        ));

        return Result<IEnumerable<MedicalShiftDto>>.Success(shiftDtos);
    }
}