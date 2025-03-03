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
        public string Username { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public SmkVersion SMKVersion { get; set; }

        public int SpecializationId { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
