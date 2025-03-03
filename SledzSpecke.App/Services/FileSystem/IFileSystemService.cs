namespace SledzSpecke.App.Services.FileSystem
{
    /// <summary>
    /// Interface for file system operations, allowing for better testability.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Gets the application data directory path.
        /// </summary>
        string AppDataDirectory { get; }

        /// <summary>
        /// Gets a path to a specific app subdirectory.
        /// </summary>
        /// <param name="subDirectory">The subdirectory name.</param>
        /// <returns>The full path to the subdirectory.</returns>
        string GetAppSubdirectory(string subDirectory);

        /// <summary>
        /// Ensures a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The directory path to check/create.</param>
        /// <returns>True if the directory exists or was created, false otherwise.</returns>
        bool EnsureDirectoryExists(string path);

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file contents as a string.</returns>
        Task<string> ReadAllTextAsync(string path);

        /// <summary>
        /// Writes all text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="contents">The text to write.</param>
        Task WriteAllTextAsync(string path, string contents);

        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file contents as a byte array.</returns>
        Task<byte[]> ReadAllBytesAsync(string path);

        /// <summary>
        /// Writes all bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="bytes">The bytes to write.</param>
        Task WriteAllBytesAsync(string path, byte[] bytes);

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Deletes a file at the specified path.
        /// </summary>
        /// <param name="path">The file path to delete.</param>
        /// <returns>True if the file was deleted, false otherwise.</returns>
        bool DeleteFile(string path);
    }
}
