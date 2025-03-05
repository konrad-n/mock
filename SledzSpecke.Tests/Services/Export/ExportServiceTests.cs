using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
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
        private Specialization testSpecialization;
        private User testUser;
        private TestSecureStorageService secureStorageService;
        private TestFileSystemService fileSystemService;
        private IFileAccessHelper fileAccessHelper;

        [SetUp]
        public void Setup()
        {
            // Initialize test services
            this.secureStorageService = new TestSecureStorageService();
            this.fileSystemService = new TestFileSystemService(useInMemoryStorage: true);

            // Set our test services
            TestHelpers.Constants.SetFileSystemService(this.fileSystemService);
            TestHelpers.SettingsHelper.SetSecureStorageService(this.secureStorageService);

            // Initialize mocks
            this.databaseService = Substitute.For<IDatabaseService>();
            this.specializationService = Substitute.For<ISpecializationService>();
            this.smkStrategy = Substitute.For<ISmkVersionStrategy>();
            this.fileAccessHelper = Substitute.For<IFileAccessHelper>();

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
            this.specializationService.GetCurrentSpecializationAsync()
                .Returns(this.testSpecialization);
            this.specializationService.GetCurrentUserAsync()
                .Returns(this.testUser);

            // Initialize the service under test
            this.exportService = new ExportService(
                this.databaseService,
                this.specializationService,
                this.smkStrategy,
                this.fileAccessHelper);
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
            await TestHelpers.SettingsHelper.SetLastExportDateAsync(expectedDate);
            var actualDate = await TestHelpers.SettingsHelper.GetLastExportDateAsync();

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

            // Retrieve the value directly from the app's Settings helper
            var retrievedDate = await App.Helpers.SettingsHelper.GetLastExportDateAsync();

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
            var mockFileAccessHelper = Substitute.For<IFileAccessHelper>();

            // Set up a temp directory that will work in any environment
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
                var sut = new ExportService(mockDatabaseService, null, mockSmkStrategy, mockFileAccessHelper);

                // Set the last export file path via reflection
                var fieldInfo = typeof(ExportService).GetField(
                    "lastExportFilePath",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo.SetValue(sut, filePath);

                // Act
                var result = await sut.GetLastExportFilePathAsync();

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

        [Test]
        public async Task ExportToExcelAsync_WithValidData_CreatesExcelFile()
        {
            // Arrange
            var options = new ExportOptions
            {
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Local),
                IncludeShifts = true,
                IncludeProcedures = true,
                IncludeInternships = true,
                FormatForOldSMK = false,
            };

            var testInternships = new List<Internship>
            {
                new Internship
                {
                    InternshipId = 1,
                    InternshipName = "Test Internship",
                    InstitutionName = "Test Hospital",
                    StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                    EndDate = new DateTime(2023, 3, 31, 0, 0, 0, DateTimeKind.Local),
                    IsCompleted = true,
                },
            };

            var testProcedures = new List<Procedure>
            {
                new Procedure
                {
                    ProcedureId = 1,
                    Code = "TEST001",
                    Date = new DateTime(2023, 2, 15, 0, 0, 0, DateTimeKind.Local),
                    Location = "Test Location",
                    OperatorCode = "A",
                },
            };

            var testShifts = new List<MedicalShift>
            {
                new MedicalShift
                {
                    ShiftId = 1,
                    Date = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Local),
                    Hours = 10,
                    Minutes = 30,
                    Location = "Test Department",
                },
            };

            this.databaseService.GetInternshipsAsync(
                Arg.Any<int?>(),
                Arg.Any<int?>())
                .Returns(testInternships);

            this.databaseService.GetProceduresAsync(
                Arg.Any<int?>(),
                Arg.Any<string>())
                .Returns(testProcedures);

            this.databaseService.GetMedicalShiftsAsync(Arg.Any<int?>())
                .Returns(testShifts);

            // Act
            var filePath = await this.exportService.ExportToExcelAsync(options);

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify that the file is a valid Excel file
            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                Assert.That(package.Workbook.Worksheets.Count, Is.GreaterThan(0));

                // Verify Summary sheet exists
                var summarySheet = package.Workbook.Worksheets["Podsumowanie"];
                Assert.That(summarySheet, Is.Not.Null);

                // Verify data sheets exist
                if (options.IncludeInternships)
                {
                    var internshipsSheet = package.Workbook.Worksheets["Staże kierunkowe"];
                    Assert.That(internshipsSheet, Is.Not.Null);
                }

                if (options.IncludeShifts)
                {
                    var shiftsSheet = package.Workbook.Worksheets["Dyżury medyczne"];
                    Assert.That(shiftsSheet, Is.Not.Null);
                }

                if (options.IncludeProcedures)
                {
                    var proceduresSheet = package.Workbook.Worksheets["Procedury"];
                    Assert.That(proceduresSheet, Is.Not.Null);
                }
            }

            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public async Task ExportToExcelAsync_WithNoSpecialization_ThrowsException()
        {
            // Arrange
            this.specializationService.GetCurrentSpecializationAsync()
                .Returns((Specialization)null);

            var options = new ExportOptions
            {
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Local),
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(
                async () => await this.exportService.ExportToExcelAsync(options));
            Assert.That(ex.Message, Is.EqualTo("Nie znaleziono aktywnej specjalizacji"));
        }

        [Test]
        public async Task ExportToExcelAsync_WithNoUser_ThrowsException()
        {
            // Arrange
            this.specializationService.GetCurrentUserAsync()
                .Returns((User)null);

            var options = new ExportOptions
            {
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Local),
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(
                async () => await this.exportService.ExportToExcelAsync(options));
            Assert.That(ex.Message, Is.EqualTo("Nie znaleziono aktywnego użytkownika"));
        }

        [Test]
        public void VerifyServicesAreCorrectlySet()
        {
            // This test verifies that our service dependency injection is working
            // Act & Assert
            var actualSecureStorage = TestServiceProvider.GetSecureStorageService();
            var actualFileSystem = TestServiceProvider.GetFileSystemService();

            Assert.That(actualSecureStorage, Is.Not.Null);
            Assert.That(actualFileSystem, Is.Not.Null);
            Assert.That(actualSecureStorage, Is.InstanceOf<TestSecureStorageService>());
            Assert.That(actualFileSystem, Is.InstanceOf<TestFileSystemService>());
        }

        [Test]
        public async Task GetLastExportFilePathAsync_WhenNoFileExported_ReturnsNull()
        {
            // Act
            var result = await this.exportService.GetLastExportFilePathAsync();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ShareExportFileAsync_WithNullPath_ReturnsFalse()
        {
            // Act
            bool result = await this.exportService.ShareExportFileAsync(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ExportToExcelAsync_WhenAllOptionsDisabled_CreatesOnlySummarySheet()
        {
            // Arrange
            var options = new ExportOptions
            {
                IncludeShifts = false,
                IncludeProcedures = false,
                IncludeInternships = false,
                IncludeCourses = false,
                IncludeSelfEducation = false,
                IncludePublications = false,
                IncludeEducationalActivities = false,
                IncludeAbsences = false,
                IncludeRecognitions = false,
                FormatForOldSMK = false,
            };

            // Act
            var filePath = await this.exportService.ExportToExcelAsync(options);

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(filePath)))
            {
                Assert.That(package.Workbook.Worksheets.Count, Is.EqualTo(1));
                Assert.That(package.Workbook.Worksheets["Podsumowanie"], Is.Not.Null);
            }

            // Cleanup
            File.Delete(filePath);
        }

        [Test]
        public async Task ExportToExcelAsync_WithExceptionDuringWrite_ThrowsException()
        {
            // Arrange
            var options = new ExportOptions
            {
                IncludeShifts = true
            };

            this.databaseService.GetInternshipsAsync(Arg.Any<int>(), Arg.Any<int?>()).Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await this.exportService.ExportToExcelAsync(options));
            Assert.That(ex.Message, Does.Contain("Database error"));
        }

        [Test]
        public async Task SaveLastExportDateAsync_WhenExceptionOccurs_DoesNotThrow()
        {
            // Arrange
            var invalidDate = DateTime.MinValue; // Potencjalny błąd

            // Act & Assert (metoda powinna obsłużyć błąd bez wyjątku)
            Assert.DoesNotThrowAsync(async () => await this.exportService.SaveLastExportDateAsync(invalidDate));
        }
    }
}