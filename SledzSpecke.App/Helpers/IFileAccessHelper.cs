
namespace SledzSpecke.App.Helpers
{
    public interface IFileAccessHelper
    {
        public Task<bool> EnsureFolderExistsAsync(string folderPath);
    }
}