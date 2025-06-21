using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserMedicalShiftsHandlerSimple : IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>>
{
    private readonly ILogger<GetUserMedicalShiftsHandlerSimple> _logger;

    public GetUserMedicalShiftsHandlerSimple(ILogger<GetUserMedicalShiftsHandlerSimple> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<MedicalShiftDto>> HandleAsync(GetUserMedicalShifts query)
    {
        _logger.LogInformation("Getting medical shifts for user {UserId}", query.UserId);
        
        // Return some sample data for testing
        var sampleShifts = new List<MedicalShiftDto>
        {
            new MedicalShiftDto(
                Id: 1,
                InternshipId: 14,
                Date: DateTime.UtcNow.AddDays(-5),
                Hours: 8,
                Minutes: 30,
                Location: "Emergency Department",
                Year: 1,
                SyncStatus: Core.Enums.SyncStatus.Unsynced,
                AdditionalFields: null,
                ApprovalDate: DateTime.UtcNow.AddDays(-4),
                ApproverName: "Dr. Smith",
                ApproverRole: "Supervisor",
                IsApproved: true,
                CanBeDeleted: false,
                Duration: new TimeSpan(8, 30, 0)
            ),
            new MedicalShiftDto(
                Id: 2,
                InternshipId: 14,
                Date: DateTime.UtcNow.AddDays(-3),
                Hours: 12,
                Minutes: 0,
                Location: "ICU",
                Year: 1,
                SyncStatus: Core.Enums.SyncStatus.Unsynced,
                AdditionalFields: null,
                ApprovalDate: null,
                ApproverName: null,
                ApproverRole: null,
                IsApproved: false,
                CanBeDeleted: true,
                Duration: new TimeSpan(12, 0, 0)
            ),
            new MedicalShiftDto(
                Id: 3,
                InternshipId: 14,
                Date: DateTime.UtcNow.AddDays(-1),
                Hours: 6,
                Minutes: 15,
                Location: "Surgery Department",
                Year: 1,
                SyncStatus: Core.Enums.SyncStatus.Unsynced,
                AdditionalFields: null,
                ApprovalDate: null,
                ApproverName: null,
                ApproverRole: null,
                IsApproved: false,
                CanBeDeleted: true,
                Duration: new TimeSpan(6, 15, 0)
            )
        };

        // Apply filters if provided
        if (query.StartDate.HasValue)
        {
            sampleShifts = sampleShifts.Where(s => s.Date >= query.StartDate.Value).ToList();
        }
        
        if (query.EndDate.HasValue)
        {
            sampleShifts = sampleShifts.Where(s => s.Date <= query.EndDate.Value).ToList();
        }

        await Task.CompletedTask; // Make it async
        return sampleShifts.OrderByDescending(s => s.Date);
    }
}