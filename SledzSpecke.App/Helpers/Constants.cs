using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SledzSpecke.App.Helpers
{
    public static class Constants
    {

        // API
        public const string ApiUrl = "https://api.sledzspecke.app";

        // App
        public const string AppName = "ŚledzSpecke";
        public const string AppVersion = "1.0.0";

        // Settings Keys
        public const string CurrentUserIdKey = "CurrentUserId";
        public const string NotificationsEnabledKey = "NotificationsEnabled";
        public const string ShiftRemindersEnabledKey = "ShiftRemindersEnabled";
        public const string CourseRemindersEnabledKey = "CourseRemindersEnabled";
        public const string DeadlineRemindersEnabledKey = "DeadlineRemindersEnabled";
        public const string ReminderDaysInAdvanceKey = "ReminderDaysInAdvance";
        public const string LastBackupDateKey = "LastBackupDate";
        public const string LastExportDateKey = "LastExportDate";
        public const string IsDarkModeKey = "IsDarkMode";
        public const string CurrentModuleIdKey = "CurrentModuleId";

        // Date Formats
        public const string ShortDateFormat = "yyyy-MM-dd";
        public const string LongDateFormat = "dddd, dd MMMM yyyy";
        public const string TimeFormat = "HH:mm";

        // Number Formats
        public const string DecimalFormat = "0.00";
        public const string PercentFormat = "0.0%";

        // Defaults
        public const int DefaultReminderDaysInAdvance = 7;

        // Folders
        public const string SpecializationTemplatesFolder = "SpecializationTemplates";
        public const string ExportsFolder = "Exports";
        public const string BackupsFolder = "Backups";
        public const string PublicationsFolder = "Publications";

        // Database
        public const string DatabaseFilename = "sledzspecke.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static string SpecializationTemplatesPath => Path.Combine(FileSystem.AppDataDirectory, SpecializationTemplatesFolder);

        public static string ExportsPath => Path.Combine(FileSystem.AppDataDirectory, ExportsFolder);

        public static string BackupsPath => Path.Combine(FileSystem.AppDataDirectory, BackupsFolder);

        public static string PublicationsPath => Path.Combine(FileSystem.AppDataDirectory, PublicationsFolder);

    }
}
