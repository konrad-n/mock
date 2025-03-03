using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.Tests.Models
{
    public class UserTests
    {
        [Test]
        public void User_PropertiesSetCorrectly()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                SmkVersion = SmkVersion.New,
                SpecializationId = 2,
                RegistrationDate = new DateTime(2023, 1, 15)
            };

            // Assert
            Assert.That(user.UserId, Is.EqualTo(1));
            Assert.That(user.Username, Is.EqualTo("testuser"));
            Assert.That(user.Email, Is.EqualTo("test@example.com"));
            Assert.That(user.PasswordHash, Is.EqualTo("hashedpassword"));
            Assert.That(user.SmkVersion, Is.EqualTo(SmkVersion.New));
            Assert.That(user.SpecializationId, Is.EqualTo(2));
            Assert.That(user.RegistrationDate, Is.EqualTo(new DateTime(2023, 1, 15)));
        }
    }
}