using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Tests.DomainServices;

public class CourseRequirementServiceTests
{
    private readonly Mock<ISpecializationRepository> _specializationRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IDomainEventDispatcher> _eventDispatcherMock;
    private readonly CourseRequirementService _service;
    private static int _testUserId = 1;

    public CourseRequirementServiceTests()
    {
        _specializationRepositoryMock = new Mock<ISpecializationRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _eventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _service = new CourseRequirementService(
            _specializationRepositoryMock.Object,
            _userRepositoryMock.Object,
            _eventDispatcherMock.Object);
    }

    [Fact]
    public async Task GetRequirementsAsync_ForAnesthesiology_ReturnsCorrectRequirements()
    {
        // Arrange
        var specializationId = new SpecializationId(1);
        var specialization = new Specialization(
            specializationId,
            new UserId(1),
            "Anestezjologia i intensywna terapia",
            "ANEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.GetRequirementsAsync(
            specializationId,
            SmkVersion.Old,
            year: 1);

        // Assert
        Assert.True(result.IsSuccess);
        var requirements = result.Value;
        Assert.Equal(specializationId, requirements.SpecializationId);
        Assert.Equal(SmkVersion.Old, requirements.SmkVersion);
        Assert.Equal(1, requirements.Year);
        Assert.NotEmpty(requirements.RequiredCourses);
        Assert.True(requirements.TotalRequiredHours > 0);
        Assert.True(requirements.MinimumPassingCourses > 0);
    }

    [Fact]
    public async Task GetRequirementsAsync_ForNonExistentSpecialization_ReturnsFailure()
    {
        // Arrange
        var specializationId = new SpecializationId(999);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync((Specialization)null);

        // Act
        var result = await _service.GetRequirementsAsync(
            specializationId,
            SmkVersion.New);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Nie znaleziono specjalizacji", result.Error);
    }

    [Fact]
    public async Task RecordCourseParticipationAsync_WithValidData_RecordsSuccessfully()
    {
        // Arrange
        var courseId = new CourseId(1);
        var specializationId = new SpecializationId(1);
        var participationDate = DateTime.UtcNow.AddDays(-5);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Test Specialization",
            "TEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.RecordCourseParticipationAsync(
            user.Id,
            courseId,
            participationDate,
            40,
            true);

        // Assert
        Assert.True(result.IsSuccess);
        _eventDispatcherMock.Verify(d => d.DispatchAsync(
            It.IsAny<CourseCompletedEvent>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecordCourseParticipationAsync_WithInvalidHours_ReturnsFailure()
    {
        // Arrange
        var courseId = new CourseId(1);

        var user = CreateTestUser(new SpecializationId(1), SmkVersion.Old);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.RecordCourseParticipationAsync(
            user.Id,
            courseId,
            DateTime.UtcNow,
            0, // Invalid hours
            true);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Liczba godzin musi być większa niż 0", result.Error);
    }

    [Fact]
    public async Task GetUserCourseProgressAsync_ForUserWithCourses_ReturnsProgress()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Anestezjologia",
            "ANEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // First record some courses
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(1), DateTime.UtcNow.AddDays(-30), 40, true);
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(2), DateTime.UtcNow.AddDays(-20), 60, true);

        // Act
        var result = await _service.GetUserCourseProgressAsync(user.Id, specializationId);

