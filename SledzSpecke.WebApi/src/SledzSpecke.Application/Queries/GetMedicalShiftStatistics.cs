using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetMedicalShiftStatistics(int? Year = null, int? InternshipRequirementId = null)
    : IQuery<MedicalShiftSummaryDto>;