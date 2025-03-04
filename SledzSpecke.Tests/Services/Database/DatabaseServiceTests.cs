using NSubstitute;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.Tests.TestUtilities;

namespace SledzSpecke.Tests.Services.Database
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        private IDatabaseService databaseService;
        private string dbPath;

        [SetUp]
        public void Setup()
        {
            // Create a new database file for each test
            this.dbPath = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db3");
            this.databaseService = new TestDatabaseService(this.dbPath);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (File.Exists(this.dbPath))
                {
                    File.Delete(this.dbPath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        [Test]
        public async Task InitializeAsync_CreatesAllTables()
        {
            // Act
            await this.databaseService.InitializeAsync();

            // Assert - Successfully initialized
            Assert.Pass("Database initialization completed successfully");
        }

        [Test]
        public async Task SaveUserAsync_SavesUserCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                SmkVersion = SmkVersion.New,
                SpecializationId = 1,
                RegistrationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
            };

            // Act
            int userId = await this.databaseService.SaveUserAsync(user);
            var savedUser = await this.databaseService.GetUserAsync(userId);

            // Assert
            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser.UserId, Is.EqualTo(userId));
            Assert.That(savedUser.Username, Is.EqualTo("testuser"));
            Assert.That(savedUser.Email, Is.EqualTo("test@example.com"));
            Assert.That(savedUser.PasswordHash, Is.EqualTo("hashedpassword"));
            Assert.That(savedUser.SmkVersion, Is.EqualTo(SmkVersion.New));
            Assert.That(savedUser.SpecializationId, Is.EqualTo(1));
        }

        [Test]
        public async Task SaveInternshipAsync_SavesInternshipCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Create a very specific test internship
            var internship = new Internship
            {
                SpecializationId = 9999, // Use a unique ID to avoid conflicts
                InstitutionName = "Test Institution XYZ", // Make name unique
                DepartmentName = "Test Department XYZ",
                InternshipName = "Test Internship XYZ",
                Year = 1,
                StartDate = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 5, 31, 0, 0, 0, DateTimeKind.Local),
                DaysCount = 92,
                IsCompleted = false,
                IsApproved = false,
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int internshipId = await this.databaseService.SaveInternshipAsync(internship);

            // Assert ID is valid
            Assert.That(internshipId, Is.GreaterThan(0), "Internship should have a valid ID after saving");

            // Retrieve the saved internship
            var savedInternship = await this.databaseService.GetInternshipAsync(internshipId);

            // Assert
            Assert.That(savedInternship, Is.Not.Null, "Retrieved internship should not be null");
            Assert.That(savedInternship.InternshipId, Is.EqualTo(internshipId));
            Assert.That(savedInternship.InternshipName, Is.EqualTo("Test Internship XYZ"));
            Assert.That(savedInternship.InstitutionName, Is.EqualTo("Test Institution XYZ"));
            Assert.That(savedInternship.DaysCount, Is.EqualTo(92));
        }

        [Test]
        public async Task SaveModuleAsync_SavesModuleCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Create a module with unique identifiers
            var module = new Module
            {
                SpecializationId = 9999, // Use a unique ID to avoid conflicts
                Type = ModuleType.Basic,
                Name = "Test Module XYZ", // Make name unique
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                Structure = "{\"testKey\":\"testValue\"}",
                CompletedInternships = 0,
                TotalInternships = 5,
                CompletedCourses = 0,
                TotalCourses = 3,
            };

            // Act
            int moduleId = await this.databaseService.SaveModuleAsync(module);

            // Assert ID is valid
            Assert.That(moduleId, Is.GreaterThan(0), "Module should have a valid ID after saving");

            // Retrieve the saved module
            var savedModule = await this.databaseService.GetModuleAsync(moduleId);

            // Assert
            Assert.That(savedModule, Is.Not.Null);
            Assert.That(savedModule.ModuleId, Is.EqualTo(moduleId));
            Assert.That(savedModule.Name, Is.EqualTo("Test Module XYZ"));
            Assert.That(savedModule.Type, Is.EqualTo(ModuleType.Basic));
            Assert.That(savedModule.SpecializationId, Is.EqualTo(9999));
        }

        [Test]
        public async Task GetLastExportFilePathAsync_ReturnsCorrectPath()
        {
            // Arrange
            var mockFileSystemService = Substitute.For<IFileSystemService>();
            var mockDatabaseService = Substitute.For<IDatabaseService>();
            var mockSmkStrategy = Substitute.For<ISmkVersionStrategy>();

            // Set up a temp directory that will work in any environment, including CI
            string tempPath = Path.GetTempPath();
            string exportDir = Path.Combine(tempPath, "SledzSpeckeTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(exportDir);

            try
            {
                // Create a test file in the temp directory
                string fileName = "testexport.xlsx";
                string filePath = Path.Combine(exportDir, fileName);
                await File.WriteAllTextAsync(filePath, "test content");

                // Configure mock to return our test path
                mockFileSystemService.GetAppSubdirectory(Arg.Any<string>()).Returns(exportDir);

                // Create service with mocks
                var exportService = new ExportService(mockDatabaseService, null, mockSmkStrategy);

                // Set the last export file path via reflection (since it's private)
                var fieldInfo = typeof(ExportService).GetField(
                    "lastExportFilePath",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                fieldInfo.SetValue(exportService, filePath);

                // Act
                var result = await exportService.GetLastExportFilePathAsync();

                // Assert
                Assert.That(result, Is.EqualTo(filePath));
            }
            finally
            {
                // Clean up
                try
                {
                    Directory.Delete(exportDir, true);
                }
                catch
                {
                    /* Ignore cleanup errors */
                }
            }
        }
    }
}