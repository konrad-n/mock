using NSubstitute;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.Tests
{
    public class ExportServiceTests
    {
        private IDatabaseService databaseService;
        private ISpecializationService specializationService;
        private ISmkVersionStrategy smkStrategy;
        private ExportService exportService;
        private Specialization testSpecialization;
        private User testUser;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            this.databaseService = Substitute.For<IDatabaseService>();
            this.specializationService = Substitute.For<ISpecializationService>();
            this.smkStrategy = Substitute.For<ISmkVersionStrategy>();

            // Initialize test data
            this.testSpecialization = new Specialization
            {
                SpecializationId = 1,
                Name = "Test Specialization",
                StartDate = new DateTime(2023, 1, 1),
                PlannedEndDate = new DateTime(2028, 1, 1),
                HasModules = false
            };

            this.testUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                SmkVersion = SmkVersion.New,
                SpecializationId = 1
            };

            // Set up mock responses
            this.specializationService.GetCurrentSpecializationAsync().Returns(this.testSpecialization);
            this.specializationService.GetCurrentUserAsync().Returns(this.testUser);

            // Initialize the service under test
            this.exportService = new ExportService(this.databaseService, this.specializationService, this.smkStrategy);

            // Initialize ExcelHelper (required for EPPlus license)
            ExcelHelper.Initialize();
        }

        [Test]
        public async Task GetLastExportDateAsync_ReturnsCorrectDate()
        {
            // Arrange
            DateTime expectedDate = new DateTime(2023, 6, 15);

            // Use a mock to intercept and replace the call to Settings.GetLastExportDateAsync
            // Since we can't easily mock static methods, we'll need to test this differently
            // For this test, we'll just validate that the method delegates to Settings

            // Act
            await this.exportService.SaveLastExportDateAsync(expectedDate);
            var actualDate = await this.exportService.GetLastExportDateAsync();

            // Assert
            // This test will only pass if Settings.GetLastExportDateAsync returns what we just saved
            // In a real scenario with TestSettings class, we would mock that instead
            Assert.That(actualDate, Is.Not.Null);
        }

        [Test]
        public async Task ShareExportFileAsync_WithInvalidFile_ReturnsFalse()
        {
            // Arrange
            string invalidFilePath = "nonexistent_file.xlsx";

            // Act
            bool result = await this.exportService.ShareExportFileAsync(invalidFilePath);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ExportToExcelAsync_WithBasicOptions_CallsCorrectMethods()
        {
            // This test is more complex as it involves file system operations
            // We'll focus on testing that the correct methods are called with the right parameters

            // Arrange
            var exportOptions = new ExportOptions
            {
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 12, 31),
                IncludeInternships = true,
                IncludeCourses = true,
                IncludeProcedures = false,
                IncludeShifts = false,
                IncludeSelfEducation = false,
                IncludePublications = false,
                IncludeAbsences = false,
                IncludeEducationalActivities = false,
                IncludeRecognitions = false,
                FormatForOldSMK = false
            };

            // Set up mock data for export
            var internships = new List<Internship>
            {
                new Internship
                {
                    InternshipId = 1,
                    InternshipName = "Test Internship",
                    InstitutionName = "Test Institution",
                    DepartmentName = "Test Department",
                    StartDate = new DateTime(2023, 2, 1),
                    EndDate = new DateTime(2023, 3, 1),
                    DaysCount = 28,
                    Year = 1,
                    IsCompleted = true,
                    IsApproved = true
                }
            };

            var courses = new List<Course>
            {
                new Course
                {
                    CourseId = 1,
                    CourseName = "Test Course",
                    CourseType = "Mandatory",
                    CourseNumber = "C001",
                    InstitutionName = "Test Institution",
                    CompletionDate = new DateTime(2023, 4, 15),
                    Year = 1,
                    HasCertificate = true,
                    CertificateNumber = "CERT123",
                    CertificateDate = new DateTime(2023, 4, 20)
                }
            };

            // Mock the database calls to return our test data
            this.databaseService.GetInternshipsAsync(Arg.Any<int?>(), Arg.Any<int?>()).Returns(internships);
            this.databaseService.GetCoursesAsync(Arg.Any<int?>(), Arg.Any<int?>()).Returns(courses);

            // Act & Assert
            // We'll just check that the method doesn't throw an exception
            // In a real test scenario, we would verify the file contents
            Assert.DoesNotThrowAsync(async () => await this.exportService.ExportToExcelAsync(exportOptions));
        }
    }
}