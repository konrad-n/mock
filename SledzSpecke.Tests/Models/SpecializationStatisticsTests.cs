using SledzSpecke.App.Models;

namespace SledzSpecke.Tests.Models
{
    public class SpecializationStatisticsTests
    {
        [Test]
        public void GetOverallProgress_CalculatesWeightedProgress()
        {
            // Arrange
            var stats = new SpecializationStatistics
            {
                CompletedInternships = 8,
                RequiredInternships = 10,
                CompletedCourses = 4,
                RequiredCourses = 5,
                CompletedProceduresA = 40,
                RequiredProceduresA = 50,
                CompletedProceduresB = 20,
                RequiredProceduresB = 25
            };

            // Act
            double progress = stats.GetOverallProgress();

            // Assert
            // Expected calculation:
            // Internship: 8/10 = 0.8 * 0.35 = 0.28
            // Course: 4/5 = 0.8 * 0.25 = 0.2
            // Procedure: (40/50 + 20/25) = (0.8 + 0.8)/2 = 0.8 * 0.3 = 0.24
            // Other: 0.1
            // Total: 0.28 + 0.2 + 0.24 + 0.1 = 0.82
            Assert.That(progress, Is.EqualTo(0.82).Within(0.0001));
        }

        [Test]
        public void GetOverallProgress_WithZeroRequirements_HandlesGracefully()
        {
            // Arrange
            var stats = new SpecializationStatistics
            {
                CompletedInternships = 0,
                RequiredInternships = 0,
                CompletedCourses = 0,
                RequiredCourses = 0,
                CompletedProceduresA = 0,
                RequiredProceduresA = 0,
                CompletedProceduresB = 0,
                RequiredProceduresB = 0
            };

            // Act
            double progress = stats.GetOverallProgress();

            // Assert
            // With all zeros, only the "other" weight (0.1) should be counted
            Assert.That(progress, Is.EqualTo(0.1).Within(0.0001));
        }

        [Test]
        public void GetOverallProgress_WithPartialRequirements_CalculatesCorrectly()
        {
            // Arrange
            var stats = new SpecializationStatistics
            {
                CompletedInternships = 5,
                RequiredInternships = 10,
                CompletedCourses = 0,
                RequiredCourses = 0, // No course requirements
                CompletedProceduresA = 30,
                RequiredProceduresA = 50,
                CompletedProceduresB = 15,
                RequiredProceduresB = 25
            };

            // Act
            double progress = stats.GetOverallProgress();

            // Assert
            // Expected calculation:
            // Internship: 5/10 = 0.5 * 0.35 = 0.175
            // Course: 0/0 = 0 * 0.25 = 0
            // Procedure: (30/50 + 15/25) = (0.6 + 0.6)/2 = 0.6 * 0.3 = 0.18
            // Other: 0.1
            // Total: 0.175 + 0 + 0.18 + 0.1 = 0.455
            Assert.That(progress, Is.EqualTo(0.455).Within(0.0001));
        }

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
                RequiredProceduresB = 25
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