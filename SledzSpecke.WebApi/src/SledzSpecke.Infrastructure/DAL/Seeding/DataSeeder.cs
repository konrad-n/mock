using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.SpecializationTemplates;
using SledzSpecke.Infrastructure.Repositories;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.DAL.Seeding;

internal sealed class DataSeeder : IDataSeeder
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ILogger<DataSeeder> _logger;
    private readonly ISpecializationTemplateRepository _templateRepository;

    public DataSeeder(SledzSpeckeDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _templateRepository = new SpecializationTemplateRepository(context);
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
            // First, ensure SpecializationTemplate definitions are in database
            await SeedSpecializationTemplateDefinitionsAsync();

            // Then, create specializations from the templates in database
            await CreateSpecializationsFromTemplatesAsync();

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
        var testUser = User.CreateWithId(
            new UserId(1),
            new Email("test@example.com"),
            new HashedPassword("$2a$10$abc123"), // BCrypt hash format
            new FirstName("Jan"),
            null, // SecondName - optional
            new LastName("Kowalski"),
            new Pesel("90010112345"), // Valid PESEL for DOB 1990-01-01
            new PwzNumber("1234567"),
            new PhoneNumber("+48123456789"),
            new DateTime(1990, 1, 1), // Date of birth matching PESEL
            new Address(
                "Marszałkowska",
                "100",
                "5A",
                "00-001",
                "Warszawa",
                "Mazowieckie",
                "Polska"
            ),
            DateTime.UtcNow
        );

        // AI HINT: If you get "Specialization with ID 1 not found" errors:
        // 1. Check that specializations are seeded BEFORE users
        // 2. Verify the migration includes specialization data
        // 3. The user references SpecializationId(1) which must exist
        
        _context.Users.Add(testUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Basic data seeding completed.");
    }

    private async Task SeedSpecializationTemplateDefinitionsAsync()
    {
        // Check if we already have templates in the database
        var existingTemplates = await _templateRepository.GetAllAsync();
        if (existingTemplates.Any())
        {
            _logger.LogInformation("SpecializationTemplate definitions already exist in database.");
            return;
        }

        _logger.LogInformation("Seeding initial SpecializationTemplate definitions...");

        // Load and save the initial 4 templates to the database
        var templateFiles = new[]
        {
            "cardiology_new.json",
            "cardiology_old.json",
            "psychiatry_new.json",
            "psychiatry_old.json"
        };

        foreach (var fileName in templateFiles)
        {
            var template = await LoadSpecializationTemplateAsync(fileName);
            if (template == null) continue;

            var version = fileName.Contains("_new") ? "CMKP 2023" : "CMKP 2014";
            var jsonContent = JsonSerializer.Serialize(template, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var templateDefinition = SpecializationTemplateDefinition.Create(
                template.Code,
                template.Name,
                version,
                jsonContent);

            if (templateDefinition.IsSuccess)
            {
                await _templateRepository.CreateAsync(templateDefinition.Value!);
                _logger.LogInformation("Created template definition: {Code} v{Version}", 
                    template.Code, version);
            }
        }
    }

    private async Task CreateSpecializationsFromTemplatesAsync()
    {
        // Get all active templates from database
        var templates = await _templateRepository.GetAllActiveAsync();
        
        foreach (var templateDef in templates)
        {
            try
            {
                // Deserialize the JSON content to SpecializationTemplate
                var template = JsonSerializer.Deserialize<SpecializationTemplate>(
                    templateDef.JsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (template == null) continue;

                // Determine SMK version from template version string
                var smkVersion = templateDef.Version.Contains("2023") ? SmkVersion.New : SmkVersion.Old;
                
                // Create specialization from template
                var specialization = await CreateSpecializationFromTemplateAsync(template, smkVersion);
                _context.Specializations.Add(specialization);

                // Add modules explicitly since navigation property is ignored
                foreach (var module in specialization.Modules)
                {
                    _context.Modules.Add(module);
                }

                _logger.LogInformation("Created specialization from template: {Code} v{Version}", 
                    templateDef.Code, templateDef.Version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating specialization from template: {Code} v{Version}", 
                    templateDef.Code, templateDef.Version);
            }
        }
    }

    // Old individual seeding methods removed - now using dynamic loading from database

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
        // Generate dynamic ID based on existing specializations
        var maxId = await _context.Specializations
            .Select(s => s.Id.Value)
            .DefaultIfEmpty(0)
            .MaxAsync();
        
        var specializationId = maxId + 1;

        var startDate = DateTime.UtcNow.Date;
        var plannedEndDate = startDate.AddYears(template.TotalDuration.Years)
            .AddMonths(template.TotalDuration.Months)
            .AddDays(template.TotalDuration.Days);

        var specialization = new Specialization(
            new SpecializationId(specializationId),
            new UserId(1), // Default user ID for template specializations
            template.Name,
            template.Code,
            smkVersion,
            "standard", // Default program variant
            startDate,
            plannedEndDate,
            1, // Default planned PES year
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