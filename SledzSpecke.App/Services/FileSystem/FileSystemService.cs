namespace SledzSpecke.App.Services.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public string AppDataDirectory => Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
    }
}
