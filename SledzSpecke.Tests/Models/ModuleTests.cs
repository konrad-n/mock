using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.Models
{
    public class ModuleTests
    {
        [Test]
        public void Module_PropertiesSetCorrectly()
        {
            // Arrange
            var module = new Module
            {
                ModuleId = 1,
                SpecializationId = 2,
                Type = ModuleType.Basic,
                Name = "Test Module",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2025, 1, 1),
                Structure = "{\"testKey\":\"testValue\"}",
                CompletedInternships = 3,
                TotalInternships = 5,
                CompletedCourses = 2,
                TotalCourses = 4,
                CompletedProceduresA = 20,
                TotalProceduresA = 30,
                CompletedProceduresB = 10,
                TotalProceduresB = 15,
            };

            // Assert
            Assert.That(module.ModuleId, Is.EqualTo(1));
            Assert.That(module.SpecializationId, Is.EqualTo(2));
            Assert.That(module.Type, Is.EqualTo(ModuleType.Basic));
            Assert.That(module.Name, Is.EqualTo("Test Module"));
            Assert.That(module.StartDate, Is.EqualTo(new DateTime(2023, 1, 1)));
            Assert.That(module.EndDate, Is.EqualTo(new DateTime(2025, 1, 1)));
            Assert.That(module.Structure, Is.EqualTo("{\"testKey\":\"testValue\"}"));
            Assert.That(module.CompletedInternships, Is.EqualTo(3));
            Assert.That(module.TotalInternships, Is.EqualTo(5));
            Assert.That(module.CompletedCourses, Is.EqualTo(2));
            Assert.That(module.TotalCourses, Is.EqualTo(4));
            Assert.That(module.CompletedProceduresA, Is.EqualTo(20));
            Assert.That(module.TotalProceduresA, Is.EqualTo(30));
            Assert.That(module.CompletedProceduresB, Is.EqualTo(10));
            Assert.That(module.TotalProceduresB, Is.EqualTo(15));
        }
    }
}