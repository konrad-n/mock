using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.DAL.Seeding;

internal sealed class DataSeeder : IDataSeeder
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(SledzSpeckeDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedSpecializationTemplatesAsync()
    {
        if (await _context.Specializations.AnyAsync())
        {
            _logger.LogInformation("Specializations already exist, skipping seeding.");
            return;
        }

        _logger.LogInformation("Starting specialization template seeding...");

        try
        {
            await SeedCardiologyNewAsync();
            await SeedCardiologyOldAsync();
            await SeedPsychiatryNewAsync();
            await SeedPsychiatryOldAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Specialization template seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during specialization template seeding.");
            throw;
        }
    }

    public async Task SeedBasicDataAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Basic data already exists, skipping seeding.");
            return;
        }

        _logger.LogInformation("Starting basic data seeding...");

        // This method can be extended to seed test users, basic lookup data, etc.
        _logger.LogInformation("Basic data seeding completed.");
    }

    private async Task SeedCardiologyNewAsync()
    {
        var cardiologyTemplate = await LoadSpecializationTemplateAsync("cardiology_new.json");
        if (cardiologyTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(cardiologyTemplate, SmkVersion.New);
        _context.Specializations.Add(specialization);
    }

    private async Task SeedCardiologyOldAsync()
    {
        var cardiologyTemplate = await LoadSpecializationTemplateAsync("cardiology_old.json");
        if (cardiologyTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(cardiologyTemplate, SmkVersion.Old);
        _context.Specializations.Add(specialization);
    }

    private async Task SeedPsychiatryNewAsync()
    {
        var psychiatryTemplate = await LoadSpecializationTemplateAsync("psychiatry_new.json");
        if (psychiatryTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(psychiatryTemplate, SmkVersion.New);
        _context.Specializations.Add(specialization);
    }

    private async Task SeedPsychiatryOldAsync()
    {
        var psychiatryTemplate = await LoadSpecializationTemplateAsync("psychiatry_old.json");
        if (psychiatryTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(psychiatryTemplate, SmkVersion.Old);
        _context.Specializations.Add(specialization);
    }

    private async Task<SpecializationTemplate?> LoadSpecializationTemplateAsync(string fileName)
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SpecializationTemplates", fileName);
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Specialization template file not found: {FileName}", fileName);
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<SpecializationTemplate>(jsonContent, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading specialization template: {FileName}", fileName);
            return null;
        }
    }

    private async Task<Specialization> CreateSpecializationFromTemplateAsync(SpecializationTemplate template, SmkVersion smkVersion)
    {
        // Generate unique specialization ID based on code and SMK version
        var idSeed = $"{template.Code}_{smkVersion}".GetHashCode();
        var specializationId = new SpecializationId(Math.Abs(idSeed));
        
        var startDate = DateTime.UtcNow.Date;
        var plannedEndDate = startDate.AddYears(template.TotalDuration.Years)
            .AddMonths(template.TotalDuration.Months)
            .AddDays(template.TotalDuration.Days);

        var specialization = new Specialization(
            specializationId,
            template.Name,
            template.Code,
            smkVersion,
            startDate,
            plannedEndDate,
            JsonSerializer.Serialize(template),
            template.TotalDuration.Years);

        // Create modules
        foreach (var moduleTemplate in template.Modules)
        {
            var moduleId = new ModuleId(moduleTemplate.ModuleId);
            var moduleType = Enum.Parse<ModuleType>(moduleTemplate.ModuleType);
            var moduleStartDate = startDate;
            var moduleEndDate = moduleStartDate.AddYears(moduleTemplate.Duration.Years)
                .AddMonths(moduleTemplate.Duration.Months)
                .AddDays(moduleTemplate.Duration.Days);

            var module = new Module(
                moduleId,
                specializationId,
                moduleType,
                smkVersion,
                moduleTemplate.Version,
                moduleTemplate.Name,
                moduleStartDate,
                moduleEndDate,
                JsonSerializer.Serialize(moduleTemplate));

            // Set totals from template with null checks
            module.UpdateProgress(
                0, // completed internships
                moduleTemplate.Internships?.Count ?? 0, // total internships
                0, // completed courses
                moduleTemplate.Courses?.Count ?? 0); // total courses

            var totalProceduresA = moduleTemplate.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
            var totalProceduresB = moduleTemplate.Procedures?.Sum(p => p.RequiredCountB) ?? 0;
            module.UpdateProceduresProgress(0, totalProceduresA, 0, totalProceduresB);

            var shiftHoursPerWeek = moduleTemplate.MedicalShifts?.HoursPerWeek ?? 0;
            var totalShiftHours = (int)(shiftHoursPerWeek * 52 * moduleTemplate.Duration.Years);
            module.UpdateShiftHours(0, totalShiftHours, shiftHoursPerWeek);
            
            module.UpdateSelfEducation(0, moduleTemplate.SelfEducation?.TotalDays ?? 0);

            specialization.AddModule(module);
        }

        return specialization;
    }
}