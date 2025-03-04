using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.Models
{
    public class InternshipTests
    {
        [Test]
        public void Internship_PropertiesSetCorrectly()
        {
            // Arrange
            var internship = new Internship
            {
                InternshipId = 1,
                SpecializationId = 2,
                ModuleId = 3,
                InstitutionName = "Test Institution",
                DepartmentName = "Test Department",
                InternshipName = "Test Internship",
                Year = 1,
                StartDate = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Local),
                EndDate = new DateTime(2023, 5, 31, 0, 0, 0, DateTimeKind.Local),
                DaysCount = 92,
                IsCompleted = true,
                IsApproved = true,
                IsRecognition = false,
                RecognitionReason = null,
                RecognitionDaysReduction = 0,
                SyncStatus = SyncStatus.Synced,
                AdditionalFields = "{\"field1\":\"value1\"}",
            };

            // Assert
            Assert.That(internship.InternshipId, Is.EqualTo(1));
            Assert.That(internship.SpecializationId, Is.EqualTo(2));
            Assert.That(internship.ModuleId, Is.EqualTo(3));
            Assert.That(internship.InstitutionName, Is.EqualTo("Test Institution"));
            Assert.That(internship.DepartmentName, Is.EqualTo("Test Department"));
            Assert.That(internship.InternshipName, Is.EqualTo("Test Internship"));
            Assert.That(internship.Year, Is.EqualTo(1));
            Assert.That(internship.StartDate, Is.EqualTo(new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(internship.EndDate, Is.EqualTo(new DateTime(2023, 5, 31, 0, 0, 0, DateTimeKind.Local)));
            Assert.That(internship.DaysCount, Is.EqualTo(92));
            Assert.That(internship.IsCompleted, Is.True);
            Assert.That(internship.IsApproved, Is.True);
            Assert.That(internship.IsRecognition, Is.False);
            Assert.That(internship.RecognitionReason, Is.Null);
            Assert.That(internship.RecognitionDaysReduction, Is.EqualTo(0));
            Assert.That(internship.SyncStatus, Is.EqualTo(SyncStatus.Synced));
            Assert.That(internship.AdditionalFields, Is.EqualTo("{\"field1\":\"value1\"}"));
        }
    }
}