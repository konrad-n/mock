
namespace SledzSpecke.App.Helpers
{
    public interface IFileAccessHelper
    {
        public bool DeleteFile(string filePath);

        public Task<bool> EnsureFolderExistsAsync(string folderPath);

        public string GetUniqueFileName(string folderPath, string baseFileName, string extension);

        public Task<string> PickFileAsync(string[] allowedTypes);

        public Task<byte[]> ReadBinaryFileAsync(string filePath);

        public Task<string> ReadTextFileAsync(string filePath);

        public Task<bool> SaveBinaryFileAsync(string folderPath, string fileName, byte[] data);

        public Task<string> SaveTextFileAsync(string folderPath, string fileName, string content);

        public Task<bool> ShareFileAsync(string filePath, string title);
    }
}