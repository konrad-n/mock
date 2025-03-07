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
    }
}