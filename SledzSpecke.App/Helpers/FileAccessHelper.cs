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

        public async Task<string> SaveTextFileAsync(string folderPath, string fileName, string content)
        {
            await this.EnsureFolderExistsAsync(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllTextAsync(filePath, content);

            return filePath;
        }

        public async Task<string> ReadTextFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }

            return null;
        }

        public async Task<bool> SaveBinaryFileAsync(string folderPath, string fileName, byte[] data)
        {
            await this.EnsureFolderExistsAsync(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(filePath, data);

            return true;
        }

        public async Task<byte[]> ReadBinaryFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                return await File.ReadAllBytesAsync(filePath);
            }

            return null;
        }

        public bool DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }

        public string GetUniqueFileName(string folderPath, string baseFileName, string extension)
        {
            string fileName = $"{baseFileName}.{extension}";
            string filePath = Path.Combine(folderPath, fileName);

            int counter = 1;
            while (File.Exists(filePath))
            {
                fileName = $"{baseFileName}_{counter}.{extension}";
                filePath = Path.Combine(folderPath, fileName);
                counter++;
            }

            return fileName;
        }

        public async Task<string> PickFileAsync(string[] allowedTypes)
        {
            var options = new PickOptions
            {
                PickerTitle = "Wybierz plik",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, allowedTypes },
                    { DevicePlatform.Android, allowedTypes },
                    { DevicePlatform.WinUI, allowedTypes },
                    { DevicePlatform.MacCatalyst, allowedTypes },
                }),
            };

            var result = await FilePicker.PickAsync(options);
            return result?.FullPath;
        }

        public async Task<bool> ShareFileAsync(string filePath, string title)
        {
            if (File.Exists(filePath))
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePath),
                });

                return true;
            }

            return false;
        }
    }
}