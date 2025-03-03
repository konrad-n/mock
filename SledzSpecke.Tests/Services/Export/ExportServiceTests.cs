using NSubstitute;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.Tests.TestUtilities;

namespace SledzSpecke.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
        private IDatabaseService databaseService;
        private ISpecializationService specializationService;
        private ISmkVersionStrategy smkStrategy;
        private ExportService exportService;
        private Specialization testSpecialization;
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
            Settings.SetSecureStorageService(this.secureStorageService);

            // Initialize mocks
            this.databaseService = Substitute.For<IDatabaseService>();
            this.specializationService = Substitute.For<ISpecializationService>();
            this.smkStrategy = Substitute.For<ISmkVersionStrategy>();

            // Initialize test data
            this.testSpecialization = new Specialization
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
            await Settings.SetLastExportDateAsync(expectedDate);
            var actualDate = await Settings.GetLastExportDateAsync();

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

            // Retrieve the value directly from our test storage
            var retrievedDate = await Settings.GetLastExportDateAsync();

            // Assert
            Assert.That(retrievedDate, Is.Not.Null);
            Assert.That(retrievedDate.Value.Date, Is.EqualTo(testDate.Date));
        }

        [Test]
        public void GetLastExportFilePathAsync_ReturnsCorrectPath()
        {
            // Arrange
            string testPath = Path.Combine(this.fileSystemService.AppDataDirectory, "testexport.xlsx");
            this.fileSystemService.EnsureDirectoryExists(Path.GetDirectoryName(testPath));

            // Write a temporary file to simulate an export
            File.WriteAllText(testPath, "Test content");

            // Set the path in the service's field using reflection
            var fieldInfo = typeof(ExportService).GetField("lastExportFilePath",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo.SetValue(this.exportService, testPath);

            // Act
            var result = this.exportService.GetLastExportFilePathAsync().Result;

            // Assert
            Assert.That(result, Is.EqualTo(testPath));
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