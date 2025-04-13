using Microsoft.Maui.Controls.PlatformConfiguration;
using SledzSpecke.App.Services.FileSystem;

namespace SledzSpecke.App.Helpers
{
    public static class Constants
    {
        private static IFileSystemService fileSystemService;

        static Constants()
        {
            fileSystemService = new FileSystemService();
        }

        public static void SetFileSystemService(IFileSystemService service)
        {
            fileSystemService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public const string CurrentUserIdKey = "CurrentUserId";
        public const string CurrentUsernameKey = "CurrentUsername";
        public const string CurrentModuleIdKey = "CurrentModuleId";
        public const string DatabaseFilename = "sledzspecke.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(fileSystemService.AppDataDirectory, DatabaseFilename);
    }
}