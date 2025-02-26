using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.Services.Platform;

public class FileSystemService : IFileSystemService
{
    public string GetAppDataDirectory()
    {
        var path = FileSystem.AppDataDirectory;
        System.Diagnostics.Debug.WriteLine($"AppDataDirectory: {path}");
        return path;
    }
}