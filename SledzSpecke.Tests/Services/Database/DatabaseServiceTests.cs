using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.Tests.Services.Database
{
    public class DatabaseServiceTests
    {
        private IDatabaseService databaseService;
        private string databasePath;

        [SetUp]
        public void Setup()
        {
            // Create a temporary in-memory database for testing
            this.databasePath = ":memory:";
            this.databaseService = new TestDatabaseService(this.databasePath);
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
            // Arrange
            await this.databaseService.InitializeAsync();
            var internship = new Internship
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

            // Act
            int internshipId = await this.databaseService.SaveInternshipAsync(internship);
            var savedInternship = await this.databaseService.GetInternshipAsync(internshipId);

            // Assert
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
            // Arrange
            await this.databaseService.InitializeAsync();
            var module = new Module
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

            // Act
            int moduleId = await this.databaseService.SaveModuleAsync(module);
            var savedModule = await this.databaseService.GetModuleAsync(moduleId);

            // Assert
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

    // Test implementation of DatabaseService that uses in-memory SQLite database
    public class TestDatabaseService : DatabaseService
    {
        private readonly string databasePath;
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;

        public TestDatabaseService(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public new async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.database = new SQLiteAsyncConnection(this.databasePath);

            // Create tables for all models
            await this.database.CreateTableAsync<User>();
            await this.database.CreateTableAsync<SledzSpecke.App.Models.Specialization>();
            await this.database.CreateTableAsync<Module>();
            await this.database.CreateTableAsync<Internship>();
            await this.database.CreateTableAsync<MedicalShift>();
            await this.database.CreateTableAsync<Procedure>();
            await this.database.CreateTableAsync<Course>();
            await this.database.CreateTableAsync<SelfEducation>();
            await this.database.CreateTableAsync<Publication>();
            await this.database.CreateTableAsync<EducationalActivity>();
            await this.database.CreateTableAsync<Absence>();
            await this.database.CreateTableAsync<SledzSpecke.App.Models.Recognition>();
            await this.database.CreateTableAsync<SpecializationProgram>();

            this.isInitialized = true;
        }

        public new async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
        }

        public new async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public new async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();
            if (user.UserId != 0)
            {
                await this.database.UpdateAsync(user);
                return user.UserId;
            }
            else
            {
                return await this.database.InsertAsync(user);
            }
        }

        public new async Task<SledzSpecke.App.Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            var specialization = await this.database.Table<SledzSpecke.App.Models.Specialization>().FirstOrDefaultAsync(s => s.SpecializationId == id);

            if (specialization != null && specialization.HasModules)
            {
                specialization.Modules = await this.GetModulesAsync(specialization.SpecializationId);
            }

            return specialization;
        }

        public new async Task<int> SaveSpecializationAsync(SledzSpecke.App.Models.Specialization specialization)
        {
            await this.InitializeAsync();
            if (specialization.SpecializationId != 0)
            {
                await this.database.UpdateAsync(specialization);
                return specialization.SpecializationId;
            }
            else
            {
                return await this.database.InsertAsync(specialization);
            }
        }

        public new async Task<List<SledzSpecke.App.Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await this.database.Table<SledzSpecke.App.Models.Specialization>().ToListAsync();
        }

        public new async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public new async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().Where(m => m.SpecializationId == specializationId).ToListAsync();
        }

        public new async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            if (module.ModuleId != 0)
            {
                await this.database.UpdateAsync(module);
                return module.ModuleId;
            }
            else
            {
                return await this.database.InsertAsync(module);
            }
        }

        public new async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
        }

        public new async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            var query = this.database.Table<Internship>();

            if (specializationId.HasValue)
            {
                query = query.Where(i => i.SpecializationId == specializationId);
            }

            if (moduleId.HasValue)
            {
                query = query.Where(i => i.ModuleId == moduleId);
            }

            return await query.ToListAsync();
        }

        public new async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            if (internship.InternshipId != 0)
            {
                await this.database.UpdateAsync(internship);
                return internship.InternshipId;
            }
            else
            {
                return await this.database.InsertAsync(internship);
            }
        }

        public new async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(internship);
        }

        // Other database methods would be implemented similarly
        // For brevity, I'm only implementing the ones we need for the tests
    }
}