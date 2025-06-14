using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserMedicalShifts(
    int UserId,
    int? InternshipId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IQuery<IEnumerable<MedicalShiftDto>>;