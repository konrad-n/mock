using NSubstitute;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.Tests.TestHelpers;
using SledzSpecke.Tests.TestUtilities;

namespace SledzSpecke.Tests.Services.Export
{
    [TestFixture]
    public class ExportServiceTests
    {
        private IDatabaseService databaseService;
        private ISpecializationService specializationService;
        private ISmkVersionStrategy smkStrategy;
        private ExportService exportService;
        private App.Models.Specialization testSpecialization;
        private User testUser;
        private TestSecureStorageService secureStorageService;
        private TestFileSystemService fileSystemService;

        [SetUp]
        public void Setup()
        {
            // Initialize test services
            this.secureStorageService = new TestSecureStorageService();
            this.fileSystemService = new TestFileSystemService(useInMemoryStorage: true);

            // Set our test services
            Constants.SetFileSystemService(this.fileSystemService);
            TestHelpers.Settings.SetSecureStorageService(this.secureStorageService);

            // Initialize mocks
            this.databaseService = Substitute.For<IDatabaseService>();
            this.specializationService = Substitute.For<ISpecializationService>();
            this.smkStrategy = Substitute.For<ISmkVersionStrategy>();

            // Initialize test data
            this.testSpecialization = new App.Models.Specialization
            {
                SpecializationId = 1,
                Name = "Test Specialization",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                HasModules = false,
            };

            this.testUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                SmkVersion = SmkVersion.New,
                SpecializationId = 1,
            };

            // Set up mock responses
            this.specializationService.GetCurrentSpecializationAsync().Returns(this.testSpecialization);
            this.specializationService.GetCurrentUserAsync().Returns(this.testUser);

            // Initialize the service under test
            this.exportService = new ExportService(this.databaseService, this.specializationService, this.smkStrategy);
        }

        [TearDown]
        public void Cleanup()
        {
            // Clear test storage
            this.secureStorageService.RemoveAll();
            this.fileSystemService.ClearAllFiles();
        }

        [Test]
        public async Task GetLastExportDateAsync_ReturnsCorrectDate()
        {
            // Arrange
            DateTime expectedDate = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Local);

            // Act
            await TestHelpers.Settings.SetLastExportDateAsync(expectedDate);
            var actualDate = await TestHelpers.Settings.GetLastExportDateAsync();

            // Assert
            Assert.That(actualDate, Is.Not.Null);
            Assert.That(actualDate.Value.Date, Is.EqualTo(expectedDate.Date));
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
        public async Task SaveLastExportDateAsync_SetsDatabaseValue()
        {
            // Arrange
            DateTime testDate = new DateTime(2023, 5, 10, 0, 0, 0, DateTimeKind.Local);

            // Act
            await this.exportService.SaveLastExportDateAsync(testDate);

            // Retrieve the value directly from the app's Settings helper, not the test helper
            // This ensures we're checking the same storage location that the ExportService uses
            var retrievedDate = await App.Helpers.Settings.GetLastExportDateAsync();

            // Assert
            Assert.That(retrievedDate, Is.Not.Null);
            Assert.That(retrievedDate.Value.Date, Is.EqualTo(testDate.Date));
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
                File.WriteAllText(filePath, "test content");

                // Configure mock to return our test path
                mockFileSystemService.GetAppSubdirectory(Arg.Any<string>()).Returns(exportDir);

                // Create service with mocks
                var exportService = new ExportService(mockDatabaseService, null, mockSmkStrategy);

                // Set the last export file path via reflection (since it's private)
                var fieldInfo = typeof(ExportService).GetField("lastExportFilePath",
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
                try { Directory.Delete(exportDir, true); } catch { /* Ignore cleanup errors */ }
            }
        }

        [Test]
        public void VerifyServicesAreCorrectlySet()
        {
            // This test just verifies that our service dependency injection is working
            // Act & Assert
            var actualSecureStorage = TestServiceProvider.GetSecureStorageService();
            var actualFileSystem = TestServiceProvider.GetFileSystemService();

            Assert.That(actualSecureStorage, Is.Not.Null);
            Assert.That(actualFileSystem, Is.Not.Null);
            Assert.That(actualSecureStorage, Is.InstanceOf<TestSecureStorageService>());
            Assert.That(actualFileSystem, Is.InstanceOf<TestFileSystemService>());
        }
    }
}