namespace SledzSpecke.App.Helpers
{
    public class FileAccessHelper : IFileAccessHelper
    {
        public async Task<bool> EnsureFolderExistsAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return true;
        }
    }
}