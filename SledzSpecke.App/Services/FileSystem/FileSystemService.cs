namespace SledzSpecke.App.Services.FileSystem
{
    public class FileSystemService : IFileSystemService
    {
        public string AppDataDirectory => Microsoft.Maui.Storage.FileSystem.AppDataDirectory;

        public string GetAppSubdirectory(string subDirectory)
        {
            string path = Path.Combine(this.AppDataDirectory, subDirectory);
            this.EnsureDirectoryExists(path);
            return path;
        }

        public bool EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return true;
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            if (!this.FileExists(path))
            {
                return null;
            }

            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteAllTextAsync(string path, string contents)
        {
            this.EnsureDirectoryExists(Path.GetDirectoryName(path));
            await File.WriteAllTextAsync(path, contents);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            if (!this.FileExists(path))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(path);
        }

        public async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            this.EnsureDirectoryExists(Path.GetDirectoryName(path));
            await File.WriteAllBytesAsync(path, bytes);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DeleteFile(string path)
        {
            if (this.FileExists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
