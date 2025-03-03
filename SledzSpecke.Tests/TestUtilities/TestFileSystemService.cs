﻿using SledzSpecke.App.Services.FileSystem;

namespace SledzSpecke.Tests.TestUtilities
{
    /// <summary>
    /// Test implementation of IFileSystemService for unit testing.
    /// </summary>
    public class TestFileSystemService : IFileSystemService
    {
        private string testBasePath = Path.Combine(Path.GetTempPath(), "SledzSpeckeTests");
        private readonly Dictionary<string, byte[]> inMemoryFiles = new Dictionary<string, byte[]>();
        private readonly bool useInMemoryStorage;

        public TestFileSystemService(bool useInMemoryStorage = true)
        {
            this.useInMemoryStorage = useInMemoryStorage;

            // Create test directory if using real file system
            if (!useInMemoryStorage)
            {
                Directory.CreateDirectory(this.testBasePath);
            }
        }

        public string AppDataDirectory
        {
            get
            {
                return this.testBasePath;
            }

            set
            {
                this.testBasePath = value;
            }
        }

        public string GetAppSubdirectory(string subDirectory)
        {
            string path = Path.Combine(this.AppDataDirectory, subDirectory);
            this.EnsureDirectoryExists(path);
            return path;
        }

        public bool EnsureDirectoryExists(string path)
        {
            if (this.useInMemoryStorage)
            {
                return true; // Always succeed for in-memory
            }

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
            if (this.useInMemoryStorage)
            {
                if (this.inMemoryFiles.TryGetValue(path, out byte[] data))
                {
                    return System.Text.Encoding.UTF8.GetString(data);
                }
                return null;
            }

            if (!this.FileExists(path))
            {
                return null;
            }

            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteAllTextAsync(string path, string contents)
        {
            if (this.useInMemoryStorage)
            {
                this.inMemoryFiles[path] = System.Text.Encoding.UTF8.GetBytes(contents);
                return;
            }

            this.EnsureDirectoryExists(Path.GetDirectoryName(path));
            await File.WriteAllTextAsync(path, contents);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            if (this.useInMemoryStorage)
            {
                if (this.inMemoryFiles.TryGetValue(path, out byte[] data))
                {
                    return data;
                }
                return null;
            }

            if (!this.FileExists(path))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(path);
        }

        public async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            if (this.useInMemoryStorage)
            {
                this.inMemoryFiles[path] = bytes;
                return;
            }

            this.EnsureDirectoryExists(Path.GetDirectoryName(path));
            await File.WriteAllBytesAsync(path, bytes);
        }

        public bool FileExists(string path)
        {
            if (this.useInMemoryStorage)
            {
                return this.inMemoryFiles.ContainsKey(path);
            }

            return File.Exists(path);
        }

        public bool DeleteFile(string path)
        {
            if (this.useInMemoryStorage)
            {
                return this.inMemoryFiles.Remove(path);
            }

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

        public void ClearAllFiles()
        {
            if (this.useInMemoryStorage)
            {
                this.inMemoryFiles.Clear();
            }
            else if (Directory.Exists(this.testBasePath))
            {
                try
                {
                    Directory.Delete(this.testBasePath, true);
                    Directory.CreateDirectory(this.testBasePath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error clearing test directory: {ex.Message}");
                }
            }
        }
    }
}
