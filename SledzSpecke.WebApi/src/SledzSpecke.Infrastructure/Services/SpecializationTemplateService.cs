using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Services;

public class SpecializationTemplateService : ISpecializationTemplateService
{
    private readonly ILogger<SpecializationTemplateService> _logger;
    private readonly string _templatesPath;
    private readonly Dictionary<string, SpecializationTemplate> _templateCache = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public SpecializationTemplateService(ILogger<SpecializationTemplateService> logger)
    {
        _logger = logger;
        _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SpecializationTemplates");
    }

    public async Task<SpecializationTemplate?> GetTemplateAsync(string specializationCode, SmkVersion smkVersion)
    {
        var cacheKey = GetCacheKey(specializationCode, smkVersion);

        await _cacheLock.WaitAsync();
        try
        {
            if (_templateCache.TryGetValue(cacheKey, out var cachedTemplate))
            {
                return cachedTemplate;
            }

            var template = await LoadTemplateAsync(specializationCode, smkVersion);
            if (template != null)
            {
                _templateCache[cacheKey] = template;
            }

            return template;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<IEnumerable<SpecializationTemplate>> GetAllTemplatesAsync()
    {
        var templates = new List<SpecializationTemplate>();

        if (!Directory.Exists(_templatesPath))
        {
            _logger.LogWarning("Templates directory not found: {Path}", _templatesPath);
            return templates;
        }

        var files = Directory.GetFiles(_templatesPath, "*.json");
        foreach (var file in files)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var parts = fileName.Split('_');
                if (parts.Length == 2)
                {
                    var code = parts[0];
                    var version = parts[1] == "old" ? SmkVersion.Old : SmkVersion.New;

                    var template = await GetTemplateAsync(code, version);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading template from file: {File}", file);
            }
        }

        return templates;
    }

    public async Task<ModuleTemplate?> GetModuleTemplateAsync(string specializationCode, SmkVersion smkVersion, int moduleId)
    {
        var template = await GetTemplateAsync(specializationCode, smkVersion);
        return template?.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
    }

    public async Task<ProcedureTemplate?> GetProcedureTemplateAsync(string specializationCode, SmkVersion smkVersion, int procedureId)
    {
        var template = await GetTemplateAsync(specializationCode, smkVersion);
        if (template == null) return null;

        foreach (var module in template.Modules)
        {
            var procedure = module.Procedures.FirstOrDefault(p => p.Id == procedureId);
            if (procedure != null) return procedure;
        }

        return null;
    }

    public async Task<bool> ValidateProcedureRequirementsAsync(string specializationCode, SmkVersion smkVersion, int procedureId, string procedureCode)
    {
        var procedureTemplate = await GetProcedureTemplateAsync(specializationCode, smkVersion, procedureId);
        if (procedureTemplate == null)
        {
            _logger.LogWarning("Procedure template not found: {Code} {Version} {Id}", specializationCode, smkVersion, procedureId);
            return false;
        }

        // For Old SMK, procedure codes A and B have specific meanings
        // For New SMK, validation might be different based on modules
        if (smkVersion == SmkVersion.Old)
        {
            return procedureCode == "A" || procedureCode == "B";
        }
        else
        {
            // New SMK might have different validation rules
            // For now, we'll accept standard codes
            return !string.IsNullOrEmpty(procedureCode);
        }
    }

    public async Task<InternshipTemplate?> GetInternshipTemplateAsync(string specializationCode, SmkVersion smkVersion, int internshipId)
    {
        var template = await GetTemplateAsync(specializationCode, smkVersion);
        if (template == null) return null;

        foreach (var module in template.Modules)
        {
            var internship = module.Internships.FirstOrDefault(i => i.Id == internshipId);
            if (internship != null) return internship;
        }

        return null;
    }

    public async Task<CourseTemplate?> GetCourseTemplateAsync(string specializationCode, SmkVersion smkVersion, int courseId)
    {
        var template = await GetTemplateAsync(specializationCode, smkVersion);
        if (template == null) return null;

        foreach (var module in template.Modules)
        {
            var course = module.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course != null) return course;
        }

        return null;
    }

    private async Task<SpecializationTemplate?> LoadTemplateAsync(string specializationCode, SmkVersion smkVersion)
    {
        try
        {
            var fileName = $"{specializationCode}_{(smkVersion == SmkVersion.Old ? "old" : "new")}.json";
            var filePath = Path.Combine(_templatesPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Template file not found: {Path}", filePath);
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<SpecializationTemplate>(jsonContent, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading template: {Code} {Version}", specializationCode, smkVersion);
            return null;
        }
    }

    private string GetCacheKey(string specializationCode, SmkVersion smkVersion)
    {
        return $"{specializationCode}_{smkVersion}";
    }
}