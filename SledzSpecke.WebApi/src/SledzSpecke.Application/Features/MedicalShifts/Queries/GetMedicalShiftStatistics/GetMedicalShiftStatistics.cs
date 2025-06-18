using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;

namespace SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftStatistics;

public class GetMedicalShiftStatistics : IQuery<MedicalShiftStatisticsDto>
{
    public int? UserId { get; set; }
    public int Year { get; set; }
    public int? Month { get; set; }
}