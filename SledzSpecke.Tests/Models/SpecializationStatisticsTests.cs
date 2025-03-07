using SledzSpecke.App.Models;

namespace SledzSpecke.Tests.Models
{
    public class SpecializationStatisticsTests
    {
        [Test]
        public void GetOverallProgress_WithCompletedRequirements_ReturnsOneOrLess()
        {
            // Arrange
            var stats = new SpecializationStatistics
            {
                CompletedInternships = 15, // More than required
                RequiredInternships = 10,
                CompletedCourses = 8, // More than required
                RequiredCourses = 5,
                CompletedProceduresA = 60, // More than required
                RequiredProceduresA = 50,
                CompletedProceduresB = 30, // More than required
                RequiredProceduresB = 25,
            };

            // Act
            double progress = stats.GetOverallProgress();

            // Assert
            // Progress should be capped at 1.0 even if individual categories exceed requirements
            Assert.That(progress, Is.LessThanOrEqualTo(1.0));

            // But with the current algorithm it should be exactly 1.0
            Assert.That(progress, Is.EqualTo(1.0).Within(0.0001));
        }
    }
}