namespace SledzSpecke.App.Services.FileSystem
{
    public interface IFileSystemService
    {
        string AppDataDirectory { get; }

        string GetAppSubdirectory(string subDirectory);

        bool EnsureDirectoryExists(string path);

        Task<string> ReadAllTextAsync(string path);

        Task WriteAllTextAsync(string path, string contents);

        Task<byte[]> ReadAllBytesAsync(string path);

        Task WriteAllBytesAsync(string path, byte[] bytes);

        bool FileExists(string path);

        bool DeleteFile(string path);
    }
}
