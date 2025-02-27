using System.Text.Json;

namespace SledzSpecke.Services
{
    public class AppSettings
    {
        private static readonly string _appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string _settingsFile = Path.Combine(_appDataFolder, "settings.json");
        private Dictionary<string, object> _settings;

        public AppSettings()
        {
            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }

            _settings = new Dictionary<string, object>();
        }

        public async Task LoadAsync()
        {
            if (File.Exists(_settingsFile))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(_settingsFile);
                    _settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading settings: {ex.Message}");
                    _settings = GetDefaultSettings();
                }
            }
            else
            {
                _settings = GetDefaultSettings();
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                string json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_settingsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
                throw;
            }
        }

        public T GetSetting<T>(string key, T defaultValue = default)
        {
            if (_settings.TryGetValue(key, out object value))
            {
                try
                {
                    if (value is JsonElement element)
                    {
                        switch (element.ValueKind)
                        {
                            case JsonValueKind.String:
                                if (typeof(T) == typeof(string))
                                    return (T)(object)element.GetString();
                                else if (typeof(T) == typeof(DateTime))
                                    return (T)(object)DateTime.Parse(element.GetString());
                                break;
                            case JsonValueKind.Number:
                                if (typeof(T) == typeof(int))
                                    return (T)(object)element.GetInt32();
                                else if (typeof(T) == typeof(double))
                                    return (T)(object)element.GetDouble();
                                break;
                            case JsonValueKind.True:
                            case JsonValueKind.False:
                                if (typeof(T) == typeof(bool))
                                    return (T)(object)element.GetBoolean();
                                break;
                        }
                    }
                    else if (value is T typedValue)
                    {
                        return typedValue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting setting for key {key}: {ex.Message}");
                }
            }

            return defaultValue;
        }

        public void SetSetting<T>(string key, T value)
        {
            _settings[key] = value;
        }

        public void RemoveSetting(string key)
        {
            if (_settings.ContainsKey(key))
                _settings.Remove(key);
        }

        public Dictionary<string, object> GetDefaultSettings()
        {
            return new Dictionary<string, object>
            {
                ["Username"] = "",
                ["MedicalLicenseNumber"] = "",
                ["TrainingUnit"] = "",
                ["Supervisor"] = "",
                ["EnableNotifications"] = true,
                ["EnableAutoSync"] = true,
                ["UseDarkTheme"] = false,
                ["LastAutoSync"] = DateTime.MinValue
            };
        }
    }
}