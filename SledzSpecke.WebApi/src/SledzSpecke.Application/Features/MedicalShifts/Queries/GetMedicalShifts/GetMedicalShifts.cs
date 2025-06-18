using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShifts;

public class GetMedicalShifts : IQuery<IEnumerable<MedicalShiftDto>>
{
    public int UserId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? InternshipId { get; set; }
}