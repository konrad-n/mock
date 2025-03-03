namespace SledzSpecke.App.Helpers
{
    public static class FileAccessHelper
    {
        public static async Task<bool> EnsureFolderExistsAsync(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating directory: {ex.Message}");
                return false;
            }
        }

        public static async Task<string> SaveTextFileAsync(string folderPath, string fileName, string content)
        {
            try
            {
                await EnsureFolderExistsAsync(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                await File.WriteAllTextAsync(filePath, content);

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving text file: {ex.Message}");
                return null;
            }
        }

        public static async Task<string> ReadTextFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return await File.ReadAllTextAsync(filePath);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading text file: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> SaveBinaryFileAsync(string folderPath, string fileName, byte[] data)
        {
            try
            {
                await EnsureFolderExistsAsync(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                await File.WriteAllBytesAsync(filePath, data);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving binary file: {ex.Message}");
                return false;
            }
        }

        public static async Task<byte[]> ReadBinaryFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading binary file: {ex.Message}");
                return null;
            }
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
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

        public static string GetUniqueFileName(string folderPath, string baseFileName, string extension)
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

        public static async Task<string> PickFileAsync(string[] allowedTypes)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking file: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> ShareFileAsync(string filePath, string title)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sharing file: {ex.Message}");
                return false;
            }
        }
    }
}