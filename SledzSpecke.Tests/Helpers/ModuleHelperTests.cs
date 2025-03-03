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
        public void CreateModulesForSpecialization_WithCardiology_CreatesTwoModules()
        {
            // Arrange
            string specializationCode = "kardiologia";
            DateTime startDate = new DateTime(2023, 1, 1);

            // Act
            List<Module> modules = ModuleHelper.CreateModulesForSpecialization(specializationCode, startDate);

            // Assert
            Assert.That(modules, Has.Count.EqualTo(2));

            // Basic module
            Assert.That(modules[0].Type, Is.EqualTo(ModuleType.Basic));
            Assert.That(modules[0].Name, Is.EqualTo("Moduł podstawowy"));
            Assert.That(modules[0].StartDate, Is.EqualTo(startDate));
            Assert.That(modules[0].EndDate, Is.EqualTo(startDate.AddYears(2)));

            // Specialistic module
            Assert.That(modules[1].Type, Is.EqualTo(ModuleType.Specialistic));
            Assert.That(modules[1].Name, Is.EqualTo("Moduł specjalistyczny"));
            Assert.That(modules[1].StartDate, Is.EqualTo(startDate.AddYears(2)));
            Assert.That(modules[1].EndDate, Is.EqualTo(startDate.AddYears(5)));
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