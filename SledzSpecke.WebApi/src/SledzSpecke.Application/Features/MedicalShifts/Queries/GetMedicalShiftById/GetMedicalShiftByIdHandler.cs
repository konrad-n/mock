using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Constants;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftById;

public sealed class GetMedicalShiftByIdHandler : IResultQueryHandler<GetMedicalShiftById, MedicalShiftDto>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;

    public GetMedicalShiftByIdHandler(IMedicalShiftRepository medicalShiftRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
    }

    public async Task<Result<MedicalShiftDto>> HandleAsync(
        GetMedicalShiftById query, 
        CancellationToken cancellationToken = default)
    {
        var shift = await _medicalShiftRepository.GetByIdAsync(query.Id);
            
        if (shift is null)
        {
            return Result<MedicalShiftDto>.Failure(
                $"Medical shift with ID {query.Id} not found",
                Core.Constants.ErrorCodes.SHIFT_NOT_FOUND);
        }

        var dto = new MedicalShiftDto(
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
        );

        return Result<MedicalShiftDto>.Success(dto);
    }
}