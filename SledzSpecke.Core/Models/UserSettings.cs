using SQLite;
using System;

namespace SledzSpecke.Core.Models
{
    [Table("UserSettings")]
    public class UserSettings
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }

        public string MedicalLicenseNumber { get; set; }

        public string TrainingUnit { get; set; }

        public string Supervisor { get; set; }

        public int CurrentSpecializationId { get; set; }

        public bool EnableNotifications { get; set; } = true;

        public bool EnableAutoSync { get; set; } = true;

        public bool UseDarkTheme { get; set; } = false;

        public DateTime LastSyncDate { get; set; } = DateTime.MinValue;
    }
}