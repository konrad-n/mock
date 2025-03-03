using System.Text.Json;
using NSubstitute;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.Tests.TestHelpers
{
    public class ProgressCalculatorTests
    {
        private IDatabaseService databaseService;
        private Specialization testSpecialization;
        private Module testModule;

        [SetUp]
        public void Setup()
        {
            this.databaseService = Substitute.For<IDatabaseService>();

            this.testSpecialization = new Specialization
            {
                SpecializationId = 1,
                Name = "Test Specialization",
                StartDate = new DateTime(2023, 1, 1),
                PlannedEndDate = new DateTime(2028, 1, 1),
                HasModules = false,
                CompletedInternships = 0,
                TotalInternships = 10,
                CompletedCourses = 0,
                TotalCourses = 5
            };

            this.testModule = new Module
            {
                ModuleId = 1,
                SpecializationId = 1,
                Type = ModuleType.Basic,
                Name = "Test Module",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2025, 1, 1),
                CompletedInternships = 0,
                TotalInternships = 5,
                CompletedCourses = 0,
                TotalCourses = 3,
                CompletedProceduresA = 0,
                TotalProceduresA = 20,
                CompletedProceduresB = 0,
                TotalProceduresB = 10
            };
        }

        [Test]
        public async Task UpdateModuleProgressAsync_UpdatesModuleStatistics()
        {
            // Arrange
            this.databaseService.GetModuleAsync(1).Returns(this.testModule);

            var internships = new List<Internship>
            {
                new Internship { InternshipId = 1, IsCompleted = true },
                new Internship { InternshipId = 2, IsCompleted = true },
                new Internship { InternshipId = 3, IsCompleted = false }
            };
            this.databaseService.GetInternshipsAsync(moduleId: 1).Returns(internships);

            var courses = new List<Course>
            {
                new Course { CourseId = 1 },
                new Course { CourseId = 2 }
            };
            this.databaseService.GetCoursesAsync(moduleId: 1).Returns(courses);

            var proceduresForInternship1 = new List<Procedure>
            {
                new Procedure { ProcedureId = 1, OperatorCode = "A" },
                new Procedure { ProcedureId = 2, OperatorCode = "A" },
                new Procedure { ProcedureId = 3, OperatorCode = "B" }
            };

            var proceduresForInternship2 = new List<Procedure>
            {
                new Procedure { ProcedureId = 4, OperatorCode = "A" },
                new Procedure { ProcedureId = 5, OperatorCode = "B" },
                new Procedure { ProcedureId = 6, OperatorCode = "B" }
            };

            this.databaseService.GetProceduresAsync(internshipId: 1).Returns(proceduresForInternship1);
            this.databaseService.GetProceduresAsync(internshipId: 2).Returns(proceduresForInternship2);
            this.databaseService.GetProceduresAsync(internshipId: 3).Returns(new List<Procedure>());

            // Set up moduleStructure JSON
            var moduleStructure = new ModuleStructure
            {
                ModuleName = "Test Module",
                ModuleType = ModuleType.Basic,
                DurationMonths = 24,
                Internships = new List<InternshipRequirement>
                {
                    new InternshipRequirement { InternshipCode = "1", InternshipName = "Internship 1" },
                    new InternshipRequirement { InternshipCode = "2", InternshipName = "Internship 2" },
                    new InternshipRequirement { InternshipCode = "3", InternshipName = "Internship 3" },
                    new InternshipRequirement { InternshipCode = "4", InternshipName = "Internship 4" },
                    new InternshipRequirement { InternshipCode = "5", InternshipName = "Internship 5" }
                },
                Courses = new List<CourseRequirement>
                {
                    new CourseRequirement { CourseName = "Course 1" },
                    new CourseRequirement { CourseName = "Course 2" },
                    new CourseRequirement { CourseName = "Course 3" }
                },
                Procedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement { ProcedureName = "Procedure 1", RequiredCountA = 10, RequiredCountB = 5 },
                    new ProcedureRequirement { ProcedureName = "Procedure 2", RequiredCountA = 10, RequiredCountB = 5 }
                }
            };

            this.testModule.Structure = JsonSerializer.Serialize(moduleStructure);

            // Act
            await ProgressCalculator.UpdateModuleProgressAsync(this.databaseService, 1);

            // Assert
            await this.databaseService.Received(1).UpdateModuleAsync(Arg.Any<Module>());
            await ProgressCalculator.UpdateSpecializationProgressAsync(this.databaseService, 1);

            // Verify the module was updated with correct statistics
            await this.databaseService.Received(1).UpdateModuleAsync(Arg.Is<Module>(m =>
                m.CompletedInternships == 2 &&
                m.TotalInternships == 5 &&
                m.CompletedCourses == 2 &&
                m.TotalCourses == 3 &&
                m.CompletedProceduresA == 3 &&
                m.TotalProceduresA == 20 &&
                m.CompletedProceduresB == 3 &&
                m.TotalProceduresB == 10
            ));
        }

        [Test]
        public async Task UpdateSpecializationProgressAsync_WithoutModules_UpdatesSpecializationStatistics()
        {
            // Arrange
            this.databaseService.GetSpecializationAsync(1).Returns(this.testSpecialization);

            var internships = new List<Internship>
            {
                new Internship { InternshipId = 1, IsCompleted = true },
                new Internship { InternshipId = 2, IsCompleted = true },
                new Internship { InternshipId = 3, IsCompleted = false }
            };
            this.databaseService.GetInternshipsAsync(specializationId: 1).Returns(internships);

            var courses = new List<Course>
            {
                new Course { CourseId = 1 },
                new Course { CourseId = 2 }
            };
            this.databaseService.GetCoursesAsync(specializationId: 1).Returns(courses);

            // Set up structure JSON
            var specializationStructure = new SpecializationStructure
            {
                Name = "Test Specialization",
                Code = "TEST",
                Internships = new List<InternshipRequirement>
                {
                    new InternshipRequirement { InternshipCode = "1", InternshipName = "Internship 1" },
                    new InternshipRequirement { InternshipCode = "2", InternshipName = "Internship 2" },
                    new InternshipRequirement { InternshipCode = "3", InternshipName = "Internship 3" },
                    new InternshipRequirement { InternshipCode = "4", InternshipName = "Internship 4" },
                    new InternshipRequirement { InternshipCode = "5", InternshipName = "Internship 5" },
                    new InternshipRequirement { InternshipCode = "6", InternshipName = "Internship 6" },
                    new InternshipRequirement { InternshipCode = "7", InternshipName = "Internship 7" },
                    new InternshipRequirement { InternshipCode = "8", InternshipName = "Internship 8" },
                    new InternshipRequirement { InternshipCode = "9", InternshipName = "Internship 9" },
                    new InternshipRequirement { InternshipCode = "10", InternshipName = "Internship 10" }
                },
                Courses = new List<CourseRequirement>
                {
                    new CourseRequirement { CourseName = "Course 1" },
                    new CourseRequirement { CourseName = "Course 2" },
                    new CourseRequirement { CourseName = "Course 3" },
                    new CourseRequirement { CourseName = "Course 4" },
                    new CourseRequirement { CourseName = "Course 5" }
                }
            };

            this.testSpecialization.ProgramStructure = JsonSerializer.Serialize(specializationStructure);

            // Act
            await ProgressCalculator.UpdateSpecializationProgressAsync(this.databaseService, 1);

            // Assert
            await this.databaseService.Received(1).UpdateSpecializationAsync(Arg.Any<Specialization>());

            // Verify the specialization was updated with correct statistics
            await this.databaseService.Received(1).UpdateSpecializationAsync(Arg.Is<Specialization>(s =>
                s.CompletedInternships == 2 &&
                s.TotalInternships == 10 &&
                s.CompletedCourses == 2 &&
                s.TotalCourses == 5
            ));
        }

        [Test]
        public async Task GetOverallProgressAsync_ForModule_CalculatesWeightedProgress()
        {
            // Arrange
            this.databaseService.GetModuleAsync(1).Returns(this.testModule);

            // Update test module with some progress
            this.testModule.CompletedInternships = 3;
            this.testModule.TotalInternships = 5;
            this.testModule.CompletedCourses = 2;
            this.testModule.TotalCourses = 3;
            this.testModule.CompletedProceduresA = 10;
            this.testModule.TotalProceduresA = 20;
            this.testModule.CompletedProceduresB = 5;
            this.testModule.TotalProceduresB = 10;

            // Act
            double progress = await ProgressCalculator.GetOverallProgressAsync(this.databaseService, 1, 1);

            // Assert
            // Expected calculation:
            // Internship: 3/5 = 0.6 * 0.35 = 0.21
            // Course: 2/3 = 0.66667 * 0.25 = 0.16667
            // Procedure: (10/20 + 5/10)/2 = 0.5 * 0.3 = 0.15
            // Other: 0.1
            // Total: 0.21 + 0.16667 + 0.15 + 0.1 = ~0.627
            Assert.That(progress, Is.EqualTo(0.6266666666666667).Within(0.0001));
        }

        [Test]
        public async Task GetOverallProgressAsync_WithZeroTotals_HandlesGracefully()
        {
            // Arrange
            this.databaseService.GetModuleAsync(1).Returns(this.testModule);

            // Update test module with zero totals
            this.testModule.CompletedInternships = 0;
            this.testModule.TotalInternships = 0;
            this.testModule.CompletedCourses = 0;
            this.testModule.TotalCourses = 0;
            this.testModule.CompletedProceduresA = 0;
            this.testModule.TotalProceduresA = 0;
            this.testModule.CompletedProceduresB = 0;
            this.testModule.TotalProceduresB = 0;

            // Act
            double progress = await ProgressCalculator.GetOverallProgressAsync(this.databaseService, 1, 1);

            // Assert
            // Expected calculation:
            // Internship: 0/0 = 0 * 0.35 = 0
            // Course: 0/0 = 0 * 0.25 = 0
            // Procedure: (0/0 + 0/0)/2 = 0 * 0.3 = 0
            // Other: 0.1
            // Total: 0 + 0 + 0 + 0.1 = 0.1
            Assert.That(progress, Is.EqualTo(0.1).Within(0.0001));
        }

        [Test]
        public async Task CalculateFullStatisticsAsync_ReturnsCompleteStatistics()
        {
            // Arrange
            this.databaseService.GetSpecializationAsync(1).Returns(this.testSpecialization);

            var internships = new List<Internship>
            {
                new Internship { InternshipId = 1, IsCompleted = true, DaysCount = 30 },
                new Internship { InternshipId = 2, IsCompleted = true, DaysCount = 20 }
            };
            this.databaseService.GetInternshipsAsync(specializationId: 1).Returns(internships);

            var courses = new List<Course>
            {
                new Course { CourseId = 1 },
                new Course { CourseId = 2 }
            };
            this.databaseService.GetCoursesAsync(specializationId: 1).Returns(courses);

            var procedures = new List<Procedure>
            {
                new Procedure { ProcedureId = 1, OperatorCode = "A" },
                new Procedure { ProcedureId = 2, OperatorCode = "A" },
                new Procedure { ProcedureId = 3, OperatorCode = "B" }
            };

            this.databaseService.GetProceduresAsync(internshipId: 1).Returns(procedures);
            this.databaseService.GetProceduresAsync(internshipId: 2).Returns(new List<Procedure>());

            var shifts = new List<MedicalShift>
            {
                new MedicalShift { ShiftId = 1, Hours = 10, Minutes = 30 },
                new MedicalShift { ShiftId = 2, Hours = 8, Minutes = 45 }
            };

            this.databaseService.GetMedicalShiftsAsync(1).Returns(shifts);
            this.databaseService.GetMedicalShiftsAsync(2).Returns(new List<MedicalShift>());

            var selfEducation = new List<SelfEducation>
            {
                new SelfEducation { SelfEducationId = 1 },
                new SelfEducation { SelfEducationId = 2 }
            };
            this.databaseService.GetSelfEducationItemsAsync(specializationId: 1).Returns(selfEducation);

            var educationalActivities = new List<EducationalActivity>
            {
                new EducationalActivity { ActivityId = 1 },
                new EducationalActivity { ActivityId = 2 }
            };
            this.databaseService.GetEducationalActivitiesAsync(specializationId: 1).Returns(educationalActivities);

            var publications = new List<Publication>
            {
                new Publication { PublicationId = 1 }
            };
            this.databaseService.GetPublicationsAsync(specializationId: 1).Returns(publications);

            var absences = new List<Absence>
            {
                new Absence { AbsenceId = 1, StartDate = new DateTime(2023, 3, 1), EndDate = new DateTime(2023, 3, 10), Type = AbsenceType.Sick },
                new Absence { AbsenceId = 2, StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023, 6, 5), Type = AbsenceType.Vacation }
            };
            this.databaseService.GetAbsencesAsync(1).Returns(absences);

            // Set up structure JSON with required properties
            var specializationStructure = new SpecializationStructure
            {
                Name = "Test Specialization",
                Code = "TEST",
                TotalWorkingDays = 240,
                SelfEducation = new SelfEducationInfo { TotalDays = 30 },
                MedicalShifts = new MedicalShiftsInfo { HoursPerWeek = 10.0 },
                Internships = new List<InternshipRequirement>(),
                Courses = new List<CourseRequirement>(),
                Procedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement { ProcedureName = "Procedure 1", RequiredCountA = 5, RequiredCountB = 3 }
                }
            };

            this.testSpecialization.ProgramStructure = JsonSerializer.Serialize(specializationStructure);

            // Act
            var stats = await ProgressCalculator.CalculateFullStatisticsAsync(this.databaseService, 1);

            // Assert
            Assert.That(stats, Is.Not.Null);
            Assert.That(stats.CompletedInternships, Is.EqualTo(2));
            Assert.That(stats.CompletedInternshipDays, Is.EqualTo(50));
            Assert.That(stats.RequiredInternshipDays, Is.EqualTo(240));
            Assert.That(stats.CompletedCourses, Is.EqualTo(2));
            Assert.That(stats.CompletedProceduresA, Is.EqualTo(2));
            Assert.That(stats.CompletedProceduresB, Is.EqualTo(1));
            Assert.That(stats.RequiredProceduresA, Is.EqualTo(5));
            Assert.That(stats.RequiredProceduresB, Is.EqualTo(3));
            Assert.That(stats.SelfEducationDaysUsed, Is.EqualTo(2));
            Assert.That(stats.SelfEducationDaysTotal, Is.EqualTo(30));
            Assert.That(stats.EducationalActivitiesCompleted, Is.EqualTo(2));
            Assert.That(stats.PublicationsCompleted, Is.EqualTo(1));
            Assert.That(stats.AbsenceDays, Is.EqualTo(15)); // 10 days + 5 days
            Assert.That(stats.AbsenceDaysExtendingSpecialization, Is.EqualTo(10)); // Only the first absence extends
        }
    }
}