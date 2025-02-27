using System.Text.Json;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services
{
    public class DataManager
    {
        private static readonly string _appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string _specializationFile = Path.Combine(_appDataFolder, "specialization.json");

        private Specialization _specialization;

        public DataManager()
        {
            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }
        }

        public async Task<Specialization> LoadSpecializationAsync()
        {
            if (_specialization != null)
                return _specialization;

            if (File.Exists(_specializationFile))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(_specializationFile);
                    _specialization = JsonSerializer.Deserialize<Specialization>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading specialization data: {ex.Message}");
                    _specialization = DataSeeder.SeedHematologySpecialization();
                }
            }
            else
            {
                _specialization = DataSeeder.SeedHematologySpecialization();
            }

            return _specialization;
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                _specialization = specialization;
                string json = JsonSerializer.Serialize(specialization, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_specializationFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving specialization data: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            try
            {
                if (File.Exists(_specializationFile))
                {
                    File.Delete(_specializationFile);
                }
                _specialization = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all data: {ex.Message}");
                return false;
            }
        }
    }
}