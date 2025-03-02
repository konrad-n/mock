using System;
using SQLite;

namespace SledzSpecke.Core.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        [Indexed]
        public string Username { get; set; }

        [MaxLength(100)]
        [Indexed]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public int SpecializationTypeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