        // Assert
        Assert.True(result.IsSuccess);
        var progress = result.Value;
        Assert.Equal(user.Id, progress.UserId);
        Assert.Equal(specializationId, progress.SpecializationId);
        Assert.Equal(2, progress.TotalCoursesCompleted);
        Assert.Equal(100, progress.TotalHoursCompleted);
        Assert.Equal(2, progress.TotalCoursesPassed);
        Assert.NotEmpty(progress.RemainingRequiredCourses);
    }

    [Fact]
    public async Task ValidateCourseCompletionAsync_WithAllRequirementsMet_ReturnsSuccess()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Anestezjologia",
            "ANEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Record all required courses
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(1), DateTime.UtcNow.AddDays(-30), 40, true);
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(2), DateTime.UtcNow.AddDays(-20), 60, true);
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(3), DateTime.UtcNow.AddDays(-10), 30, true);

        // Act
        var result = await _service.ValidateCourseCompletionAsync(
            user.Id, 
            specializationId,
            year: 1);

        // Assert
        Assert.True(result.IsSuccess);
        var status = result.Value;
        Assert.True(status.MeetsAllRequirements);
        Assert.Empty(status.UnmetRequirements);
        Assert.Empty(status.MissingMandatoryCourses);
    }

    [Fact]
    public async Task ValidateCourseCompletionAsync_WithMissingCourses_ShowsUnmetRequirements()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Anestezjologia",
            "ANEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Record only one course
        await _service.RecordCourseParticipationAsync(
            user.Id, new CourseId(1), DateTime.UtcNow.AddDays(-30), 40, true);

        // Act
        var result = await _service.ValidateCourseCompletionAsync(userId, specializationId);

        // Assert
        Assert.True(result.IsSuccess);
        var status = result.Value;
        Assert.False(status.MeetsAllRequirements);
        Assert.NotEmpty(status.UnmetRequirements);
        Assert.NotEmpty(status.MissingMandatoryCourses);
        Assert.True(status.CompletedHours < status.RequiredHours);
    }

    [Fact]
    public async Task GetUpcomingRequiredCoursesAsync_ReturnsCoursesInPriorityOrder()
    {
        // Arrange
        var userId = new UserId(1);
        var specializationId = new SpecializationId(1);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Anestezjologia",
            "ANEST",
            new SmkVersion("old"),
            "standard", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(4),
            2025, // plannedPesYear
            "Standard Program",
            5);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.GetUpcomingRequiredCoursesAsync(user.Id, specializationId);

        // Assert
        Assert.True(result.IsSuccess);
        var upcomingCourses = result.Value.ToList();
        Assert.NotEmpty(upcomingCourses);
        
        // Verify courses are sorted by priority
        for (int i = 1; i < upcomingCourses.Count; i++)
        {
            Assert.True(upcomingCourses[i-1].Priority <= upcomingCourses[i].Priority);
        }
        
        // Verify mandatory courses have priority 1
        var mandatoryCourses = upcomingCourses.Where(c => c.Requirement.IsMandatory);
        Assert.All(mandatoryCourses, c => Assert.Equal(1, c.Priority));
        
        // Verify all have recommended timing and location
        Assert.All(upcomingCourses, c => 
        {
            Assert.NotEmpty(c.RecommendedTiming);
            Assert.NotEmpty(c.Location);
            Assert.NotNull(c.NextAvailableDate);
        });
    }
    
    private User CreateTestUser(SpecializationId specializationId, SmkVersion smkVersion)
    {
        return User.CreateWithId(
            new UserId(_testUserId++),
            new Email($"test{_testUserId}@example.com"),
            new HashedPassword("tL8XQn5ScIhHqxKNMQJfYGD3GmjptUPgxlrXH1zVBvI="), // Valid base64 hash
            new FirstName("Test"),
            null, // SecondName
            new LastName("User"),
            new Pesel("44051401458"), // Valid PESEL with correct checksum
            new PwzNumber("5425127"), // Valid PWZ with correct checksum
            new PhoneNumber("+48123456789"), // Valid phone
            new DateTime(1944, 5, 14), // Date of birth matching PESEL 44051401458
            new Address("Test Street", "1", null, "00-000", "Warsaw", "Mazowieckie", "Poland"),
            DateTime.UtcNow);
    }

    [Fact]
    public async Task RecordCourseParticipationAsync_ForNewSmk_HandlesModuleRequirements()
    {
        // Arrange
        var courseId = new CourseId(1);
        var specializationId = new SpecializationId(2);
        var moduleId = new ModuleId(1);

        var user = CreateTestUser(specializationId, SmkVersion.Old);

        var specialization = new Specialization(
            specializationId,
            user.Id,
            "Chirurgia onkologiczna",
            "CHOG",
            new SmkVersion("new"),
            "modular", // programVariant
            DateTime.Today.AddYears(-1),
            DateTime.Today.AddYears(5),
            2025, // plannedPesYear
            "Modular Program",
            0); // Module-based, not year-based
            
        // Update user to have matching SMK version
        var userWithNewSmk = CreateTestUser(specializationId, SmkVersion.New);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userWithNewSmk.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userWithNewSmk);
        _specializationRepositoryMock.Setup(r => r.GetByIdAsync(specializationId))
            .ReturnsAsync(specialization);

        // Act
        var recordResult = await _service.RecordCourseParticipationAsync(
            userWithNewSmk.Id,
            courseId,
            DateTime.UtcNow.AddDays(-5),
            50,
            true);

        var requirementsResult = await _service.GetRequirementsAsync(
            specializationId,
            SmkVersion.New,
            moduleId: moduleId);

        // Assert
        Assert.True(recordResult.IsSuccess);
        Assert.True(requirementsResult.IsSuccess);
        var requirements = requirementsResult.Value;
        Assert.Equal(SmkVersion.New, requirements.SmkVersion);
        Assert.Equal(moduleId, requirements.ModuleId);
        Assert.Null(requirements.Year);
    }
}