using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.TestHelpers
{
    public class DateCalculatorTests
    {
        private DateTime startDate;
        private int durationDays;
        private List<Absence> absences;

        [SetUp]
        public void Setup()
        {
            this.startDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local);
            this.durationDays = 365; // 1 year
            this.absences = new List<Absence>();
        }

        [Test]
        public void CalculateSpecializationEndDate_NoAbsences_ReturnsPlannedEndDate()
        {
            // Arrange
            // Use the startDate and durationDays from Setup

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            Assert.That(calculatedEndDate, Is.EqualTo(this.startDate.AddDays(this.durationDays - 1)));
        }

        [Test]
        public void CalculateSpecializationEndDate_WithSickLeave_ExtendsEndDate()
        {
            // Arrange
            var sickLeave = new Absence
            {
                Type = AbsenceType.Sick,
                StartDate = this.startDate.AddDays(100),
                EndDate = this.startDate.AddDays(109)
            };
            this.absences.Add(sickLeave);

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            // 10 days of sick leave should extend the end date by 10 days
            DateTime expectedEndDate = this.startDate.AddDays(this.durationDays - 1 + 10);
            Assert.That(calculatedEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void CalculateSpecializationEndDate_WithMaternityLeave_ExtendsEndDate()
        {
            // Arrange
            var maternityLeave = new Absence
            {
                Type = AbsenceType.Maternity,
                StartDate = this.startDate.AddDays(100),
                EndDate = this.startDate.AddDays(279), // 180 days
            };
            this.absences.Add(maternityLeave);

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            // 180 days of maternity leave should extend the end date by 180 days
            DateTime expectedEndDate = this.startDate.AddDays(this.durationDays - 1 + 180);
            Assert.That(calculatedEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void CalculateSpecializationEndDate_WithVacation_DoesNotExtendEndDate()
        {
            // Arrange
            var vacation = new Absence
            {
                Type = AbsenceType.Vacation,
                StartDate = this.startDate.AddDays(100),
                EndDate = this.startDate.AddDays(109)
            };
            this.absences.Add(vacation);

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            // Vacation should not extend the end date
            DateTime expectedEndDate = this.startDate.AddDays(this.durationDays - 1);
            Assert.That(calculatedEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void CalculateSpecializationEndDate_WithRecognition_ReducesEndDate()
        {
            // Arrange
            var recognition = new Absence
            {
                Type = AbsenceType.Recognition,
                StartDate = this.startDate.AddDays(100),
                EndDate = this.startDate.AddDays(109)
            };
            this.absences.Add(recognition);

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            // 10 days of recognition should reduce the end date by 10 days
            DateTime expectedEndDate = this.startDate.AddDays(this.durationDays - 1 - 10);
            Assert.That(calculatedEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void CalculateSpecializationEndDate_WithMultipleAbsences_CalculatesCorrectly()
        {
            // Arrange
            var sickLeave = new Absence
            {
                Type = AbsenceType.Sick,
                StartDate = this.startDate.AddDays(50),
                EndDate = this.startDate.AddDays(59)
            };

            var maternityLeave = new Absence
            {
                Type = AbsenceType.Maternity,
                StartDate = this.startDate.AddDays(150),
                EndDate = this.startDate.AddDays(179)
            };

            var recognition = new Absence
            {
                Type = AbsenceType.Recognition,
                StartDate = this.startDate.AddDays(250),
                EndDate = this.startDate.AddDays(254)
            };

            var vacation = new Absence
            {
                Type = AbsenceType.Vacation,
                StartDate = this.startDate.AddDays(300),
                EndDate = this.startDate.AddDays(309)
            };

            this.absences.Add(sickLeave);
            this.absences.Add(maternityLeave);
            this.absences.Add(recognition);
            this.absences.Add(vacation);

            // Act
            DateTime calculatedEndDate = DateCalculator.CalculateSpecializationEndDate(this.startDate, this.durationDays, this.absences);

            // Assert
            // 10 days sick + 30 days maternity - 5 days recognition = +35 days net
            DateTime expectedEndDate = this.startDate.AddDays(this.durationDays - 1 + 10 + 30 - 5);
            Assert.That(calculatedEndDate, Is.EqualTo(expectedEndDate));
        }

        [Test]
        public void CalculateWorkingDays_WeekdaysOnly_ReturnsCorrectCount()
        {
            // Arrange
            // Monday to Friday (5 working days)
            DateTime startDate = new DateTime(2023, 1, 2); // Monday
            DateTime endDate = new DateTime(2023, 1, 6);   // Friday

            // Act
            int workingDays = DateCalculator.CalculateWorkingDays(startDate, endDate);

            // Assert
            Assert.That(workingDays, Is.EqualTo(5));
        }

        [Test]
        public void CalculateWorkingDays_IncludesWeekend_ExcludesSaturdayAndSunday()
        {
            // Arrange
            // Monday to Monday (6 working days, excluding weekend)
            DateTime startDate = new DateTime(2023, 1, 2); // Monday
            DateTime endDate = new DateTime(2023, 1, 9);   // Next Monday

            // Act
            int workingDays = DateCalculator.CalculateWorkingDays(startDate, endDate);

            // Assert
            Assert.That(workingDays, Is.EqualTo(6)); // 5 weekdays + Monday
        }

        [Test]
        public void CalculateWorkingDays_SingleDay_ReturnsOne()
        {
            // Arrange
            DateTime startDate = new DateTime(2023, 1, 2); // Monday
            DateTime endDate = new DateTime(2023, 1, 2);   // Same day

            // Act
            int workingDays = DateCalculator.CalculateWorkingDays(startDate, endDate);

            // Assert
            Assert.That(workingDays, Is.EqualTo(1));
        }

        [Test]
        public void CalculateWorkingDays_Weekend_ReturnsZero()
        {
            // Arrange
            DateTime startDate = new DateTime(2023, 1, 7); // Saturday
            DateTime endDate = new DateTime(2023, 1, 8);   // Sunday

            // Act
            int workingDays = DateCalculator.CalculateWorkingDays(startDate, endDate);

            // Assert
            Assert.That(workingDays, Is.EqualTo(0));
        }
    }
}