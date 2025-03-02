using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App.Services.Implementations
{
    public class MauiFileSystemService : IFileSystemService
    {
        public string GetAppDataDirectory()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}

