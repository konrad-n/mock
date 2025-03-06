using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class SpecializationLoader
    {
        private static readonly string ResourcePrefix = "SledzSpecke.App.Resources.Raw.SpecializationTemplates";

        /// <summary>
        /// Ładuje program specjalizacji o podanym kodzie i wersji SMK.
        /// </summary>
        /// <param name="code">Kod specjalizacji.</param>
        /// <param name="smkVersion">Wersja SMK.</param>
        /// <returns>Załadowany program specjalizacji.</returns>
        public static async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            // Konstruujemy nazwę pliku na podstawie kodu i wersji SMK
            string fileName = $"{code.ToLowerInvariant()}_{(smkVersion == SmkVersion.New ? "new" : "old")}.json";
            System.Diagnostics.Debug.WriteLine($"Próbuję załadować program specjalizacji: {fileName}");

            // Ścieżka do folderu z szablonami specjalizacji
            string templatesPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
            Directory.CreateDirectory(templatesPath);

            // Pełna ścieżka do pliku
            string filePath = Path.Combine(templatesPath, fileName);

            // Sprawdzamy, czy plik już istnieje lokalnie
            if (File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"Znaleziono lokalny plik: {filePath}");
                string json = await File.ReadAllTextAsync(filePath);
                var program = JsonSerializer.Deserialize<SpecializationProgram>(json);
                if (program != null)
                {
                    program.SmkVersion = smkVersion;
                    return program;
                }
            }

            // Jeśli nie znaleziono lokalnie, próbujemy załadować z zasobów
            System.Diagnostics.Debug.WriteLine("Próbuję załadować z zasobów wbudowanych...");

            var assembly = typeof(SpecializationLoader).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            System.Diagnostics.Debug.WriteLine($"Znalezione zasoby: {string.Join(", ", resourceNames)}");

            // Szukamy dokładnego zasobu
            string resourceName = $"{ResourcePrefix}.{fileName}";

            // Jeśli nie znaleziono dokładnego zasobu, szukamy podobnego
            if (!resourceNames.Contains(resourceName))
            {
                resourceName = resourceNames
                    .FirstOrDefault(r => r.Contains(code.ToLowerInvariant()) &&
                                        r.Contains(smkVersion == SmkVersion.New ? "new" : "old"));

                System.Diagnostics.Debug.WriteLine($"Próbuję znaleźć podobny zasób: {resourceName}");
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
                                System.Diagnostics.Debug.WriteLine($"Odczytano {json.Length} znaków z zasobu.");

                                var program = DeserializeSpecializationProgram(json);
                                if (program != null)
                                {
                                    program.SmkVersion = smkVersion;

                                    // Zapisujemy do lokalnego pliku na przyszłość
                                    try
                                    {
                                        await File.WriteAllTextAsync(filePath, json);
                                        System.Diagnostics.Debug.WriteLine($"Zapisano zasób do pliku: {filePath}");
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

            // Jeśli nic nie znaleziono, zwracamy podstawową specjalizację
            System.Diagnostics.Debug.WriteLine("Tworzę domyślny program specjalizacji");
            return new SpecializationProgram
            {
                ProgramId = 1,
                Name = code.First().ToString().ToUpperInvariant() + code.Substring(1).ToLowerInvariant().Replace("_", " "),
                Code = code,
                SmkVersion = smkVersion,
                HasModules = code.ToLowerInvariant() == "cardiology",
                BasicModuleCode = code.ToLowerInvariant() == "cardiology" ? "internal_medicine" : null,
                BasicModuleDurationMonths = code.ToLowerInvariant() == "cardiology" ? 24 : 0,
                TotalDurationMonths = 60
            };
        }

        /// <summary>
        /// Ładuje wszystkie dostępne programy specjalizacji dla określonej wersji SMK.
        /// </summary>
        /// <param name="smkVersion">Wersja SMK do filtrowania.</param>
        /// <returns>Lista programów specjalizacji pasujących do określonej wersji SMK.</returns>
        public static async Task<List<SpecializationProgram>> LoadAllSpecializationProgramsForVersionAsync(SmkVersion smkVersion)
        {
            var programs = new List<SpecializationProgram>();

            try
            {
                System.Diagnostics.Debug.WriteLine($"Ładowanie programów specjalizacji dla wersji SMK: {smkVersion}");

                // Zapewniamy, że katalog istnieje
                string targetPath = Path.Combine(FileSystem.AppDataDirectory, "SpecializationTemplates");
                Directory.CreateDirectory(targetPath);

                System.Diagnostics.Debug.WriteLine($"Katalog docelowy: {targetPath}");

                // Najpierw próbujemy załadować z katalogu aplikacji
                if (Directory.Exists(targetPath))
                {
                    var files = Directory.GetFiles(targetPath, "*.json");
                    System.Diagnostics.Debug.WriteLine($"Znaleziono {files.Length} plików JSON w katalogu.");

                    foreach (var file in files)
                    {
                        try
                        {
                            string json = await File.ReadAllTextAsync(file);
                            var program = DeserializeSpecializationProgram(json);

                            if (program != null)
                            {
                                // Nadpisujemy wersję SMK jeśli potrzebne
                                string fileName = Path.GetFileName(file);
                                if (fileName.Contains("_new") && smkVersion == SmkVersion.New)
                                {
                                    program.SmkVersion = SmkVersion.New;
                                    programs.Add(program);
                                    System.Diagnostics.Debug.WriteLine($"Dodano specjalizację z pliku (nowy SMK): {program.Name}");
                                }
                                else if (fileName.Contains("_old") && smkVersion == SmkVersion.Old)
                                {
                                    program.SmkVersion = SmkVersion.Old;
                                    programs.Add(program);
                                    System.Diagnostics.Debug.WriteLine($"Dodano specjalizację z pliku (stary SMK): {program.Name}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd ładowania pliku {file}: {ex.Message}");
                        }
                    }
                }

                // Jeśli nie załadowano programów z plików, próbujemy z zasobów wbudowanych
                if (programs.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Próbuję załadować z zasobów wbudowanych...");

                    var assembly = typeof(SpecializationLoader).Assembly;
                    var resourceNames = assembly.GetManifestResourceNames();

                    var matchingResources = resourceNames
                        .Where(r => r.StartsWith(ResourcePrefix) &&
                               r.EndsWith(".json") &&
                               r.Contains(smkVersion == SmkVersion.New ? "_new" : "_old"))
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Znaleziono pasujących zasobów: {matchingResources.Count}");
                    System.Diagnostics.Debug.WriteLine($"Zasoby: {string.Join(", ", matchingResources)}");

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
                                            // Nadpisujemy wersję SMK
                                            program.SmkVersion = smkVersion;
                                            programs.Add(program);
                                            System.Diagnostics.Debug.WriteLine($"Dodano specjalizację z zasobu: {program.Name}");

                                            // Zapisujemy do lokalnego pliku dla przyszłych użyć
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
                                                    System.Diagnostics.Debug.WriteLine($"Zapisano do pliku: {filePath}");
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

                // Jeśli wciąż brak programów, dodajemy domyślne
                if (programs.Count == 0)
                {
                    throw new InvalidDataException("Nie udało się załadować żadnych programów specjalizacji.");
                }

                System.Diagnostics.Debug.WriteLine($"Łącznie załadowano {programs.Count} programów specjalizacji");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania programów specjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            return programs;
        }

        /// <summary>
        /// Deserializuje program specjalizacji z JSON.
        /// </summary>
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

                // Jeśli deserializacja standardowa nie zadziała, próbujemy dostosowania
                if (program == null || string.IsNullOrEmpty(program.Name))
                {
                    // Deserializuj do słownika
                    var jsonObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, options);

                    if (jsonObj != null)
                    {
                        program = new SpecializationProgram();

                        if (jsonObj.TryGetValue("name", out var nameElement))
                        {
                            program.Name = nameElement.GetString();
                        }

                        if (jsonObj.TryGetValue("code", out var codeElement))
                        {
                            program.Code = codeElement.GetString();
                        }

                        if (jsonObj.TryGetValue("hasModules", out var hasModulesElement))
                        {
                            program.HasModules = hasModulesElement.GetBoolean();
                        }
                        else if (jsonObj.TryGetValue("modules", out var modulesElement) &&
                                modulesElement.ValueKind == JsonValueKind.Array &&
                                modulesElement.GetArrayLength() > 0)
                        {
                            program.HasModules = true;
                        }

                        // Pobierz czas trwania
                        if (jsonObj.TryGetValue("totalDuration", out var durationElement) &&
                            durationElement.ValueKind == JsonValueKind.Object)
                        {
                            var durationObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(durationElement.GetRawText(), options);
                            if (durationObj.TryGetValue("years", out var yearsElement))
                            {
                                program.TotalDurationMonths = yearsElement.GetInt32() * 12;
                            }
                        }
                        else if (jsonObj.TryGetValue("totalDurationMonths", out var monthsElement))
                        {
                            program.TotalDurationMonths = monthsElement.GetInt32();
                        }

                        // Pobierz dane o module podstawowym
                        if (program.HasModules && jsonObj.TryGetValue("modules", out var modules) &&
                            modules.ValueKind == JsonValueKind.Array)
                        {
                            for (int i = 0; i < modules.GetArrayLength(); i++)
                            {
                                var module = modules[i];
                                if (module.TryGetProperty("moduleType", out var typeElement) &&
                                    typeElement.GetString() == "Basic")
                                {
                                    if (module.TryGetProperty("code", out var moduleCodeElement))
                                    {
                                        program.BasicModuleCode = moduleCodeElement.GetString();
                                    }

                                    if (module.TryGetProperty("duration", out var moduleDurationElement) &&
                                        moduleDurationElement.ValueKind == JsonValueKind.Object)
                                    {
                                        var moduleDurationObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(moduleDurationElement.GetRawText(), options);
                                        if (moduleDurationObj.TryGetValue("years", out var moduleYearsElement))
                                        {
                                            program.BasicModuleDurationMonths = moduleYearsElement.GetInt32() * 12;
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                        // Zachowujemy oryginalny JSON jako Structure
                        program.Structure = json;
                    }
                }

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