using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class CourseRequirementService : ICourseRequirementService
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    // Mock data for course requirements - in real implementation would come from database
    private readonly Dictionary<string, List<CourseRequirement>> _courseRequirementsBySpecialization = new()
    {
        ["ANEST"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(1), CourseName = "Podstawy anestezjologii", CourseCode = "ANEST-001", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical },
            new() { CourseId = new CourseId(2), CourseName = "Intensywna terapia", CourseCode = "ANEST-002", RequiredHours = 60, IsMandatory = true, Type = CourseType.Practical },
            new() { CourseId = new CourseId(3), CourseName = "Zaawansowane techniki znieczulenia", CourseCode = "ANEST-003", RequiredHours = 30, IsMandatory = false, Type = CourseType.Workshop }
        },
        ["CHOG"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(4), CourseName = "Chirurgia onkologiczna podstawy", CourseCode = "CHOG-001", RequiredHours = 50, IsMandatory = true, Type = CourseType.Theoretical },
            new() { CourseId = new CourseId(5), CourseName = "Techniki operacyjne w onkologii", CourseCode = "CHOG-002", RequiredHours = 80, IsMandatory = true, Type = CourseType.Practical },
            new() { CourseId = new CourseId(6), CourseName = "Radioterapia i chemioterapia", CourseCode = "CHOG-003", RequiredHours = 40, IsMandatory = true, Type = CourseType.Seminar }
        }
    };
    
    // Mock completed courses data - in real implementation would come from database
    private readonly Dictionary<int, List<CompletedCourse>> _completedCoursesByUser = new();

    public CourseRequirementService(
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<CourseRequirements>> GetRequirementsAsync(
        SpecializationId specializationId,
        SmkVersion smkVersion,
        int? year = null,
        ModuleId? moduleId = null)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseRequirements>.Failure("Nie znaleziono specjalizacji");
        }

        var requirements = new CourseRequirements
        {
            SpecializationId = specializationId,
            SmkVersion = smkVersion,
            Year = year,
            ModuleId = moduleId
        };

        // Get courses for specialization
        if (_courseRequirementsBySpecialization.TryGetValue(specialization.ProgramCode, out var courses))
        {
            // Filter by year/module if needed
            if (smkVersion == SmkVersion.Old && year.HasValue)
            {
                // For Old SMK, distribute courses across years
                // Ensure at least 1 course per year if courses exist
                var coursesPerYear = Math.Max(1, courses.Count / specialization.DurationYears);
                var startIndex = (year.Value - 1) * coursesPerYear;
                var endIndex = Math.Min(startIndex + coursesPerYear, courses.Count);
                requirements.RequiredCourses = courses.Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            else if (smkVersion == SmkVersion.New && moduleId != null)
            {
                // For New SMK, assign courses to modules
                // This is simplified - in real implementation would have proper module mapping
                requirements.RequiredCourses = courses.Take(courses.Count / 2).ToList();
            }
            else
            {
                requirements.RequiredCourses = courses;
            }
        }

        // Calculate totals
        requirements.TotalRequiredHours = requirements.RequiredCourses.Sum(c => c.RequiredHours);
        requirements.MinimumPassingCourses = requirements.RequiredCourses.Count(c => c.IsMandatory);

        return Result<CourseRequirements>.Success(requirements);
    }

    public async Task<Result> RecordCourseParticipationAsync(
        UserId userId,
        CourseId courseId,
        DateTime participationDate,
        int hoursCompleted,
        bool isPassed)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("Nie znaleziono użytkownika");
        }

        // Validate hours
        if (hoursCompleted <= 0)
        {
            return Result.Failure("Liczba godzin musi być większa niż 0");
        }

        // TODO: Fix after User-Specialization relationship is properly implemented
        // For now, this is commented out as User no longer has SpecializationId
        /*
        var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        if (specialization == null)
        {
            return Result.Failure("Nie znaleziono specjalizacji użytkownika");
        }
        */

        // Record the participation
        var completedCourse = new CompletedCourse
        {
            CourseId = courseId,
            CourseName = GetCourseName(courseId), // In real implementation, would fetch from DB
            CompletionDate = participationDate,
            HoursCompleted = hoursCompleted,
            IsPassed = isPassed,
            CertificateNumber = isPassed ? GenerateCertificateNumber() : null
        };

        // Add to user's completed courses
        if (!_completedCoursesByUser.ContainsKey(userId.Value))
        {
            _completedCoursesByUser[userId.Value] = new List<CompletedCourse>();
        }
        _completedCoursesByUser[userId.Value].Add(completedCourse);

        // Raise domain event
        var courseCompletedEvent = new CourseCompletedEvent(
            userId,
            courseId,
            participationDate,
            hoursCompleted,
            isPassed);
        
        await _eventDispatcher.DispatchAsync(courseCompletedEvent);

        return Result.Success();
    }

    public async Task<Result<CourseProgress>> GetUserCourseProgressAsync(
        UserId userId,
        SpecializationId specializationId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<CourseProgress>.Failure("Nie znaleziono użytkownika");
        }

        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseProgress>.Failure("Nie znaleziono specjalizacji");
        }

        var progress = new CourseProgress
        {
            UserId = userId,
            SpecializationId = specializationId
        };

        // Get user's completed courses
        if (_completedCoursesByUser.TryGetValue(userId.Value, out var completedCourses))
        {
            progress.CompletedCourses = completedCourses;
            progress.TotalHoursCompleted = completedCourses.Sum(c => c.HoursCompleted);
            progress.TotalCoursesCompleted = completedCourses.Count;
            progress.TotalCoursesPassed = completedCourses.Count(c => c.IsPassed);
        }

        // Get requirements
        var requirementsResult = await GetRequirementsAsync(
            specializationId, 
            specialization.SmkVersion, 
            null, 
            null);
        
        if (!requirementsResult.IsSuccess)
        {
            return Result<CourseProgress>.Failure(requirementsResult.Error);
        }

        var requirements = requirementsResult.Value;
        
        // Calculate remaining courses
        var completedCourseIds = progress.CompletedCourses.Select(c => c.CourseId.Value).ToHashSet();
        progress.RemainingRequiredCourses = requirements.RequiredCourses
            .Where(r => !completedCourseIds.Contains(r.CourseId.Value))
            .ToList();

        // Calculate completion percentage
        if (requirements.RequiredCourses.Any())
        {
            var mandatoryCompleted = progress.CompletedCourses
                .Count(c => c.IsPassed && requirements.RequiredCourses
                    .Any(r => r.CourseId.Value == c.CourseId.Value && r.IsMandatory));
            
            var totalMandatory = requirements.RequiredCourses.Count(r => r.IsMandatory);
            progress.CompletionPercentage = totalMandatory > 0 
                ? (mandatoryCompleted * 100.0) / totalMandatory 
                : 100;
        }

        return Result<CourseProgress>.Success(progress);
    }

    public async Task<Result<CourseCompletionStatus>> ValidateCourseCompletionAsync(
        UserId userId,
        SpecializationId specializationId,
        int? year = null,
        ModuleId? moduleId = null)
    {
        var progressResult = await GetUserCourseProgressAsync(userId, specializationId);
        if (!progressResult.IsSuccess)
        {
            return Result<CourseCompletionStatus>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseCompletionStatus>.Failure("Nie znaleziono specjalizacji");
        }

        var requirementsResult = await GetRequirementsAsync(
            specializationId,
            specialization.SmkVersion,
            year,
            moduleId);
        
        if (!requirementsResult.IsSuccess)
        {
            return Result<CourseCompletionStatus>.Failure(requirementsResult.Error);
        }

        var requirements = requirementsResult.Value;
        var status = new CourseCompletionStatus
        {
            RequiredHours = requirements.TotalRequiredHours,
            CompletedHours = progress.TotalHoursCompleted,
            RequiredCourses = requirements.MinimumPassingCourses,
            CompletedCourses = progress.TotalCoursesPassed
        };

        // Check hours requirement
        if (status.CompletedHours < status.RequiredHours)
        {
            status.UnmetRequirements.Add($"Brakuje {status.RequiredHours - status.CompletedHours} godzin kursów");
        }

        // Check mandatory courses
        var completedCourseIds = progress.CompletedCourses
            .Where(c => c.IsPassed)
            .Select(c => c.CourseId.Value)
            .ToHashSet();
        
        status.MissingMandatoryCourses = requirements.RequiredCourses
            .Where(r => r.IsMandatory && !completedCourseIds.Contains(r.CourseId.Value))
            .ToList();

        if (status.MissingMandatoryCourses.Any())
        {
            status.UnmetRequirements.Add($"Brakuje {status.MissingMandatoryCourses.Count} obowiązkowych kursów");
        }

        // Check minimum courses
        if (status.CompletedCourses < status.RequiredCourses)
        {
            status.UnmetRequirements.Add($"Brakuje {status.RequiredCourses - status.CompletedCourses} zaliczonych kursów");
        }

        status.MeetsAllRequirements = !status.UnmetRequirements.Any();

        return Result<CourseCompletionStatus>.Success(status);
    }

    public async Task<Result<IEnumerable<RequiredCourse>>> GetUpcomingRequiredCoursesAsync(
        UserId userId,
        SpecializationId specializationId)
    {
        var progressResult = await GetUserCourseProgressAsync(userId, specializationId);
        if (!progressResult.IsSuccess)
        {
            return Result<IEnumerable<RequiredCourse>>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        var upcomingCourses = new List<RequiredCourse>();

        foreach (var requirement in progress.RemainingRequiredCourses)
        {
            var requiredCourse = new RequiredCourse
            {
                Requirement = requirement,
                Priority = requirement.IsMandatory ? 1 : 2,
                RecommendedTiming = GetRecommendedTiming(requirement),
                NextAvailableDate = GetNextCourseDate(requirement.CourseId),
                Location = GetCourseLocation(requirement.CourseId)
            };
            
            upcomingCourses.Add(requiredCourse);
        }

        // Sort by priority and date
        upcomingCourses = upcomingCourses
            .OrderBy(c => c.Priority)
            .ThenBy(c => c.NextAvailableDate ?? DateTime.MaxValue)
            .ToList();

        return Result<IEnumerable<RequiredCourse>>.Success(upcomingCourses);
    }

    // Helper methods
    private string GetCourseName(CourseId courseId)
    {
        // In real implementation, would fetch from database
        return _courseRequirementsBySpecialization
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(c => c.CourseId.Value == courseId.Value)?.CourseName ?? "Unknown Course";
    }

    private string GenerateCertificateNumber()
    {
        return $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }

    private string GetRecommendedTiming(CourseRequirement requirement)
    {
        if (requirement.IsMandatory)
        {
            return "Jak najszybciej - kurs obowiązkowy";
        }
        return "W dowolnym momencie specjalizacji";
    }

    private DateTime? GetNextCourseDate(CourseId courseId)
    {
        // Mock implementation - in real system would check course schedule
        var daysUntilNext = Random.Shared.Next(7, 60);
        return DateTime.UtcNow.AddDays(daysUntilNext);
    }

    private string GetCourseLocation(CourseId courseId)
    {
        // Mock implementation
        var locations = new[] { "Warszawa", "Kraków", "Wrocław", "Gdańsk", "Online" };
        return locations[Random.Shared.Next(locations.Length)];
    }
}

// Domain Event
public class CourseCompletedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public UserId UserId { get; }
    public CourseId CourseId { get; }
    public DateTime CompletionDate { get; }
    public int HoursCompleted { get; }
    public bool IsPassed { get; }
    public DateTime OccurredAt { get; }

    public CourseCompletedEvent(
        UserId userId,
        CourseId courseId,
        DateTime completionDate,
        int hoursCompleted,
        bool isPassed)
    {
        EventId = Guid.NewGuid();
        UserId = userId;
        CourseId = courseId;
        CompletionDate = completionDate;
        HoursCompleted = hoursCompleted;
        IsPassed = isPassed;
        OccurredAt = DateTime.UtcNow;
    }
}