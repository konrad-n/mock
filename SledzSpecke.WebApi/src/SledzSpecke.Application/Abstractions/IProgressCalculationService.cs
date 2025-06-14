using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Abstractions;

public interface IProgressCalculationService
{
    Task<decimal> CalculateOverallProgressAsync(int specializationId, int? moduleId = null);
    Task<SpecializationStatisticsDto> CalculateFullStatisticsAsync(int specializationId, int? moduleId = null);
    decimal CalculateWeightedProgress(decimal internshipProgress, decimal courseProgress, decimal procedureProgress, decimal otherProgress);
}