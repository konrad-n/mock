using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.Models
{
    public class SpecializationTests
    {
        [Test]
        public void Specialization_PropertiesSetCorrectly()
        {
            // Arrange
            var specialization = new Specialization
            {
                SpecializationId = 1,
                Name = "Test Specialization",
                ProgramCode = "TEST",
                StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local),
                PlannedEndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CalculatedEndDate = new DateTime(2028, 6, 1, 0, 0, 0, DateTimeKind.Local), // With extensions
                ProgramStructure = "{\"testKey\":\"testValue\"}",
                CurrentModuleId = 2,
                Modules = new List<Module>
                {
                    new Module { ModuleId = 1, Name = "Basic Module", Type = ModuleType.Basic },
                    new Module { ModuleId = 2, Name = "Specialistic Module", Type = ModuleType.Specialistic },
                },
                CompletedInternships = 8,
                TotalInternships = 15,
                CompletedCourses = 4,
                TotalCourses = 7,
            };

            // Assert
            Assert.That(specialization.SpecializationId, Is.EqualTo(1));
            Assert.That(specialization.Name, Is.EqualTo("Test Specialization"));
            Assert.That(specialization.ProgramCode, Is.EqualTo("TEST"));
            Assert.That(specialization.StartDate, Is.EqualTo(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(specialization.PlannedEndDate, Is.EqualTo(new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(specialization.CalculatedEndDate, Is.EqualTo(new DateTime(2028, 6, 1, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(specialization.ProgramStructure, Is.EqualTo("{\"testKey\":\"testValue\"}"));
            Assert.That(specialization.CurrentModuleId, Is.EqualTo(2));
            Assert.That(specialization.Modules, Has.Count.EqualTo(2));
            Assert.That(specialization.CompletedInternships, Is.EqualTo(8));
            Assert.That(specialization.TotalInternships, Is.EqualTo(15));
            Assert.That(specialization.CompletedCourses, Is.EqualTo(4));
            Assert.That(specialization.TotalCourses, Is.EqualTo(7));
        }
    }
}