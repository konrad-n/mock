using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public SmkVersion SmkVersion { get; set; }

        public int SpecializationId { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
