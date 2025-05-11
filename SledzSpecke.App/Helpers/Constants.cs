using Microsoft.Maui.Controls.PlatformConfiguration;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.FileSystem;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Helpers
{
    public static class Constants
    {
        private static IFileSystemService fileSystemService;
        private static IExceptionHandlerService exceptionHandlerService;
        private static IDialogService dialogService;
        private static ILoggingService loggingService;

        static Constants()
        {
            fileSystemService = new FileSystemService();
        }

        public static void SetFileSystemService(IFileSystemService service)
        {
            fileSystemService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public const string CurrentUserIdKey = "CurrentUserId";
        public const string CurrentModuleIdKey = "CurrentModuleId";
        public const string DatabaseFilename = "sledzspecke.db3";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(fileSystemService.AppDataDirectory, DatabaseFilename);
    }
}