using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetInternshipProgress(int InternshipId) : IQuery<InternshipProgressDto>;

public record InternshipProgressDto(
    int InternshipId,
    string InternshipName,
    int TotalShifts,
    int TotalHours,
    int TotalMinutes,
    int ApprovedShifts,
    int TotalProcedures,
    int CompletedProcedures,
    double CompletionPercentage,
    DateTime StartDate,
    DateTime EndDate,
    bool IsCompleted);