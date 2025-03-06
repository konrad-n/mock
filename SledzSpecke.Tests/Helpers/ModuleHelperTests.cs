using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.TestHelpers
{
    public class ModuleHelperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsModuleSpecialization_WithCardiology_ReturnsTrue()
        {
            // Arrange
            string specializationCode = "kardiologia";

            // Act
            bool isModuleSpecialization = ModuleHelper.IsModuleSpecialization(specializationCode);

            // Assert
            Assert.That(isModuleSpecialization, Is.True);
        }

        [Test]
        public void IsModuleSpecialization_WithCardiologyMixedCase_ReturnsTrue()
        {
            // Arrange
            string specializationCode = "KaRdIoLoGiA";

            // Act
            bool isModuleSpecialization = ModuleHelper.IsModuleSpecialization(specializationCode);

            // Assert
            Assert.That(isModuleSpecialization, Is.True);
        }

        [Test]
        public void IsModuleSpecialization_WithNonModuleSpecialization_ReturnsFalse()
        {
            // Arrange
            string specializationCode = "psychiatria";

            // Act
            bool isModuleSpecialization = ModuleHelper.IsModuleSpecialization(specializationCode);

            // Assert
            Assert.That(isModuleSpecialization, Is.False);
        }

        [Test]
        public void GetBasicModuleName_WithCardiology_ReturnsInternalMedicine()
        {
            // Arrange
            string specializationCode = "kardiologia";
            string expectedModuleName = "internal_medicine";

            // Act
            string basicModuleName = ModuleHelper.GetBasicModuleName(specializationCode);

            // Assert
            Assert.That(basicModuleName, Is.EqualTo(expectedModuleName));
        }

        [Test]
        public void GetBasicModuleName_WithNonModuleSpecialization_ReturnsNull()
        {
            // Arrange
            string specializationCode = "psychiatria";

            // Act
            string basicModuleName = ModuleHelper.GetBasicModuleName(specializationCode);

            // Assert
            Assert.That(basicModuleName, Is.Null);
        }

        [Test]
        public void CreateModulesForSpecialization_WithNonModuleSpecialization_ReturnsEmptyList()
        {
            // Arrange
            string specializationCode = "psychiatria";
            DateTime startDate = new DateTime(2023, 1, 1);

            // Act
            List<Module> modules = ModuleHelper.CreateModulesForSpecialization(specializationCode, startDate);

            // Assert
            Assert.That(modules, Is.Empty);
        }

        [Test]
        public void CreateModulesForSpecialization_WithNullBasicCode_ReturnsEmptyList()
        {
            // Arrange
            // Create a mock specialization that would return null from GetBasicModuleName
            string specializationCode = "mockedSpecialization";
            DateTime startDate = new DateTime(2023, 1, 1);

            // Act
            List<Module> modules = ModuleHelper.CreateModulesForSpecialization(specializationCode, startDate);

            // Assert
            Assert.That(modules, Is.Empty);
        }
    }
}