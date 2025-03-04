using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.Models
{
    public class AbsenceTests
    {
        [Test]
        public void DaysCount_CalculatesCorrectly()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                StartDate = new DateTime(2023, 5, 1),
                EndDate = new DateTime(2023, 5, 10),
            };

            // Act
            int daysCount = absence.DaysCount;

            // Assert
            Assert.That(daysCount, Is.EqualTo(10)); // 10 days inclusive of start and end
        }

        [Test]
        public void ExtendsSpecialization_ForSickLeave_ReturnsTrue()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                Type = AbsenceType.Sick,
            };

            // Act
            bool extendsSpecialization = absence.ExtendsSpecialization;

            // Assert
            Assert.That(extendsSpecialization, Is.True);
        }

        [Test]
        public void ExtendsSpecialization_ForMaternityLeave_ReturnsTrue()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                Type = AbsenceType.Maternity,
            };

            // Act
            bool extendsSpecialization = absence.ExtendsSpecialization;

            // Assert
            Assert.That(extendsSpecialization, Is.True);
        }

        [Test]
        public void ExtendsSpecialization_ForPaternityLeave_ReturnsTrue()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                Type = AbsenceType.Paternity,
            };

            // Act
            bool extendsSpecialization = absence.ExtendsSpecialization;

            // Assert
            Assert.That(extendsSpecialization, Is.True);
        }

        [Test]
        public void ExtendsSpecialization_ForVacation_ReturnsFalse()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                Type = AbsenceType.Vacation,
            };

            // Act
            bool extendsSpecialization = absence.ExtendsSpecialization;

            // Assert
            Assert.That(extendsSpecialization, Is.False);
        }

        [Test]
        public void ExtendsSpecialization_ForUnpaid_ReturnsFalse()
        {
            // Arrange
            var absence = new Absence
            {
                AbsenceId = 1,
                Type = AbsenceType.Unpaid,
            };

            // Act
            bool extendsSpecialization = absence.ExtendsSpecialization;

            // Assert
            Assert.That(extendsSpecialization, Is.False);
        }
    }
}