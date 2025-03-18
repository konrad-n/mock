using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class SpecializationLoader
    {
        private static readonly string ResourcePrefix = "SledzSpecke.App.Resources.Raw.SpecializationTemplates";

        public static async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            string fileName = $"{code.ToLowerInvariant()}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";
            string templatesPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
            Directory.CreateDirectory(templatesPath);
            string filePath = Path.Combine(templatesPath, fileName);

            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                var program = DeserializeSpecializationProgram(json);

                if (program != null)
                {
                    program.SmkVersion = smkVersion;
                    return program;
                }
            }

            var assembly = typeof(SpecializationLoader).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            string resourceName = $"{ResourcePrefix}.{fileName}";

            if (!resourceNames.Contains(resourceName))
            {
                resourceName = resourceNames
                    .FirstOrDefault(r => r.Contains(code.ToLowerInvariant()) &&
                                        r.Contains(smkVersion == SmkVersion.New ? "new" : "old"));
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                try
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                string json = await reader.ReadToEndAsync();
                                var program = DeserializeSpecializationProgram(json);

                                if (program != null)
                                {
                                    program.SmkVersion = smkVersion;

                                    try
                                    {
                                        await File.WriteAllTextAsync(filePath, json);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Błąd zapisywania do pliku: {ex.Message}");
                                    }

                                    return program;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd ładowania zasobu: {ex.Message}");
                }
            }

            return new SpecializationProgram();
        }

        public static async Task<List<SpecializationProgram>> LoadAllSpecializationProgramsForVersionAsync(SmkVersion smkVersion)
        {
            var programs = new List<SpecializationProgram>();

            try
            {
                string targetPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
                Directory.CreateDirectory(targetPath);

                if (Directory.Exists(targetPath))
                {
                    var files = Directory.GetFiles(targetPath, "*.json");

                    foreach (var file in files)
                    {
                        try
                        {
                            string json = await File.ReadAllTextAsync(file);
                            var program = DeserializeSpecializationProgram(json);

                            if (program != null)
                            {
                                string fileName = Path.GetFileName(file);
                                if (fileName.Contains("_new") && smkVersion == SmkVersion.New)
                                {
                                    program.SmkVersion = SmkVersion.New;
                                    programs.Add(program);
                                }
                                else if (fileName.Contains("_old") && smkVersion == SmkVersion.Old)
                                {
                                    program.SmkVersion = SmkVersion.Old;
                                    programs.Add(program);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd ładowania pliku {file}: {ex.Message}");
                        }
                    }
                }

                if (programs.Count == 0)
                {
                    var assembly = typeof(SpecializationLoader).Assembly;
                    var resourceNames = assembly.GetManifestResourceNames();

                    var matchingResources = resourceNames
                        .Where(r => r.StartsWith(ResourcePrefix) &&
                               r.EndsWith(".json") &&
                               r.Contains(smkVersion == SmkVersion.New ? "_new" : "_old"))
                        .ToList();

                    foreach (var resourceName in matchingResources)
                    {
                        try
                        {
                            using (var stream = assembly.GetManifestResourceStream(resourceName))
                            {
                                if (stream != null)
                                {
                                    using (var reader = new StreamReader(stream))
                                    {
                                        string json = await reader.ReadToEndAsync();
                                        var program = DeserializeSpecializationProgram(json);

                                        if (program != null)
                                        {
                                            program.SmkVersion = smkVersion;
                                            programs.Add(program);
                                            string fileName = Path.GetFileName(resourceName);

                                            if (string.IsNullOrEmpty(fileName))
                                            {
                                                fileName = $"{program.Code?.ToLowerInvariant() ?? "unknown"}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";
                                            }

                                            string filePath = Path.Combine(targetPath, fileName);
                                            if (!File.Exists(filePath))
                                            {
                                                try
                                                {
                                                    await File.WriteAllTextAsync(filePath, json);
                                                }
                                                catch (Exception ex)
                                                {
                                                    System.Diagnostics.Debug.WriteLine($"Błąd zapisywania do pliku: {ex.Message}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd ładowania zasobu {resourceName}: {ex.Message}");
                        }
                    }
                }

                if (programs.Count == 0)
                {
                    throw new InvalidDataException("Nie udało się załadować żadnych programów specjalizacji.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania programów specjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            return programs;
        }

        private static SpecializationProgram DeserializeSpecializationProgram(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new JsonStringEnumConverter() }
                };

                var program = JsonSerializer.Deserialize<SpecializationProgram>(json, options);
                program.Structure = json;

                return program;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd deserializacji programu specjalizacji: {ex.Message}");
                return null;
            }
        }
    }
}