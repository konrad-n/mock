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

        // Seed a test user
        var testUser = new User(
            new Email("test@example.com"),
            new Username("testuser"),
            new Password("VN5/YG8lI8uo76wXP6tC+39Z1Wzv+XTI/bc0LPLP40U="), // SHA256 hash of "Test123!"
            new FullName("Test User"),
            SmkVersion.Old,
            new SpecializationId(1), // WARNING: This assumes specialization ID 1 exists!
            DateTime.UtcNow
        );

        // AI HINT: If you get "Specialization with ID 1 not found" errors:
        // 1. Check that specializations are seeded BEFORE users
        // 2. Verify the migration includes specialization data
        // 3. The user references SpecializationId(1) which must exist
        
        testUser.SetId(new UserId(1));
        _context.Users.Add(testUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Basic data seeding completed.");
    }

    private async Task SeedCardiologyNewAsync()
    {
        var cardiologyTemplate = await LoadSpecializationTemplateAsync("cardiology_new.json");
        if (cardiologyTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(cardiologyTemplate, SmkVersion.New);
        _context.Specializations.Add(specialization);

        // Add modules explicitly since navigation property is ignored
        foreach (var module in specialization.Modules)
        {
            _context.Modules.Add(module);
        }
    }

    private async Task SeedCardiologyOldAsync()
    {
        var cardiologyTemplate = await LoadSpecializationTemplateAsync("cardiology_old.json");
        if (cardiologyTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(cardiologyTemplate, SmkVersion.Old);
        _context.Specializations.Add(specialization);

        // Add modules explicitly since navigation property is ignored
        foreach (var module in specialization.Modules)
        {
            _context.Modules.Add(module);
        }
    }

    private async Task SeedPsychiatryNewAsync()
    {
        var psychiatryTemplate = await LoadSpecializationTemplateAsync("psychiatry_new.json");
        if (psychiatryTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(psychiatryTemplate, SmkVersion.New);
        _context.Specializations.Add(specialization);

        // Add modules explicitly since navigation property is ignored
        foreach (var module in specialization.Modules)
        {
            _context.Modules.Add(module);
        }
    }

    private async Task SeedPsychiatryOldAsync()
    {
        var psychiatryTemplate = await LoadSpecializationTemplateAsync("psychiatry_old.json");
        if (psychiatryTemplate == null) return;

        var specialization = await CreateSpecializationFromTemplateAsync(psychiatryTemplate, SmkVersion.Old);
        _context.Specializations.Add(specialization);

        // Add modules explicitly since navigation property is ignored
        foreach (var module in specialization.Modules)
        {
            _context.Modules.Add(module);
        }
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
        // Use hardcoded IDs for now (1-4 for the 4 specializations)
        // Old: Cardiology=1, Psychiatry=2
        // New: Cardiology=3, Psychiatry=4
        var specializationId = smkVersion == SmkVersion.Old
            ? (template.Code == "cardiology" ? 1 : 2)
            : (template.Code == "cardiology" ? 3 : 4);

        var startDate = DateTime.UtcNow.Date;
        var plannedEndDate = startDate.AddYears(template.TotalDuration.Years)
            .AddMonths(template.TotalDuration.Months)
            .AddDays(template.TotalDuration.Days);

        var specialization = new Specialization(
            new SpecializationId(specializationId),
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
            // Generate unique module ID by combining specialization ID and module ID
            var moduleId = new ModuleId(specializationId * 100 + moduleTemplate.ModuleId);
            var moduleType = Enum.Parse<ModuleType>(moduleTemplate.ModuleType);
            var moduleStartDate = startDate;
            var moduleEndDate = moduleStartDate.AddYears(moduleTemplate.Duration.Years)
                .AddMonths(moduleTemplate.Duration.Months)
                .AddDays(moduleTemplate.Duration.Days);

            var module = new Module(
                moduleId,
                new SpecializationId(specializationId),
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