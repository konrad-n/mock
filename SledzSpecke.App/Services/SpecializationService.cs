using System.Text.Json;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services
{
    public class SpecializationService
    {
        private static readonly string _appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string _specializationFile = Path.Combine(_appDataFolder, "specialization.json");

        public async Task<Specialization> GetSpecializationAsync()
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(_specializationFile))
                {
                    // If the file doesn't exist, create a default specialization and save it
                    var defaultSpecialization = DataSeeder.SeedHematologySpecialization();
                    await SaveSpecializationAsync(defaultSpecialization);
                    return defaultSpecialization;
                }

                // Read the file content
                string jsonContent = await File.ReadAllTextAsync(_specializationFile);

                // Deserialize the JSON to a Specialization object
                var specialization = JsonSerializer.Deserialize<Specialization>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return specialization ?? DataSeeder.SeedHematologySpecialization();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading specialization: {ex.Message}");
                // If there's an error, return a default specialization
                return DataSeeder.SeedHematologySpecialization();
            }
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(_appDataFolder);

                // Serialize the Specialization object to JSON
                string jsonContent = JsonSerializer.Serialize(specialization,
                    new JsonSerializerOptions { WriteIndented = true });

                // Write the JSON to the file
                await File.WriteAllTextAsync(_specializationFile, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving specialization: {ex.Message}");
                throw;
            }
        }
    }
}