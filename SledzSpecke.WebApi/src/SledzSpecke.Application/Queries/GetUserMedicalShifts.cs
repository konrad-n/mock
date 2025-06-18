using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;

namespace SledzSpecke.Application.Queries;

public record GetUserMedicalShifts(
    int UserId,
    int? InternshipId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IQuery<IEnumerable<MedicalShiftDto>>;