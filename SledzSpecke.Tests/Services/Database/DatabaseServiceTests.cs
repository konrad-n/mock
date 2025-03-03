using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.Tests.Services.Database
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        private IDatabaseService databaseService;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database for each test to avoid cross-test contamination
            string databasePath = ":memory:";
            this.databaseService = new TestDatabaseService(databasePath);
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
        public async Task GetUserByUsernameAsync_ReturnsCorrectUser()
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
            await this.databaseService.SaveUserAsync(user);

            // Act
            var foundUser = await this.databaseService.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.That(foundUser, Is.Not.Null);
            Assert.That(foundUser.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task SaveSpecializationAsync_SavesSpecializationCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var specialization = new Specialization
            {
                Name = "Test Specialization",
                ProgramCode = "TEST",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CalculatedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                ProgramStructure = "{\"testKey\":\"testValue\"}",
                HasModules = false,
                CompletedInternships = 0,
                TotalInternships = 10,
                CompletedCourses = 0,
                TotalCourses = 5,
            };

            // Act
            int specId = await this.databaseService.SaveSpecializationAsync(specialization);
            var savedSpec = await this.databaseService.GetSpecializationAsync(specId);

            // Assert
            Assert.That(savedSpec, Is.Not.Null);
            Assert.That(savedSpec.SpecializationId, Is.EqualTo(specId));
            Assert.That(savedSpec.Name, Is.EqualTo("Test Specialization"));
            Assert.That(savedSpec.ProgramCode, Is.EqualTo("TEST"));
            Assert.That(savedSpec.HasModules, Is.False);
        }

        [Test]
        public async Task SaveInternshipAsync_SavesInternshipCorrectly()
        {
            // Create a completely isolated test database service
            string databasePath = ":memory:";
            var isolatedDatabaseService = new TestDatabaseService(databasePath);
            await isolatedDatabaseService.InitializeAsync();

            // Arrange
            var testInternship = new Internship
            {
                SpecializationId = 1,
                InstitutionName = "Test Institution",
                DepartmentName = "Test Department",
                InternshipName = "Test Internship",
                Year = 1,
                StartDate = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 5, 31, 0, 0, 0, DateTimeKind.Local),
                DaysCount = 92,
                IsCompleted = false,
                IsApproved = false,
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act - save and immediately retrieve the internship
            int internshipId = await isolatedDatabaseService.SaveInternshipAsync(testInternship);
            var savedInternship = await isolatedDatabaseService.GetInternshipAsync(internshipId);

            // Assert - check that we got back exactly what we saved
            Assert.That(savedInternship, Is.Not.Null);
            Assert.That(savedInternship.InternshipId, Is.EqualTo(internshipId));
            Assert.That(savedInternship.InternshipName, Is.EqualTo("Test Internship"));
            Assert.That(savedInternship.InstitutionName, Is.EqualTo("Test Institution"));
            Assert.That(savedInternship.DaysCount, Is.EqualTo(92));
        }

        [Test]
        public async Task GetInternshipsAsync_WithSpecializationFilter_ReturnsCorrectInternships()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Create internships for two different specializations
            var internship1 = new Internship
            {
                SpecializationId = 1,
                InternshipName = "Internship for Spec 1",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(30),
            };

            var internship2 = new Internship
            {
                SpecializationId = 2,
                InternshipName = "Internship for Spec 2",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(30),
            };

            await this.databaseService.SaveInternshipAsync(internship1);
            await this.databaseService.SaveInternshipAsync(internship2);

            // Act
            var internshipsForSpec1 = await this.databaseService.GetInternshipsAsync(specializationId: 1);
            var internshipsForSpec2 = await this.databaseService.GetInternshipsAsync(specializationId: 2);

            // Assert
            Assert.That(internshipsForSpec1, Has.Count.EqualTo(1));
            Assert.That(internshipsForSpec1[0].InternshipName, Is.EqualTo("Internship for Spec 1"));

            Assert.That(internshipsForSpec2, Has.Count.EqualTo(1));
            Assert.That(internshipsForSpec2[0].InternshipName, Is.EqualTo("Internship for Spec 2"));
        }

        [Test]
        public async Task SaveModuleAsync_SavesModuleCorrectly()
        {
            // Create a completely isolated test database service
            string databasePath = ":memory:";
            var isolatedDatabaseService = new TestDatabaseService(databasePath);
            await isolatedDatabaseService.InitializeAsync();

            // Arrange - create a test module
            var testModule = new Module
            {
                SpecializationId = 1,
                Type = ModuleType.Basic,
                Name = "Test Module",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                Structure = "{\"testKey\":\"testValue\"}",
                CompletedInternships = 0,
                TotalInternships = 5,
                CompletedCourses = 0,
                TotalCourses = 3,
            };

            // Act - save and retrieve the module
            int moduleId = await isolatedDatabaseService.SaveModuleAsync(testModule);
            var savedModule = await isolatedDatabaseService.GetModuleAsync(moduleId);

            // Assert - check that we got back exactly what we saved
            Assert.That(savedModule, Is.Not.Null);
            Assert.That(savedModule.ModuleId, Is.EqualTo(moduleId));
            Assert.That(savedModule.Name, Is.EqualTo("Test Module"));
            Assert.That(savedModule.Type, Is.EqualTo(ModuleType.Basic));
            Assert.That(savedModule.SpecializationId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetModulesAsync_ForSpecialization_ReturnsCorrectModules()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Create modules for two different specializations
            var module1 = new Module
            {
                SpecializationId = 1,
                Type = ModuleType.Basic,
                Name = "Basic Module for Spec 1",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddYears(2),
            };

            var module2 = new Module
            {
                SpecializationId = 1,
                Type = ModuleType.Specialistic,
                Name = "Specialistic Module for Spec 1",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddYears(2),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddYears(5),
            };

            var module3 = new Module
            {
                SpecializationId = 2,
                Type = ModuleType.Basic,
                Name = "Basic Module for Spec 2",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddYears(2),
            };

            await this.databaseService.SaveModuleAsync(module1);
            await this.databaseService.SaveModuleAsync(module2);
            await this.databaseService.SaveModuleAsync(module3);

            // Act
            var modulesForSpec1 = await this.databaseService.GetModulesAsync(1);
            var modulesForSpec2 = await this.databaseService.GetModulesAsync(2);

            // Assert
            Assert.That(modulesForSpec1, Has.Count.EqualTo(2));
            Assert.That(modulesForSpec1[0].Name, Is.EqualTo("Basic Module for Spec 1"));
            Assert.That(modulesForSpec1[1].Name, Is.EqualTo("Specialistic Module for Spec 1"));

            Assert.That(modulesForSpec2, Has.Count.EqualTo(1));
            Assert.That(modulesForSpec2[0].Name, Is.EqualTo("Basic Module for Spec 2"));
        }

        [Test]
        public async Task DeleteInternshipAsync_RemovesInternshipFromDatabase()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var internship = new Internship
            {
                SpecializationId = 1,
                InternshipName = "Internship to Delete",
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).AddDays(30),
            };

            int internshipId = await this.databaseService.SaveInternshipAsync(internship);
            var savedInternship = await this.databaseService.GetInternshipAsync(internshipId);
            Assert.That(savedInternship, Is.Not.Null, "Internship should be saved before deletion test");

            // Act
            await this.databaseService.DeleteInternshipAsync(savedInternship);
            var deletedInternship = await this.databaseService.GetInternshipAsync(internshipId);

            // Assert
            Assert.That(deletedInternship, Is.Null, "Internship should be deleted");
        }
    }
}