using System.Text.Json;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class SpecializationLoader
    {

        public static async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            string fileName = $"{code.ToLower()}.json";

            if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates", fileName)))
            {
                // Kopiowanie pliku z zasobów do katalogu aplikacji, jeśli jeszcze nie istnieje
                string resourcePath = $"SledzSpecke.Resources.Raw.SpecializationTemplates.{fileName}";
                using var stream = await FileSystem.OpenAppPackageFileAsync(resourcePath);

                if (stream == null)
                {
                    throw new FileNotFoundException($"Program specjalizacji o kodzie {code} nie został znaleziony");
                }

                string targetPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
                Directory.CreateDirectory(targetPath);

                using var fileStream = File.Create(Path.Combine(targetPath, fileName));
                await stream.CopyToAsync(fileStream);
            }

            // Wczytanie pliku z dysku
            string json = File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates", fileName));

            // Deserializacja JSON do modelu programu specjalizacji
            var program = JsonSerializer.Deserialize<SpecializationProgram>(json);
            program!.SmkVersion = smkVersion;

            return program;
        }
    }
}
