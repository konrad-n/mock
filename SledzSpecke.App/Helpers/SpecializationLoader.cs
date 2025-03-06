using System.Text.Json;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class SpecializationLoader
    {
        public static async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            string fileName = $"{code.ToLower()}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";

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

        /// <summary>
        /// Loads all available specialization programs for the specified SMK version.
        /// </summary>
        /// <param name="smkVersion">The SMK version to filter by.</param>
        /// <returns>A list of specialization programs matching the specified SMK version.</returns>
        public static async Task<List<SpecializationProgram>> LoadAllSpecializationProgramsForVersionAsync(SmkVersion smkVersion)
        {
            var programs = new List<SpecializationProgram>();

            try
            {
                // Ensure the directory exists
                string targetPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
                Directory.CreateDirectory(targetPath);

                // First try to load from the app data directory
                if (Directory.Exists(targetPath))
                {
                    var files = Directory.GetFiles(targetPath, "*.json");
                    foreach (var file in files)
                    {
                        try
                        {
                            string json = await File.ReadAllTextAsync(file);
                            var program = JsonSerializer.Deserialize<SpecializationProgram>(json);

                            if (program != null && program.SmkVersion == smkVersion)
                            {
                                programs.Add(program);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading file {file}: {ex.Message}");
                        }
                    }
                }

                // If no programs were loaded from files, try to load from embedded resources
                if (programs.Count == 0)
                {
                    var assembly = typeof(SpecializationLoader).Assembly;
                    var resourcePrefix = "SledzSpecke.App.Resources.Raw.SpecializationTemplates.";
                    var resourceNames = assembly.GetManifestResourceNames()
                        .Where(r => r.StartsWith(resourcePrefix) && r.EndsWith(".json"))
                        .ToList();

                    foreach (var resourceName in resourceNames)
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
                                        var program = JsonSerializer.Deserialize<SpecializationProgram>(json);

                                        if (program != null)
                                        {
                                            // Override the SMK version with the requested one
                                            program.SmkVersion = smkVersion;
                                            programs.Add(program);

                                            // Save to the app data directory for future use
                                            string fileName = Path.GetFileName(resourceName);
                                            if (string.IsNullOrEmpty(fileName))
                                            {
                                                fileName = $"{program.Code?.ToLower() ?? "unknown"}.json";
                                            }

                                            string filePath = Path.Combine(targetPath, fileName);
                                            if (!File.Exists(filePath))
                                            {
                                                await File.WriteAllTextAsync(filePath, json);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading resource {resourceName}: {ex.Message}");
                        }
                    }
                }

                // If still no programs, add default ones
                if (programs.Count == 0)
                {
                    programs.AddRange(GetDefaultSpecializationPrograms(smkVersion));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading specialization programs: {ex.Message}");
                // Fallback to default programs
                programs.AddRange(GetDefaultSpecializationPrograms(smkVersion));
            }

            return programs;
        }

        /// <summary>
        /// Gets a list of default specialization programs for the specified SMK version.
        /// </summary>
        /// <param name="smkVersion">The SMK version to create programs for.</param>
        /// <returns>A list of default specialization programs.</returns>
        private static List<SpecializationProgram> GetDefaultSpecializationPrograms(SmkVersion smkVersion)
        {
            return new List<SpecializationProgram>
            {
                new SpecializationProgram
                {
                    ProgramId = 1,
                    Name = "Choroby wewnętrzne",
                    Code = "internal_medicine",
                    SmkVersion = smkVersion,
                    HasModules = false,
                    TotalDurationMonths = 48,
                },
                new SpecializationProgram
                {
                    ProgramId = 2,
                    Name = "Kardiologia",
                    Code = "cardiology",
                    SmkVersion = smkVersion,
                    HasModules = true,
                    BasicModuleCode = "internal_medicine",
                    BasicModuleDurationMonths = 24,
                    TotalDurationMonths = 60,
                },
                new SpecializationProgram
                {
                    ProgramId = 3,
                    Name = "Psychiatria",
                    Code = "psychiatry",
                    SmkVersion = smkVersion,
                    HasModules = false,
                    TotalDurationMonths = 48,
                },
                new SpecializationProgram
                {
                    ProgramId = 4,
                    Name = "Anestezjologia i intensywna terapia",
                    Code = "anesthesiology",
                    SmkVersion = smkVersion,
                    HasModules = false,
                    TotalDurationMonths = 72,
                }
            };
        }
    }
}