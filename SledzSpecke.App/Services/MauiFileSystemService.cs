using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App.Services
{
    public class MauiFileSystemService : IFileSystemService
    {
        public string GetAppDataDirectory()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}
