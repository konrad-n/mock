namespace SledzSpecke.App.Services.FileSystem
{
    /// <summary>
    /// Implementation of IFileSystemService that uses the actual file system.
    /// </summary>
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
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating directory: {ex.Message}");
                return false;
            }
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
            try
            {
                if (this.FileExists(path))
                {
                    File.Delete(path);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
    }
}
