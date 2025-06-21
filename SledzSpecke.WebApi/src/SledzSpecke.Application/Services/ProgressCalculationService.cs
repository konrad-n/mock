using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Services;

public class ProgressCalculationService : IProgressCalculationService
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly ISpecializationTemplateService _templateService;
    private readonly IModuleRepository _moduleRepository;

    // Weight constants as per MAUI app
    private const decimal InternshipWeight = 0.35m;
    private const decimal CourseWeight = 0.25m;
    private const decimal ProcedureWeight = 0.30m;
    private const decimal OtherWeight = 0.10m;

    public ProgressCalculationService(
        IInternshipRepository internshipRepository,
        IProcedureRepository procedureRepository,
        ICourseRepository courseRepository,
        IMedicalShiftRepository medicalShiftRepository,
        ISpecializationTemplateService templateService,
        IModuleRepository moduleRepository)
    {
        _internshipRepository = internshipRepository;
        _procedureRepository = procedureRepository;
        _courseRepository = courseRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _templateService = templateService;
        _moduleRepository = moduleRepository;
    }

    public async Task<decimal> CalculateOverallProgressAsync(int specializationId, int? moduleId = null)
    {
        var statistics = await CalculateFullStatisticsAsync(specializationId, moduleId);
        return (decimal)statistics.OverallProgress;
    }

    public async Task<SpecializationStatisticsDto> CalculateFullStatisticsAsync(int specializationId, int? moduleId = null)
    {
        var statistics = new SpecializationStatisticsDto();

        // Get module information if specified
        string moduleName = "All Modules";
        if (moduleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(new ModuleId(moduleId.Value));
            if (module != null)
            {
                moduleName = module.Name;
            }
        }

        // Calculate internship progress
        var internshipStats = await CalculateInternshipProgressAsync(specializationId, moduleId);
        statistics.InternshipProgress = internshipStats;

        // Calculate course progress
        var courseStats = await CalculateCourseProgressAsync(specializationId, moduleId);
        statistics.CourseProgress = courseStats;

        // Calculate procedure progress
        var procedureStats = await CalculateProcedureProgressAsync(specializationId, moduleId);
        statistics.ProcedureProgress = procedureStats;

        // Calculate medical shift progress
        var shiftStats = await CalculateMedicalShiftProgressAsync(specializationId, moduleId);
        statistics.MedicalShiftProgress = shiftStats;

        // Calculate overall weighted progress
        statistics.OverallProgress = (double)CalculateWeightedProgress(
            (decimal)internshipStats.PercentageComplete,
            (decimal)courseStats.PercentageComplete,
            (decimal)procedureStats.PercentageComplete,
            (decimal)shiftStats.PercentageComplete
        );

        statistics.CalculatedAt = DateTime.UtcNow;

        return statistics;
    }

    public decimal CalculateWeightedProgress(decimal internshipProgress, decimal courseProgress, decimal procedureProgress, decimal otherProgress)
    {
        var weightedProgress = 
            (internshipProgress * InternshipWeight) +
            (courseProgress * CourseWeight) +
            (procedureProgress * ProcedureWeight) +
            (otherProgress * OtherWeight);

        // Ensure progress is between 0 and 100
        return Math.Min(100m, Math.Max(0m, Math.Round(weightedProgress, 2)));
    }

    private async Task<InternshipProgressDto> CalculateInternshipProgressAsync(int specializationId, int? moduleId)
    {
        var internships = await _internshipRepository.GetBySpecializationIdAsync(new SpecializationId(specializationId));
        
        if (moduleId.HasValue)
        {
            internships = internships.Where(i => i.ModuleId == moduleId.Value);
        }

        var completed = internships.Count(i => i.IsCompleted && i.IsApproved);
        var total = internships.Count();

        // TODO: Get required internships from template service
        var required = 10; // Default value, should be from template

        return new InternshipProgressDto
        {
            Completed = completed,
            Total = Math.Max(total, required),
            PercentageComplete = required > 0 ? Math.Round((double)completed / required * 100, 2) : 0
        };
    }

    private async Task<CourseProgressDto> CalculateCourseProgressAsync(int specializationId, int? moduleId)
    {
        var courses = await _courseRepository.GetBySpecializationIdAsync(new SpecializationId(specializationId));
        
        if (moduleId.HasValue)
        {
            courses = courses.Where(c => c.ModuleId?.Value == moduleId.Value);
        }

        var completed = courses.Count(c => c.IsApproved);
        var total = courses.Count();

        // TODO: Get required courses from template service
        var required = 20; // Default value, should be from template

        return new CourseProgressDto
        {
            Completed = completed,
            Total = Math.Max(total, required),
            PercentageComplete = required > 0 ? Math.Round((double)completed / required * 100, 2) : 0
        };
    }

    private async Task<ProcedureProgressDto> CalculateProcedureProgressAsync(int specializationId, int? moduleId)
    {
        // Get procedures for specialization through internships
        var internships = await _internshipRepository.GetBySpecializationIdAsync(new SpecializationId(specializationId));
        
        if (moduleId.HasValue)
        {
            internships = internships.Where(i => i.ModuleId == moduleId.Value);
        }

        var procedures = new List<Core.Entities.ProcedureBase>();
        foreach (var internship in internships)
        {
            var internshipProcedures = await _procedureRepository.GetByInternshipIdAsync(internship.InternshipId);
            procedures.AddRange(internshipProcedures);
        }

        // Count procedures by type using IsCodeA and IsCodeB properties
        var completedTypeA = procedures.Count(p => p.Status == Core.ValueObjects.ProcedureStatus.Approved && p.IsCodeA);
        var completedTypeB = procedures.Count(p => p.Status == Core.ValueObjects.ProcedureStatus.Approved && p.IsCodeB);

        // TODO: Get required procedures from template service
        var requiredTypeA = 50; // Default value
        var requiredTypeB = 30; // Default value

        var totalCompleted = completedTypeA + completedTypeB;
        var totalRequired = requiredTypeA + requiredTypeB;

        return new ProcedureProgressDto
        {
            CompletedTypeA = completedTypeA,
            TotalTypeA = requiredTypeA,
            CompletedTypeB = completedTypeB,
            TotalTypeB = requiredTypeB,
            PercentageComplete = totalRequired > 0 ? Math.Round((double)totalCompleted / totalRequired * 100, 2) : 0
        };
    }

    private async Task<MedicalShiftProgressDto> CalculateMedicalShiftProgressAsync(int specializationId, int? moduleId)
    {
        // Get medical shifts for specialization through internships
        var internships = await _internshipRepository.GetBySpecializationIdAsync(new SpecializationId(specializationId));
        
        if (moduleId.HasValue)
        {
            internships = internships.Where(i => i.ModuleId == moduleId.Value);
        }

        var shifts = new List<Core.Entities.MedicalShift>();
        foreach (var internship in internships)
        {
            shifts.AddRange(internship.MedicalShifts);
        }
        
        // Calculate total hours including minutes
        var totalMinutes = shifts.Where(s => s.IsApproved).Sum(s => s.Hours * 60 + s.Minutes);
        var completedHours = totalMinutes / 60; // Convert to hours

        // TODO: Get required hours from template service
        var requiredHours = 160; // Default value, should be from template

        return new MedicalShiftProgressDto
        {
            CompletedHours = completedHours,
            RequiredHours = requiredHours,
            PercentageComplete = requiredHours > 0 ? Math.Round((double)completedHours / requiredHours * 100, 2) : 0
        };
    }
}