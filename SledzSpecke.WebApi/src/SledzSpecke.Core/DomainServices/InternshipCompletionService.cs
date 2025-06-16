using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class InternshipCompletionService : IInternshipCompletionService
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    private const int OldSmkMinimumShiftHours = 160; // Monthly minimum
    private const int NewSmkMinimumShiftHours = 140; // Varies by module

    public InternshipCompletionService(
        IInternshipRepository internshipRepository,
        IMedicalShiftRepository medicalShiftRepository,
        IProcedureRepository procedureRepository,
        ISpecializationRepository specializationRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _internshipRepository = internshipRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _procedureRepository = procedureRepository;
        _specializationRepository = specializationRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<InternshipProgress>> CalculateProgressAsync(
        Internship internship,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureBase> procedures)
    {
        var progress = new InternshipProgress
        {
            InternshipId = internship.InternshipId,
            InternshipName = $"{internship.DepartmentName} - {internship.InstitutionName}",
            RequiredDays = internship.DaysCount
        };

        // Calculate days progress
        var daysPassed = (DateTime.UtcNow - internship.StartDate).Days + 1; // +1 to include start date
        progress.CompletedDays = Math.Min(daysPassed, internship.DaysCount);
        progress.DaysProgressPercentage = internship.DaysCount > 0 
            ? (progress.CompletedDays * 100.0) / internship.DaysCount 
            : 0;

        // Calculate shift hours
        var internshipShifts = shifts.Where(s => s.InternshipId == internship.InternshipId);
        progress.CompletedShiftHours = internshipShifts.Sum(s => s.TotalMinutes) / 60;
        
        // Get requirements based on specialization
        var requirementsResult = await GetCompletionRequirementsAsync(internship.InternshipId);
        if (!requirementsResult.IsSuccess)
        {
            return Result<InternshipProgress>.Failure(requirementsResult.Error);
        }
        
        var requirements = requirementsResult.Value;
        progress.RequiredShiftHours = requirements.MinimumShiftHours;
        progress.ShiftProgressPercentage = progress.RequiredShiftHours > 0
            ? Math.Min(100, (progress.CompletedShiftHours * 100.0) / progress.RequiredShiftHours)
            : 100;

        // Calculate procedures
        var internshipProcedures = procedures.Where(p => 
            p.InternshipId == internship.InternshipId && p.IsCompleted);
        progress.CompletedProcedures = internshipProcedures.Count();
        progress.RequiredProcedures = requirements.MinimumProcedures;
        progress.ProcedureProgressPercentage = progress.RequiredProcedures > 0
            ? Math.Min(100, (progress.CompletedProcedures * 100.0) / progress.RequiredProcedures)
            : 100;

        // Calculate overall progress
        progress.OverallProgressPercentage = CalculateOverallProgress(
            progress.DaysProgressPercentage,
            progress.ShiftProgressPercentage,
            progress.ProcedureProgressPercentage);

        // Check requirements
        progress.MeetsAllRequirements = 
            progress.CompletedDays >= progress.RequiredDays &&
            progress.CompletedShiftHours >= progress.RequiredShiftHours &&
            progress.CompletedProcedures >= progress.RequiredProcedures;

        // Add unmet requirements
        if (progress.CompletedDays < progress.RequiredDays)
        {
            progress.UnmetRequirements.Add($"Wymagane dni: {progress.RequiredDays - progress.CompletedDays} pozostało");
        }
        
        if (progress.CompletedShiftHours < progress.RequiredShiftHours)
        {
            progress.UnmetRequirements.Add($"Wymagane godziny: {progress.RequiredShiftHours - progress.CompletedShiftHours} pozostało");
        }
        
        if (progress.CompletedProcedures < progress.RequiredProcedures)
        {
            progress.UnmetRequirements.Add($"Wymagane procedury: {progress.RequiredProcedures - progress.CompletedProcedures} pozostało");
        }

        // Estimate completion date
        if (!progress.MeetsAllRequirements)
        {
            progress.EstimatedCompletionDate = EstimateCompletionDate(internship, progress);
        }

        return Result<InternshipProgress>.Success(progress);
    }

    public async Task<Result<bool>> CanCompleteAsync(
        Internship internship,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureBase> procedures)
    {
        var progressResult = await CalculateProgressAsync(internship, shifts, procedures);
        if (!progressResult.IsSuccess)
        {
            return Result<bool>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        
        // Check if all requirements are met
        if (!progress.MeetsAllRequirements)
        {
            return Result<bool>.Success(false);
        }

        // Check if internship is not already completed
        if (internship.IsCompleted)
        {
            return Result<bool>.Failure("Staż jest już zakończony");
        }

        // Check if current date is after start date
        if (DateTime.UtcNow < internship.StartDate)
        {
            return Result<bool>.Failure("Nie można zakończyć stażu przed datą rozpoczęcia");
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result> CompleteInternshipAsync(
        InternshipId internshipId,
        UserId completedBy,
        DateTime completionDate)
    {
        var internship = await _internshipRepository.GetByIdAsync(internshipId);
        if (internship == null)
        {
            return Result.Failure("Nie znaleziono stażu");
        }

        // Get related data
        var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(internshipId.Value);
        var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);

        // Check if can complete
        var canCompleteResult = await CanCompleteAsync(internship, shifts, procedures);
        if (!canCompleteResult.IsSuccess || !canCompleteResult.Value)
        {
            return Result.Failure("Nie można zakończyć stażu - nie wszystkie wymagania są spełnione");
        }

        // Complete the internship
        var markAsCompletedResult = internship.MarkAsCompleted();
        if (!markAsCompletedResult.IsSuccess)
        {
            return markAsCompletedResult;
        }

        // Update repository
        await _internshipRepository.UpdateAsync(internship);

        // Raise domain event
        var completedEvent = new InternshipCompletedEvent(
            internshipId,
            completedBy,
            completionDate,
            internship.SpecializationId,
            internship.ModuleId);
        
        await _eventDispatcher.DispatchAsync(completedEvent);

        return Result.Success();
    }

    public async Task<Result<InternshipCompletionRequirements>> GetCompletionRequirementsAsync(
        InternshipId internshipId)
    {
        var internship = await _internshipRepository.GetByIdAsync(internshipId);
        if (internship == null)
        {
            return Result<InternshipCompletionRequirements>.Failure("Nie znaleziono stażu");
        }

        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization == null)
        {
            return Result<InternshipCompletionRequirements>.Failure("Nie znaleziono specjalizacji");
        }

        var requirements = new InternshipCompletionRequirements
        {
            MinimumDays = internship.DaysCount,
            SmkVersion = specialization.SmkVersion,
            RequiresSupervisorApproval = specialization.SmkVersion == SmkVersion.New
        };

        // Set shift hour requirements based on SMK version
        if (specialization.SmkVersion == SmkVersion.Old)
        {
            requirements.MinimumShiftHours = CalculateOldSmkShiftHours(internship);
            requirements.MinimumProcedures = 10; // Example minimum
        }
        else
        {
            requirements.MinimumShiftHours = CalculateNewSmkShiftHours(internship, specialization);
            requirements.MinimumProcedures = 15; // Higher for new SMK
            requirements.SpecificRequirements.Add("Wymagane zatwierdzenie przez opiekuna");
        }

        // Add specific requirements
        if (internship.DaysCount >= 30)
        {
            requirements.SpecificRequirements.Add("Miesięczne sprawozdanie wymagane");
        }

        return Result<InternshipCompletionRequirements>.Success(requirements);
    }

    public async Task<Result<IEnumerable<InternshipMilestone>>> GetMilestonesAsync(
        InternshipId internshipId)
    {
        var internship = await _internshipRepository.GetByIdAsync(internshipId);
        if (internship == null)
        {
            return Result<IEnumerable<InternshipMilestone>>.Failure("Nie znaleziono stażu");
        }

        var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(internshipId.Value);
        var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
        
        var progressResult = await CalculateProgressAsync(internship, shifts, procedures);
        if (!progressResult.IsSuccess)
        {
            return Result<IEnumerable<InternshipMilestone>>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        var milestones = new List<InternshipMilestone>();

        // Days milestones
        milestones.Add(CreateMilestone(
            "25% dni ukończone",
            "Ukończono 25% wymaganych dni stażu",
            25,
            progress.DaysProgressPercentage >= 25,
            MilestoneType.Days25Percent));

        milestones.Add(CreateMilestone(
            "50% dni ukończone",
            "Ukończono połowę wymaganych dni stażu",
            50,
            progress.DaysProgressPercentage >= 50,
            MilestoneType.Days50Percent));

        milestones.Add(CreateMilestone(
            "75% dni ukończone",
            "Ukończono 75% wymaganych dni stażu",
            75,
            progress.DaysProgressPercentage >= 75,
            MilestoneType.Days75Percent));

        // Procedure milestones
        if (progress.RequiredProcedures > 0)
        {
            milestones.Add(CreateMilestone(
                "Pierwsza procedura",
                "Wykonano pierwszą procedurę",
                0,
                progress.CompletedProcedures > 0,
                MilestoneType.FirstProcedure));

            milestones.Add(CreateMilestone(
                "Połowa procedur",
                "Wykonano połowę wymaganych procedur",
                50,
                progress.ProcedureProgressPercentage >= 50,
                MilestoneType.HalfProcedures));
        }

        // Time-based milestones
        var daysSinceStart = (DateTime.UtcNow - internship.StartDate).Days;
        
        milestones.Add(CreateMilestone(
            "Pierwszy tydzień",
            "Ukończono pierwszy tydzień stażu",
            0,
            daysSinceStart >= 7,
            MilestoneType.FirstWeekCompleted));

        if (internship.DaysCount >= 30)
        {
            milestones.Add(CreateMilestone(
                "Pierwszy miesiąc",
                "Ukończono pierwszy miesiąc stażu",
                0,
                daysSinceStart >= 30,
                MilestoneType.FirstMonthCompleted));
        }

        return Result<IEnumerable<InternshipMilestone>>.Success(milestones);
    }

    private static double CalculateOverallProgress(
        double daysProgress,
        double shiftProgress,
        double procedureProgress)
    {
        // Weighted average with days being most important
        return (daysProgress * 0.4) + (shiftProgress * 0.3) + (procedureProgress * 0.3);
    }

    private static DateTime EstimateCompletionDate(
        Internship internship,
        InternshipProgress progress)
    {
        // Estimate based on current progress rate
        var daysRemaining = internship.DaysCount - progress.CompletedDays;
        var estimatedDate = DateTime.UtcNow.AddDays(daysRemaining);

        // Adjust based on shift hours progress
        if (progress.ShiftProgressPercentage < progress.DaysProgressPercentage)
        {
            // Need to work more hours per day
            var additionalDays = (int)((100 - progress.ShiftProgressPercentage) / 5); // Rough estimate
            estimatedDate = estimatedDate.AddDays(additionalDays);
        }

        return estimatedDate;
    }

    private static int CalculateOldSmkShiftHours(Internship internship)
    {
        // Old SMK: 160 hours per month minimum
        var months = Math.Max(1, internship.DaysCount / 30);
        return OldSmkMinimumShiftHours * months;
    }

    private static int CalculateNewSmkShiftHours(Internship internship, Specialization specialization)
    {
        // New SMK: varies by module
        // This is simplified - in real implementation would check module requirements
        var months = Math.Max(1, internship.DaysCount / 30);
        return NewSmkMinimumShiftHours * months;
    }

    private static InternshipMilestone CreateMilestone(
        string name,
        string description,
        double targetPercentage,
        bool isAchieved,
        MilestoneType type)
    {
        return new InternshipMilestone
        {
            Name = name,
            Description = description,
            TargetPercentage = targetPercentage,
            IsAchieved = isAchieved,
            AchievedDate = isAchieved ? DateTime.UtcNow : null,
            Type = type
        };
    }
}

public class InternshipCompletedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public InternshipId InternshipId { get; }
    public UserId CompletedBy { get; }
    public DateTime CompletionDate { get; }
    public SpecializationId SpecializationId { get; }
    public ModuleId? ModuleId { get; }
    public DateTime OccurredAt { get; }

    public InternshipCompletedEvent(
        InternshipId internshipId,
        UserId completedBy,
        DateTime completionDate,
        SpecializationId specializationId,
        ModuleId? moduleId)
    {
        EventId = Guid.NewGuid();
        InternshipId = internshipId;
        CompletedBy = completedBy;
        CompletionDate = completionDate;
        SpecializationId = specializationId;
        ModuleId = moduleId;
        OccurredAt = DateTime.UtcNow;
    }
}