using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class SpecializationProgressService : ISpecializationProgressService
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IAbsenceRepository _absenceRepository;
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IPublicationRepository _publicationRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public SpecializationProgressService(
        IInternshipRepository internshipRepository,
        ICourseRepository courseRepository,
        IProcedureRepository procedureRepository,
        IMedicalShiftRepository medicalShiftRepository,
        IAbsenceRepository absenceRepository,
        IRecognitionRepository recognitionRepository,
        ISelfEducationRepository selfEducationRepository,
        IPublicationRepository publicationRepository,
        ISpecializationRepository specializationRepository)
    {
        _internshipRepository = internshipRepository;
        _courseRepository = courseRepository;
        _procedureRepository = procedureRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _absenceRepository = absenceRepository;
        _recognitionRepository = recognitionRepository;
        _selfEducationRepository = selfEducationRepository;
        _publicationRepository = publicationRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<SpecializationProgressSummary> CalculateProgressAsync(UserId userId, SpecializationId specializationId)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization is null)
            throw new ArgumentException("Specialization not found.");

        var internships = await _internshipRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        var courses = await _courseRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        var procedures = await _procedureRepository.GetByUserAsync(userId); // Procedures are linked through internships
        var medicalShifts = await _medicalShiftRepository.GetByUserAsync(userId);

        var completedInternships = internships.Count(i => i.IsCompleted);
        var completedCourses = courses.Count(c => c.IsApproved);
        var completedProcedures = procedures.Count(p => p.IsCompleted);
        var totalMedicalShiftHours = medicalShifts.Sum(ms => ms.Hours);

        var daysExtended = await CalculateExtensionDaysAsync(userId, specializationId);
        var daysReduced = await CalculateReductionDaysAsync(userId, specializationId);
        var netAdjustment = daysExtended - daysReduced;

        var originalCompletionDate = specialization.StartDate.AddDays(specialization.DurationInDays);
        var estimatedCompletionDate = originalCompletionDate.AddDays(netAdjustment);

        var totalRequiredInternships = specialization.Modules.Sum(m => m.TotalInternships);
        var totalRequiredCourses = specialization.Modules.Sum(m => m.TotalCourses);
        var totalRequiredProcedures = specialization.Modules.Sum(m => m.TotalProceduresA + m.TotalProceduresB);
        var requiredMedicalShiftHours = specialization.Modules.Sum(m => m.RequiredShiftHours);

        var overallProgress = CalculateOverallProgress(
            completedInternships, totalRequiredInternships,
            completedCourses, totalRequiredCourses,
            completedProcedures, totalRequiredProcedures,
            totalMedicalShiftHours, requiredMedicalShiftHours);

        return new SpecializationProgressSummary
        {
            OverallProgressPercentage = overallProgress,
            CompletedInternships = completedInternships,
            TotalInternships = totalRequiredInternships,
            CompletedCourses = completedCourses,
            TotalCourses = totalRequiredCourses,
            CompletedProcedures = completedProcedures,
            TotalProcedures = totalRequiredProcedures,
            TotalMedicalShiftHours = totalMedicalShiftHours,
            RequiredMedicalShiftHours = requiredMedicalShiftHours,
            DaysExtended = daysExtended,
            DaysReduced = daysReduced,
            NetDurationAdjustment = netAdjustment,
            EstimatedCompletionDate = estimatedCompletionDate,
            OriginalCompletionDate = originalCompletionDate,
            IsOnTrack = overallProgress >= GetExpectedProgressForDate(specialization.StartDate, originalCompletionDate)
        };
    }

    public async Task<DateTime> EstimateCompletionDateAsync(UserId userId, SpecializationId specializationId)
    {
        var progress = await CalculateProgressAsync(userId, specializationId);
        return progress.EstimatedCompletionDate;
    }

    public async Task<int> CalculateAdjustedDurationDaysAsync(UserId userId, SpecializationId specializationId)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization is null)
            throw new ArgumentException("Specialization not found.");

        var extensionDays = await CalculateExtensionDaysAsync(userId, specializationId);
        var reductionDays = await CalculateReductionDaysAsync(userId, specializationId);

        return specialization.DurationInDays + extensionDays - reductionDays;
    }

    public async Task<WeightedProgressStatistics> CalculateWeightedProgressAsync(UserId userId, SpecializationId specializationId)
    {
        var basicProgress = await CalculateProgressAsync(userId, specializationId);
        var selfEducation = await _selfEducationRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        var publications = await _publicationRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        var absences = await _absenceRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        var recognitions = await _recognitionRepository.GetByUserAndSpecializationAsync(userId, specializationId);

        var internshipProgress = basicProgress.TotalInternships > 0 
            ? (double)basicProgress.CompletedInternships / basicProgress.TotalInternships 
            : 0;
        
        var courseProgress = basicProgress.TotalCourses > 0 
            ? (double)basicProgress.CompletedCourses / basicProgress.TotalCourses 
            : 0;
        
        var procedureProgress = basicProgress.TotalProcedures > 0 
            ? (double)basicProgress.CompletedProcedures / basicProgress.TotalProcedures 
            : 0;

        var selfEducationProgress = CalculateSelfEducationProgress(selfEducation);
        var qualityScore = CalculateQualityScore(publications, selfEducation, recognitions);

        var weightedProgress = new WeightedProgressStatistics
        {
            InternshipProgress = internshipProgress,
            CourseProgress = courseProgress,
            ProcedureProgress = procedureProgress,
            SelfEducationProgress = selfEducationProgress,
            QualityScore = qualityScore,
            TotalPublications = publications.Count(),
            TotalAbsences = absences.Count(),
            TotalRecognitions = recognitions.Count()
        };

        weightedProgress.WeightedProgressPercentage = 
            (internshipProgress * weightedProgress.InternshipWeight) +
            (courseProgress * weightedProgress.CourseWeight) +
            (procedureProgress * weightedProgress.ProcedureWeight) +
            (selfEducationProgress * weightedProgress.SelfEducationWeight);

        return weightedProgress;
    }

    public async Task<IEnumerable<ModuleProgressSummary>> GetModuleProgressAsync(UserId userId, SpecializationId specializationId)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization is null)
            throw new ArgumentException("Specialization not found.");

        var moduleProgress = new List<ModuleProgressSummary>();

        foreach (var module in specialization.Modules)
        {
            var moduleInternships = await _internshipRepository.GetByModuleAsync(module.Id);
            var moduleCourses = await _courseRepository.GetByModuleAsync(module.Id);
            
            var completedInternships = moduleInternships.Count(i => i.IsCompleted);
            var completedCourses = moduleCourses.Count(c => c.IsApproved);

            var progress = CalculateModuleProgress(
                completedInternships, module.TotalInternships,
                completedCourses, module.TotalCourses);

            var isCompleted = progress >= 100;

            moduleProgress.Add(new ModuleProgressSummary
            {
                ModuleId = module.Id,
                ModuleName = module.Name,
                ProgressPercentage = progress,
                CompletedInternships = completedInternships,
                RequiredInternships = module.TotalInternships,
                CompletedCourses = completedCourses,
                RequiredCourses = module.TotalCourses,
                IsCompleted = isCompleted,
                CompletedAt = isCompleted ? DateTime.UtcNow : null // This could be enhanced to track actual completion date
            });
        }

        return moduleProgress;
    }

    public async Task<CompletionProjection> ProjectCompletionAsync(UserId userId, SpecializationId specializationId)
    {
        var progress = await CalculateProgressAsync(userId, specializationId);
        var weightedProgress = await CalculateWeightedProgressAsync(userId, specializationId);

        var remainingDays = (int)(progress.EstimatedCompletionDate - DateTime.UtcNow).TotalDays;
        var completionProbability = CalculateCompletionProbability(progress, weightedProgress);
        
        var riskFactors = IdentifyRiskFactors(progress, weightedProgress);
        var recommendations = GenerateRecommendations(progress, weightedProgress, riskFactors);
        var requiresIntervention = completionProbability < 0.7 || remainingDays > progress.OriginalCompletionDate.Subtract(DateTime.UtcNow).TotalDays * 1.2;

        return new CompletionProjection
        {
            EstimatedCompletionDate = progress.EstimatedCompletionDate,
            RemainingDays = Math.Max(0, remainingDays),
            CompletionProbability = completionProbability,
            RiskFactors = riskFactors,
            Recommendations = recommendations,
            RequiresIntervention = requiresIntervention
        };
    }

    private async Task<int> CalculateExtensionDaysAsync(UserId userId, SpecializationId specializationId)
    {
        var absences = await _absenceRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        return absences.Where(a => a.IsApproved).Sum(a => a.CalculateSpecializationExtensionDays());
    }

    private async Task<int> CalculateReductionDaysAsync(UserId userId, SpecializationId specializationId)
    {
        var recognitions = await _recognitionRepository.GetByUserAndSpecializationAsync(userId, specializationId);
        return recognitions.Where(r => r.IsApproved).Sum(r => r.CalculateSpecializationReductionDays());
    }

    private static double CalculateOverallProgress(int completedInternships, int totalInternships,
        int completedCourses, int totalCourses, int completedProcedures, int totalProcedures,
        int completedHours, int requiredHours)
    {
        var internshipProgress = totalInternships > 0 ? (double)completedInternships / totalInternships : 1.0;
        var courseProgress = totalCourses > 0 ? (double)completedCourses / totalCourses : 1.0;
        var procedureProgress = totalProcedures > 0 ? (double)completedProcedures / totalProcedures : 1.0;
        var hoursProgress = requiredHours > 0 ? Math.Min(1.0, (double)completedHours / requiredHours) : 1.0;

        return (internshipProgress * 0.3 + courseProgress * 0.25 + procedureProgress * 0.25 + hoursProgress * 0.2) * 100;
    }

    private static double CalculateModuleProgress(int completedInternships, int requiredInternships,
        int completedCourses, int requiredCourses)
    {
        var internshipProgress = requiredInternships > 0 ? (double)completedInternships / requiredInternships : 1.0;
        var courseProgress = requiredCourses > 0 ? (double)completedCourses / requiredCourses : 1.0;

        return (internshipProgress * 0.6 + courseProgress * 0.4) * 100;
    }

    private static double CalculateSelfEducationProgress(IEnumerable<SelfEducation> selfEducation)
    {
        var completed = selfEducation.Count(se => se.IsCompleted);
        var total = selfEducation.Count();
        return total > 0 ? (double)completed / total : 0;
    }

    private static int CalculateQualityScore(IEnumerable<Publication> publications, 
        IEnumerable<SelfEducation> selfEducation, IEnumerable<Recognition> recognitions)
    {
        var publicationScore = publications.Sum(p => p.CalculateImpactScore());
        var selfEducationScore = selfEducation.Sum(se => se.CalculateQualityScore());
        var recognitionScore = recognitions.Count(r => r.IsApproved) * 5;

        return publicationScore + selfEducationScore + recognitionScore;
    }

    private static double GetExpectedProgressForDate(DateTime startDate, DateTime endDate)
    {
        var totalDays = (endDate - startDate).TotalDays;
        var elapsedDays = (DateTime.UtcNow - startDate).TotalDays;
        return Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100));
    }

    private static double CalculateCompletionProbability(SpecializationProgressSummary progress, 
        WeightedProgressStatistics weightedProgress)
    {
        var progressFactor = progress.OverallProgressPercentage / 100.0;
        var qualityFactor = Math.Min(1.0, weightedProgress.QualityScore / 50.0);
        var timeFactor = progress.IsOnTrack ? 1.0 : 0.8;
        var absenceFactor = weightedProgress.TotalAbsences > 3 ? 0.9 : 1.0;

        return Math.Min(1.0, progressFactor * 0.5 + qualityFactor * 0.2 + timeFactor * 0.2 + absenceFactor * 0.1);
    }

    private static IEnumerable<string> IdentifyRiskFactors(SpecializationProgressSummary progress, 
        WeightedProgressStatistics weightedProgress)
    {
        var risks = new List<string>();

        if (!progress.IsOnTrack)
            risks.Add("Behind schedule");

        if (progress.NetDurationAdjustment > 30)
            risks.Add("Significant time extension");

        if (weightedProgress.TotalAbsences > 3)
            risks.Add("High number of absences");

        if (weightedProgress.WeightedProgressPercentage < 0.6)
            risks.Add("Low overall progress");

        if (weightedProgress.QualityScore < 20)
            risks.Add("Low quality engagement");

        return risks;
    }

    private static IEnumerable<string> GenerateRecommendations(SpecializationProgressSummary progress, 
        WeightedProgressStatistics weightedProgress, IEnumerable<string> riskFactors)
    {
        var recommendations = new List<string>();

        if (riskFactors.Contains("Behind schedule"))
            recommendations.Add("Increase internship and course completion rate");

        if (riskFactors.Contains("Low quality engagement"))
            recommendations.Add("Focus on research publications and self-education activities");

        if (weightedProgress.InternshipProgress < 0.5)
            recommendations.Add("Prioritize completing required internships");

        if (weightedProgress.CourseProgress < 0.5)
            recommendations.Add("Focus on mandatory course completion");

        if (weightedProgress.TotalPublications == 0)
            recommendations.Add("Consider engaging in research and publication activities");

        return recommendations;
    }
}