using SledzSpecke.App.Services.FileSystem;

namespace SledzSpecke.Tests.TestHelpers
{
    /// <summary>
    /// Constants used throughout the application.
    /// </summary>
    public static class Constants
    {
        private static IFileSystemService fileSystemService;

        // Initialize with a default implementation
        static Constants()
        {
            // This will be replaced with a proper implementation during app startup
            // or with a test implementation during testing
            fileSystemService = new FileSystemService();
        }

        /// <summary>
        /// Sets the file system service to use.
        /// </summary>
        /// <param name="service">The file system service implementation.</param>
        public static void SetFileSystemService(IFileSystemService service)
        {
            fileSystemService = service ?? throw new ArgumentNullException(nameof(service));
        }

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

        public static string DatabasePath => Path.Combine(fileSystemService.AppDataDirectory, DatabaseFilename);

        public static string SpecializationTemplatesPath => fileSystemService.GetAppSubdirectory(SpecializationTemplatesFolder);

        public static string ExportsPath => fileSystemService.GetAppSubdirectory(ExportsFolder);

        public static string BackupsPath => fileSystemService.GetAppSubdirectory(BackupsFolder);

        public static string PublicationsPath => fileSystemService.GetAppSubdirectory(PublicationsFolder);
    }
}