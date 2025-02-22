using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.Services.Platform;

public class FileSystemService : IFileSystemService
{
    public string GetAppDataDirectory()
    {
        return FileSystem.AppDataDirectory;
    }
}