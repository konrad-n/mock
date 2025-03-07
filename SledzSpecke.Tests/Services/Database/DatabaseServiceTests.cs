using System.Reflection;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.Tests.TestUtilities;
using Module = SledzSpecke.App.Models.Module;

namespace SledzSpecke.Tests.Services.Database
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        private DatabaseService databaseService;
        private string dbPath;
        private TestFileSystemService fileSystemService;

        [SetUp]
        public void Setup()
        {
            // Create a new database file for each test in a temp directory
            string tempPath = Path.GetTempPath();
            string testDir = Path.Combine(tempPath, $"SledzSpeckeTests_{Guid.NewGuid():N}");
            Directory.CreateDirectory(testDir);
            this.dbPath = Path.Combine(testDir, "test.db3");

            // Setup test file system service
            this.fileSystemService = new TestFileSystemService(useInMemoryStorage: false)
            {
                AppDataDirectory = testDir,
            };

            // Configure the app to use our test file system service
            Constants.SetFileSystemService(this.fileSystemService);

            // Initialize real DatabaseService
            this.databaseService = new DatabaseService();
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(this.dbPath);
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
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

            // Assert - Successfully initialized and can save data
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                SmkVersion = SmkVersion.New,
                SpecializationId = 1,
                RegistrationDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
            };

            var userId = await this.databaseService.SaveUserAsync(user);
            Assert.That(userId, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetUserAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Act
            var user = await this.databaseService.GetUserAsync(999);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public async Task SaveUserAsync_SavesNewUser()
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
                RegistrationDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
            };

            // Act
            int userId = await this.databaseService.SaveUserAsync(user);

            // Assert
            Assert.That(userId, Is.GreaterThan(0));
            var savedUser = await this.databaseService.GetUserAsync(userId);
            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task SaveUserAsync_UpdatesExistingUser()
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
                RegistrationDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
            };

            int userId = await this.databaseService.SaveUserAsync(user);
            user.UserId = userId;
            user.Email = "updated@example.com";

            // Act
            await this.databaseService.SaveUserAsync(user);

            // Assert
            var updatedUser = await this.databaseService.GetUserAsync(userId);
            Assert.That(updatedUser.Email, Is.EqualTo("updated@example.com"));
        }

        [Test]
        public async Task SaveAndGetSpecialization_WorksCorrectly()
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
                CurrentModuleId = null,
                CompletedInternships = 0,
                TotalInternships = 10,
            };

            // Act
            int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);

            // Assert
            Assert.That(specializationId, Is.GreaterThan(0));
            var savedSpecialization = await this.databaseService.GetSpecializationAsync(specializationId);
            Assert.That(savedSpecialization, Is.Not.Null);
            Assert.That(savedSpecialization.Name, Is.EqualTo("Test Specialization"));
        }

        [Test]
        public async Task GetInternshipsAsync_WithFilters_ReturnsFilteredResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var internship1 = new Internship
            {
                SpecializationId = 1,
                ModuleId = 1,
                InternshipName = "Internship 1",
                InstitutionName = "Hospital 1",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 3, 31, 0, 0, 0, DateTimeKind.Local),
            };

            var internship2 = new Internship
            {
                SpecializationId = 1,
                ModuleId = 2,
                InternshipName = "Internship 2",
                InstitutionName = "Hospital 2",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 3, 31, 0, 0, 0, DateTimeKind.Local),
            };

            await this.databaseService.SaveInternshipAsync(internship1);
            await this.databaseService.SaveInternshipAsync(internship2);

            // Act - Get by moduleId
            var moduleInternships = await this.databaseService.GetInternshipsAsync(moduleId: 1);
            var specializationInternships = await this.databaseService.GetInternshipsAsync(specializationId: 1);

            // Assert
            Assert.That(moduleInternships, Has.Count.EqualTo(1));
            Assert.That(specializationInternships, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task SaveAndGetMedicalShift_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var shift = new MedicalShift
            {
                InternshipId = 1,
                Date = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                Hours = 12,
                Minutes = 30,
                Location = "Test Hospital",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int shiftId = await this.databaseService.SaveMedicalShiftAsync(shift);

            // Assert
            var savedShift = await this.databaseService.GetMedicalShiftAsync(shiftId);
            Assert.That(savedShift, Is.Not.Null);
            Assert.That(savedShift.Hours, Is.EqualTo(12));
            Assert.That(savedShift.Minutes, Is.EqualTo(30));
            Assert.That(savedShift.Location, Is.EqualTo("Test Hospital"));
        }

        [Test]
        public async Task GetMedicalShiftsAsync_WithInternshipFilter_ReturnsFilteredResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var shift1 = new MedicalShift
            {
                InternshipId = 1,
                Date = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                Hours = 12,
                Location = "Hospital 1",
            };

            var shift2 = new MedicalShift
            {
                InternshipId = 2,
                Date = new DateTime(2023, 6, 2, 0, 0, 0, DateTimeKind.Local),
                Hours = 10,
                Location = "Hospital 2",
            };

            await this.databaseService.SaveMedicalShiftAsync(shift1);
            await this.databaseService.SaveMedicalShiftAsync(shift2);

            // Act
            var internshipShifts = await this.databaseService.GetMedicalShiftsAsync(internshipId: 1);
            var allShifts = await this.databaseService.GetMedicalShiftsAsync();

            // Assert
            Assert.That(internshipShifts, Has.Count.EqualTo(1));
            Assert.That(allShifts, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task DeleteMedicalShift_RemovesShiftFromDatabase()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var shift = new MedicalShift
            {
                InternshipId = 1,
                Date = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                Hours = 12,
                Location = "Test Hospital",
            };

            int shiftId = await this.databaseService.SaveMedicalShiftAsync(shift);
            shift.ShiftId = shiftId;

            // Act
            await this.databaseService.DeleteMedicalShiftAsync(shift);

            // Assert
            var deletedShift = await this.databaseService.GetMedicalShiftAsync(shiftId);
            Assert.That(deletedShift, Is.Null);
        }

        [Test]
        public async Task SaveAndGetProcedure_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var procedure = new Procedure
            {
                InternshipId = 1,
                Date = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                Code = "TEST001",
                Location = "Test Hospital",
                PatientInitials = "JD",
                PatientGender = "M",
                OperatorCode = "A",
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int procedureId = await this.databaseService.SaveProcedureAsync(procedure);

            // Assert
            var savedProcedure = await this.databaseService.GetProcedureAsync(procedureId);
            Assert.That(savedProcedure, Is.Not.Null);
            Assert.That(savedProcedure.Code, Is.EqualTo("TEST001"));
            Assert.That(savedProcedure.OperatorCode, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetProceduresAsync_WithFiltersAndSearch_ReturnsFilteredResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var procedure1 = new Procedure
            {
                InternshipId = 1,
                Code = "TEST001",
                Location = "Hospital A",
                PatientInitials = "JD",
            };

            var procedure2 = new Procedure
            {
                InternshipId = 2,
                Code = "CARD001",
                Location = "Hospital B",
                PatientInitials = "MS",
            };

            await this.databaseService.SaveProcedureAsync(procedure1);
            await this.databaseService.SaveProcedureAsync(procedure2);

            // Act - Test internship filter
            var internshipProcedures = await this.databaseService.GetProceduresAsync(internshipId: 1);
            Assert.That(internshipProcedures, Has.Count.EqualTo(1));

            // Act - Test search
            var searchResults = await this.databaseService.GetProceduresAsync(searchText: "TEST");
            Assert.That(searchResults, Has.Count.EqualTo(1));
            Assert.That(searchResults[0].Code, Is.EqualTo("TEST001"));
        }

        [Test]
        public async Task SaveAndGetSelfEducation_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var selfEducation = new SelfEducation
            {
                SpecializationId = 1,
                Year = 1,
                Type = "Reading",
                Title = "Test Article",
                Publisher = "Test Publisher",
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int selfEducationId = await this.databaseService.SaveSelfEducationAsync(selfEducation);

            // Assert
            var savedSelfEducation = await this.databaseService.GetSelfEducationAsync(selfEducationId);
            Assert.That(savedSelfEducation, Is.Not.Null);
            Assert.That(savedSelfEducation.Title, Is.EqualTo("Test Article"));
            Assert.That(savedSelfEducation.Type, Is.EqualTo("Reading"));
        }

        [Test]
        public async Task SaveAndGetEducationalActivity_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var activity = new EducationalActivity
            {
                SpecializationId = 1,
                Type = EducationalActivityType.Conference,
                Title = "Test Conference",
                Description = "Test Description",
                StartDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 6, 3, 0, 0, 0, DateTimeKind.Local),
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int activityId = await this.databaseService.SaveEducationalActivityAsync(activity);

            // Assert
            var savedActivity = await this.databaseService.GetEducationalActivityAsync(activityId);
            Assert.That(savedActivity, Is.Not.Null);
            Assert.That(savedActivity.Title, Is.EqualTo("Test Conference"));
            Assert.That(savedActivity.Type, Is.EqualTo(EducationalActivityType.Conference));
        }

        [Test]
        public async Task SaveAndGetPublication_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var publication = new Publication
            {
                SpecializationId = 1,
                Description = "Test Publication",
                FilePath = "/test/path/publication.pdf",
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int publicationId = await this.databaseService.SavePublicationAsync(publication);

            // Assert
            var savedPublication = await this.databaseService.GetPublicationAsync(publicationId);
            Assert.That(savedPublication, Is.Not.Null);
            Assert.That(savedPublication.Description, Is.EqualTo("Test Publication"));
            Assert.That(savedPublication.FilePath, Is.EqualTo("/test/path/publication.pdf"));
        }

        [Test]
        public async Task GetPublicationsAsync_WithFilters_ReturnsFilteredResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var publication1 = new Publication
            {
                SpecializationId = 1,
                ModuleId = 1,
                Description = "Publication 1",
            };

            var publication2 = new Publication
            {
                SpecializationId = 1,
                ModuleId = 2,
                Description = "Publication 2",
            };

            await this.databaseService.SavePublicationAsync(publication1);
            await this.databaseService.SavePublicationAsync(publication2);

            // Act
            var modulePublications = await this.databaseService.GetPublicationsAsync(moduleId: 1);
            var allPublications = await this.databaseService.GetPublicationsAsync(specializationId: 1);

            // Assert
            Assert.That(modulePublications, Has.Count.EqualTo(1));
            Assert.That(allPublications, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task SaveAndGetAbsence_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var absence = new Absence
            {
                SpecializationId = 1,
                Type = AbsenceType.Sick,
                StartDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 6, 7, 0, 0, 0, DateTimeKind.Local),
                Description = "Test Absence",
                SyncStatus = SyncStatus.NotSynced,
            };

            // Act
            int absenceId = await this.databaseService.SaveAbsenceAsync(absence);

            // Assert
            var savedAbsence = await this.databaseService.GetAbsenceAsync(absenceId);
            Assert.That(savedAbsence, Is.Not.Null);
            Assert.That(savedAbsence.Type, Is.EqualTo(AbsenceType.Sick));
            Assert.That(savedAbsence.Description, Is.EqualTo("Test Absence"));
        }

        [Test]
        public async Task GetAbsencesAsync_ForSpecialization_ReturnsCorrectResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var absence1 = new Absence
            {
                SpecializationId = 1,
                Type = AbsenceType.Sick,
                StartDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 6, 7, 0, 0, 0, DateTimeKind.Local),
            };

            var absence2 = new Absence
            {
                SpecializationId = 2,
                Type = AbsenceType.Vacation,
                StartDate = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 7, 7, 0, 0, 0, DateTimeKind.Local),
            };

            await this.databaseService.SaveAbsenceAsync(absence1);
            await this.databaseService.SaveAbsenceAsync(absence2);

            // Act
            var absences = await this.databaseService.GetAbsencesAsync(1);

            // Assert
            Assert.That(absences, Has.Count.EqualTo(1));
            Assert.That(absences[0].SpecializationId, Is.EqualTo(1));
        }

        [Test]
        public async Task SaveAndGetSpecializationProgram_WorksCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var program = new SpecializationProgram
            {
                Name = "Test Program",
                Code = "TEST",
                Structure = "{\"testKey\":\"testValue\"}",
                SmkVersion = SmkVersion.New,
                BasicModuleCode = "TEST_BASIC",
                BasicModuleDurationMonths = 24,
                TotalDurationMonths = 60,
            };

            // Act
            int programId = await this.databaseService.SaveSpecializationProgramAsync(program);

            // Assert
            var savedProgram = await this.databaseService.GetSpecializationProgramAsync(programId);
            Assert.That(savedProgram, Is.Not.Null);
            Assert.That(savedProgram.Name, Is.EqualTo("Test Program"));
            Assert.That(savedProgram.Code, Is.EqualTo("TEST"));
        }

        [Test]
        public async Task GetSpecializationProgramByCodeAsync_ReturnsCorrectProgram()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var program = new SpecializationProgram
            {
                Name = "Test Program",
                Code = "TEST",
                SmkVersion = SmkVersion.New,
            };

            await this.databaseService.SaveSpecializationProgramAsync(program);

            // Act
            var foundProgram = await this.databaseService.GetSpecializationProgramByCodeAsync("TEST", SmkVersion.New);

            // Assert
            Assert.That(foundProgram, Is.Not.Null);
            Assert.That(foundProgram.Code, Is.EqualTo("TEST"));
            Assert.That(foundProgram.SmkVersion, Is.EqualTo(SmkVersion.New));
        }

        [Test]
        public async Task GetProceduresAsync_WithNullSearch_ReturnsAllProcedures()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var procedure1 = new Procedure
            {
                InternshipId = 1,
                Code = "TEST001",
                Location = "Hospital A",
            };

            var procedure2 = new Procedure
            {
                InternshipId = 1,
                Code = "TEST002",
                Location = "Hospital B",
            };

            await this.databaseService.SaveProcedureAsync(procedure1);
            await this.databaseService.SaveProcedureAsync(procedure2);

            // Act
            var procedures = await this.databaseService.GetProceduresAsync(internshipId: 1, searchText: null);

            // Assert
            Assert.That(procedures, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetProceduresAsync_SearchWithNullFields_HandlesNullsGracefully()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var procedure = new Procedure
            {
                InternshipId = 1,
                Code = "TEST001",
                Location = "Hospital A",
                PatientInitials = null,
                ProcedureGroup = null,
            };

            await this.databaseService.SaveProcedureAsync(procedure);

            // Act
            var procedures = await this.databaseService.GetProceduresAsync(searchText: "TEST");

            // Assert
            Assert.That(procedures, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task UpdateModule_WithExistingModule_UpdatesCorrectly()
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
            };

            int moduleId = await this.databaseService.SaveModuleAsync(module);
            module.ModuleId = moduleId;
            module.Name = "Updated Module";

            // Act
            await this.databaseService.UpdateModuleAsync(module);

            // Assert
            var updatedModule = await this.databaseService.GetModuleAsync(moduleId);
            Assert.That(updatedModule.Name, Is.EqualTo("Updated Module"));
        }

        [Test]
        public async Task GetCoursesAsync_WithBothFilters_ReturnsCorrectlyCombinedResults()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var course1 = new Course
            {
                SpecializationId = 1,
                ModuleId = 1,
                CourseName = "Course 1",
            };

            var course2 = new Course
            {
                SpecializationId = 1,
                ModuleId = 2,
                CourseName = "Course 2",
            };

            var course3 = new Course
            {
                SpecializationId = 2,
                ModuleId = 1,
                CourseName = "Course 3",
            };

            await this.databaseService.SaveCourseAsync(course1);
            await this.databaseService.SaveCourseAsync(course2);
            await this.databaseService.SaveCourseAsync(course3);

            // Act
            var filteredCourses = await this.databaseService.GetCoursesAsync(specializationId: 1, moduleId: 1);

            // Assert
            Assert.That(filteredCourses, Has.Count.EqualTo(1));
            Assert.That(filteredCourses[0].CourseName, Is.EqualTo("Course 1"));
        }

        [Test]
        public async Task GetEducationalActivitiesAsync_WithModuleFilter_ReturnsCorrectActivities()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var activity1 = new EducationalActivity
            {
                SpecializationId = 1,
                ModuleId = 1,
                Type = EducationalActivityType.Conference,
                Title = "Activity 1",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Local),
            };

            var activity2 = new EducationalActivity
            {
                SpecializationId = 1,
                ModuleId = 2,
                Type = EducationalActivityType.Workshop,
                Title = "Activity 2",
                StartDate = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 2, 2, 0, 0, 0, DateTimeKind.Local),
            };

            await this.databaseService.SaveEducationalActivityAsync(activity1);
            await this.databaseService.SaveEducationalActivityAsync(activity2);

            // Act
            var filteredActivities = await this.databaseService.GetEducationalActivitiesAsync(specializationId: 1, moduleId: 1);

            // Assert
            Assert.That(filteredActivities, Has.Count.EqualTo(1));
            Assert.That(filteredActivities[0].Title, Is.EqualTo("Activity 1"));
        }

        [Test]
        public async Task SaveUser_UpdateExistingUser_UpdatesAllFields()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash1",
                SmkVersion = SmkVersion.New,
                SpecializationId = 1,
                RegistrationDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
            };

            int userId = await this.databaseService.SaveUserAsync(user);
            user.UserId = userId;

            // Update all fields
            user.Username = "updateduser";
            user.Email = "updated@example.com";
            user.PasswordHash = "hash2";
            user.SmkVersion = SmkVersion.Old;
            user.SpecializationId = 2;

            // Act
            await this.databaseService.SaveUserAsync(user);

            // Assert
            var updatedUser = await this.databaseService.GetUserAsync(userId);
            Assert.That(updatedUser.Username, Is.EqualTo("updateduser"));
            Assert.That(updatedUser.Email, Is.EqualTo("updated@example.com"));
            Assert.That(updatedUser.PasswordHash, Is.EqualTo("hash2"));
            Assert.That(updatedUser.SmkVersion, Is.EqualTo(SmkVersion.Old));
            Assert.That(updatedUser.SpecializationId, Is.EqualTo(2));
        }

        [Test]
        public async Task SaveSpecialization_UpdateExisting_WithModules_UpdatesCorrectly()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var specialization = new Specialization
            {
                Name = "Test Specialization",
                ProgramCode = "TEST",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                Modules = new List<Module>
        {
            new Module
            {
                Type = ModuleType.Basic,
                Name = "Basic Module",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
            },
        },
            };

            int specId = await this.databaseService.SaveSpecializationAsync(specialization);
            specialization.SpecializationId = specId;
            specialization.Name = "Updated Specialization";
            specialization.ProgramCode = "UPDATED";

            // Act
            await this.databaseService.UpdateSpecializationAsync(specialization);

            // Assert
            var updatedSpec = await this.databaseService.GetSpecializationAsync(specId);
            Assert.That(updatedSpec.Name, Is.EqualTo("Updated Specialization"));
            Assert.That(updatedSpec.ProgramCode, Is.EqualTo("UPDATED"));
        }

        [Test]
        public async Task GetAllSpecializationProgramsAsync_WithMultiplePrograms_ReturnsAllPrograms()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var programs = new List<SpecializationProgram>
    {
        new SpecializationProgram
        {
            Name = "Program 1",
            Code = "CODE1",
            SmkVersion = SmkVersion.New,
        },
        new SpecializationProgram
        {
            Name = "Program 2",
            Code = "CODE2",
            SmkVersion = SmkVersion.Old,
        },
        new SpecializationProgram
        {
            Name = "Program 3",
            Code = "CODE3",
            SmkVersion = SmkVersion.New,
        },
    };

            foreach (var program in programs)
            {
                await this.databaseService.SaveSpecializationProgramAsync(program);
            }

            // Act
            var allPrograms = await this.databaseService.GetAllSpecializationProgramsAsync();

            // Assert
            Assert.That(allPrograms, Has.Count.EqualTo(3));
            Assert.That(allPrograms.Select(p => p.Code), Does.Contain("CODE1"));
            Assert.That(allPrograms.Select(p => p.Code), Does.Contain("CODE2"));
            Assert.That(allPrograms.Select(p => p.Code), Does.Contain("CODE3"));
        }

        [Test]
        public async Task GetSpecializationProgramByCodeAsync_WithNonexistentCode_ReturnsNull()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Act
            var program = await this.databaseService.GetSpecializationProgramByCodeAsync(
                "NONEXISTENT",
                SmkVersion.New);

            // Assert
            Assert.That(program, Is.Null);
        }

        [Test]
        public async Task SaveMedicalShift_UpdateExisting_UpdatesAllFields()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var shift = new MedicalShift
            {
                InternshipId = 1,
                Date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                Hours = 10,
                Minutes = 30,
                Location = "Initial Location",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
            };

            int shiftId = await this.databaseService.SaveMedicalShiftAsync(shift);
            shift.ShiftId = shiftId;

            // Update all fields
            shift.InternshipId = 2;
            shift.Date = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Local);
            shift.Hours = 12;
            shift.Minutes = 45;
            shift.Location = "Updated Location";
            shift.Year = 2;
            shift.SyncStatus = SyncStatus.Modified;

            // Act
            await this.databaseService.SaveMedicalShiftAsync(shift);

            // Assert
            var updatedShift = await this.databaseService.GetMedicalShiftAsync(shiftId);
            Assert.That(updatedShift.InternshipId, Is.EqualTo(2));
            Assert.That(updatedShift.Hours, Is.EqualTo(12));
            Assert.That(updatedShift.Minutes, Is.EqualTo(45));
            Assert.That(updatedShift.Location, Is.EqualTo("Updated Location"));
            Assert.That(updatedShift.Year, Is.EqualTo(2));
            Assert.That(updatedShift.SyncStatus, Is.EqualTo(SyncStatus.Modified));
        }

        [Test]
        public async Task GetUserByUsernameAsync_UserExists_ReturnsUser()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var user = new User { Username = "testuser", Email = "test@example.com" };
            await this.databaseService.SaveUserAsync(user);

            // Act
            var result = await this.databaseService.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetUserByUsernameAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Act
            var result = await this.databaseService.GetUserByUsernameAsync("nonexistent");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllSpecializationsAsync_ReturnsAllSpecializations()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            await this.databaseService.SaveSpecializationAsync(new Specialization { Name = "Spec1" });
            await this.databaseService.SaveSpecializationAsync(new Specialization { Name = "Spec2" });

            // Act
            var result = await this.databaseService.GetAllSpecializationsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetInternshipAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            await this.databaseService.InitializeAsync();

            // Act
            var result = await this.databaseService.GetInternshipAsync(99);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSelfEducationItemsAsync_WithSpecializationFilter_ReturnsFilteredItems()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var item1 = new SelfEducation { SpecializationId = 1, Title = "Item1" };
            var item2 = new SelfEducation { SpecializationId = 2, Title = "Item2" };
            await this.databaseService.SaveSelfEducationAsync(item1);
            await this.databaseService.SaveSelfEducationAsync(item2);

            // Act
            var result = await this.databaseService.GetSelfEducationItemsAsync(specializationId: 1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Item1"));
        }

        [Test]
        public async Task DeleteSelfEducationAsync_ExistingItem_RemovesItem()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var item = new SelfEducation { SpecializationId = 1, Title = "Item1" };
            await this.databaseService.SaveSelfEducationAsync(item);

            // Act
            var deleteResult = await this.databaseService.DeleteSelfEducationAsync(item);
            var checkResult = await this.databaseService.GetSelfEducationItemsAsync(specializationId: 1);

            // Assert
            Assert.That(deleteResult, Is.EqualTo(1));
            Assert.That(checkResult.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteProcedureAsync_ExistingProcedure_RemovesProcedure()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var procedure = new Procedure { InternshipId = 1, Code = "TEST001" };
            await this.databaseService.SaveProcedureAsync(procedure);

            // Act
            var deleteResult = await this.databaseService.DeleteProcedureAsync(procedure);
            var checkResult = await this.databaseService.GetProceduresAsync();

            // Assert
            Assert.That(deleteResult, Is.EqualTo(1));
            Assert.That(checkResult, Is.Empty);
        }

        [Test]
        public async Task DeleteEducationalActivityAsync_ExistingActivity_RemovesActivity()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var activity = new EducationalActivity { SpecializationId = 1, Title = "Test Activity" };
            await this.databaseService.SaveEducationalActivityAsync(activity);

            // Act
            var deleteResult = await this.databaseService.DeleteEducationalActivityAsync(activity);
            var checkResult = await this.databaseService.GetEducationalActivitiesAsync(specializationId: 1);

            // Assert
            Assert.That(deleteResult, Is.EqualTo(1));
            Assert.That(checkResult, Is.Empty);
        }

        [Test]
        public async Task DeletePublicationAsync_ExistingPublication_RemovesPublication()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var publication = new Publication { SpecializationId = 1, Description = "Test Publication" };
            await this.databaseService.SavePublicationAsync(publication);

            // Act
            var deleteResult = await this.databaseService.DeletePublicationAsync(publication);
            var checkResult = await this.databaseService.GetPublicationsAsync(specializationId: 1);

            // Assert
            Assert.That(deleteResult, Is.EqualTo(1));
            Assert.That(checkResult, Is.Empty);
        }

        [Test]
        public async Task DeleteAbsenceAsync_ExistingAbsence_RemovesAbsence()
        {
            // Arrange
            await this.databaseService.InitializeAsync();
            var absence = new Absence { SpecializationId = 1, Description = "Test Absence" };
            await this.databaseService.SaveAbsenceAsync(absence);

            // Act
            var deleteResult = await this.databaseService.DeleteAbsenceAsync(absence);
            var checkResult = await this.databaseService.GetAbsencesAsync(1);

            // Assert
            Assert.That(deleteResult, Is.EqualTo(1));
            Assert.That(checkResult, Is.Empty);
        }
    }
}