using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.SpecializationTemplates.DTOs;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.SpecializationTemplates;

namespace SledzSpecke.Application.SpecializationTemplates.Services;

public sealed class SpecializationTemplateImportService : ISpecializationTemplateImportService
{
    private readonly ISpecializationTemplateRepository _repository;
    private readonly ILogger<SpecializationTemplateImportService> _logger;
    private readonly string _templatesDirectory;

    public SpecializationTemplateImportService(
        ISpecializationTemplateRepository repository,
        ILogger<SpecializationTemplateImportService> logger)
    {
        _repository = repository;
        _logger = logger;
        _templatesDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "Data", 
            "SpecializationTemplates");
    }

    public async Task<Result<List<SpecializationTemplateDto>>> GetAllTemplatesAsync()
    {
        try
        {
            var templates = await _repository.GetAllAsync();
            var dtos = templates.Select(MapToDto).ToList();
            return Result<List<SpecializationTemplateDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all templates");
            return Result<List<SpecializationTemplateDto>>.Failure($"Error fetching templates: {ex.Message}");
        }
    }

    public async Task<Result<SpecializationTemplateDto>> GetTemplateAsync(string code, string version)
    {
        try
        {
            var template = await _repository.GetByCodeAndVersionAsync(code, version);
            if (template == null)
            {
                return Result<SpecializationTemplateDto>.Failure($"Template not found: {code} v{version}");
            }

            return Result<SpecializationTemplateDto>.Success(MapToDto(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching template {Code} v{Version}", code, version);
            return Result<SpecializationTemplateDto>.Failure($"Error fetching template: {ex.Message}");
        }
    }

    public async Task<Result<int>> ImportTemplateAsync(SpecializationTemplateDto dto)
    {
        try
        {
            // Check if already exists
            if (await _repository.ExistsAsync(dto.Code, dto.Version))
            {
                return Result<int>.Failure($"Template already exists: {dto.Code} v{dto.Version}");
            }

            // Serialize DTO to JSON
            var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Create entity
            var createResult = SpecializationTemplateDefinition.Create(
                dto.Code,
                dto.Name,
                dto.Version,
                jsonContent);

            if (!createResult.IsSuccess)
            {
                return Result<int>.Failure(createResult.Error!);
            }

            // Save to repository
            var id = await _repository.CreateAsync(createResult.Value!);
            _logger.LogInformation("Imported template {Code} v{Version} with ID {Id}", 
                dto.Code, dto.Version, id);

            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing template {Code} v{Version}", dto.Code, dto.Version);
            return Result<int>.Failure($"Error importing template: {ex.Message}");
        }
    }

    public async Task<Result<List<int>>> ImportFromDirectoryAsync(string directoryPath)
    {
        try
        {
            var fullPath = string.IsNullOrEmpty(directoryPath) 
                ? _templatesDirectory 
                : directoryPath;

            if (!Directory.Exists(fullPath))
            {
                return Result<List<int>>.Failure($"Directory not found: {fullPath}");
            }

            var jsonFiles = Directory.GetFiles(fullPath, "*.json");
            if (jsonFiles.Length == 0)
            {
                return Result<List<int>>.Failure($"No JSON files found in: {fullPath}");
            }

            var importedIds = new List<int>();
            var errors = new List<string>();

            foreach (var file in jsonFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var dto = JsonSerializer.Deserialize<SpecializationTemplateDto>(json, 
                        new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });

                    if (dto == null)
                    {
                        errors.Add($"Failed to deserialize: {Path.GetFileName(file)}");
                        continue;
                    }

                    // Skip if already exists
                    if (await _repository.ExistsAsync(dto.Code, dto.Version))
                    {
                        _logger.LogInformation("Skipping existing template: {Code} v{Version}", 
                            dto.Code, dto.Version);
                        continue;
                    }

                    var result = await ImportTemplateAsync(dto);
                    if (result.IsSuccess)
                    {
                        importedIds.Add(result.Value);
                    }
                    else
                    {
                        errors.Add($"{Path.GetFileName(file)}: {result.Error}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(file)}: {ex.Message}");
                }
            }

            if (errors.Any())
            {
                _logger.LogWarning("Import completed with errors: {Errors}", string.Join("; ", errors));
            }

            return Result<List<int>>.Success(importedIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing from directory: {Directory}", directoryPath);
            return Result<List<int>>.Failure($"Error importing from directory: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ValidateTemplateAsync(SpecializationTemplateDto dto)
    {
        try
        {
            var errors = new List<string>();

            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.Code))
                errors.Add("Code is required");
            
            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Name is required");
            
            if (string.IsNullOrWhiteSpace(dto.Version))
                errors.Add("Version is required");
            
            if (dto.Version != "CMKP 2014" && dto.Version != "CMKP 2023")
                errors.Add("Version must be 'CMKP 2014' or 'CMKP 2023'");
            
            if (dto.TotalDuration == null || dto.TotalDuration.Years <= 0)
                errors.Add("Total duration must be specified");
            
            if (dto.Modules == null || !dto.Modules.Any())
                errors.Add("At least one module is required");

            // Module validation
            if (dto.Modules != null)
            {
                var hasBasicModule = dto.Modules.Any(m => m.ModuleType == "Basic");
                if (!hasBasicModule)
                    errors.Add("Basic module is required");

                foreach (var module in dto.Modules)
                {
                    if (string.IsNullOrWhiteSpace(module.Name))
                        errors.Add($"Module name is required (ID: {module.ModuleId})");
                    
                    if (module.ModuleType != "Basic" && module.ModuleType != "Specialist")
                        errors.Add($"Module type must be 'Basic' or 'Specialist' (ID: {module.ModuleId})");
                    
                    if (module.Courses == null || !module.Courses.Any())
                        errors.Add($"Module must have at least one course (ID: {module.ModuleId})");
                    
                    if (module.Internships == null || !module.Internships.Any())
                        errors.Add($"Module must have at least one internship (ID: {module.ModuleId})");
                }
            }

            if (errors.Any())
            {
                return Result<bool>.Failure(string.Join("; ", errors));
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating template");
            return Result<bool>.Failure($"Validation error: {ex.Message}");
        }
    }

    public async Task<Result<int>> UpdateTemplateAsync(string code, string version, SpecializationTemplateDto dto)
    {
        try
        {
            var template = await _repository.GetByCodeAndVersionAsync(code, version);
            if (template == null)
            {
                return Result<int>.Failure($"Template not found: {code} v{version}");
            }

            // Serialize updated DTO to JSON
            var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Update entity
            var updateResult = template.Update(dto.Name, jsonContent);
            if (!updateResult.IsSuccess)
            {
                return Result<int>.Failure(updateResult.Error!);
            }

            await _repository.UpdateAsync(template);
            _logger.LogInformation("Updated template {Code} v{Version}", code, version);

            return Result<int>.Success(template.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {Code} v{Version}", code, version);
            return Result<int>.Failure($"Error updating template: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteTemplateAsync(string code, string version)
    {
        try
        {
            var template = await _repository.GetByCodeAndVersionAsync(code, version);
            if (template == null)
            {
                return Result<bool>.Failure($"Template not found: {code} v{version}");
            }

            // Soft delete - just deactivate
            template.Deactivate();
            await _repository.UpdateAsync(template);
            
            _logger.LogInformation("Deactivated template {Code} v{Version}", code, version);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {Code} v{Version}", code, version);
            return Result<bool>.Failure($"Error deleting template: {ex.Message}");
        }
    }

    public async Task<Result<List<int>>> ImportFromCmkpWebsiteAsync(string smkVersion)
    {
        try
        {
            _logger.LogInformation("Starting CMKP website import for SMK version: {SmkVersion}", smkVersion);
            
            // Validate SMK version
            if (smkVersion != "old" && smkVersion != "new")
            {
                return Result<List<int>>.Failure("Invalid SMK version. Must be 'old' or 'new'");
            }

            // Use the CMKP helper script to download and process specializations
            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "../..", "cmkp-specialization-helper.sh");
            if (!File.Exists(scriptPath))
            {
                return Result<List<int>>.Failure($"CMKP helper script not found at: {scriptPath}");
            }

            // First, list all available specializations
            var listProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"{scriptPath} list-specializations {smkVersion}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            listProcess.Start();
            var specializationList = await listProcess.StandardOutput.ReadToEndAsync();
            var errors = await listProcess.StandardError.ReadToEndAsync();
            await listProcess.WaitForExitAsync();

            if (listProcess.ExitCode != 0)
            {
                _logger.LogError("Failed to list specializations: {Error}", errors);
                return Result<List<int>>.Failure($"Failed to list specializations: {errors}");
            }

            // Parse specialization codes from the output
            var specializations = specializationList
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            _logger.LogInformation("Found {Count} specializations to import", specializations.Count);

            var importedIds = new List<int>();
            var importErrors = new List<string>();

            // Process each specialization
            foreach (var specializationInfo in specializations)
            {
                try
                {
                    // Expected format: "code - name"
                    var parts = specializationInfo.Split(" - ", 2);
                    if (parts.Length < 2) continue;

                    var code = parts[0].Trim();
                    var name = parts[1].Trim();

                    _logger.LogInformation("Processing specialization: {Code} - {Name}", code, name);

                    // Check if already exists
                    var version = smkVersion == "new" ? "CMKP 2023" : "CMKP 2014";
                    if (await _repository.ExistsAsync(code, version))
                    {
                        _logger.LogInformation("Specialization already exists: {Code} v{Version}", code, version);
                        continue;
                    }

                    // Download PDF
                    var downloadProcess = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"{scriptPath} download-pdf {smkVersion} {code}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    downloadProcess.Start();
                    await downloadProcess.WaitForExitAsync();

                    if (downloadProcess.ExitCode != 0)
                    {
                        var downloadError = await downloadProcess.StandardError.ReadToEndAsync();
                        importErrors.Add($"{code}: Failed to download PDF - {downloadError}");
                        continue;
                    }

                    // Parse PDF to JSON
                    var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "../..", "docs", "cmkp-pdfs", 
                        $"{smkVersion}-smk", $"{code}.pdf");

                    var parseProcess = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"{scriptPath} parse-pdf \"{pdfPath}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    parseProcess.Start();
                    var jsonOutput = await parseProcess.StandardOutput.ReadToEndAsync();
                    await parseProcess.WaitForExitAsync();

                    if (parseProcess.ExitCode != 0)
                    {
                        var parseError = await parseProcess.StandardError.ReadToEndAsync();
                        importErrors.Add($"{code}: Failed to parse PDF - {parseError}");
                        continue;
                    }

                    // Import the JSON template
                    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SpecializationTemplates", 
                        $"{code}_{smkVersion}.json");

                    if (File.Exists(jsonPath))
                    {
                        var json = await File.ReadAllTextAsync(jsonPath);
                        var dto = JsonSerializer.Deserialize<SpecializationTemplateDto>(json, 
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (dto != null)
                        {
                            var importResult = await ImportTemplateAsync(dto);
                            if (importResult.IsSuccess)
                            {
                                importedIds.Add(importResult.Value!);
                                _logger.LogInformation("Successfully imported: {Code} v{Version}", code, version);
                            }
                            else
                            {
                                importErrors.Add($"{code}: {importResult.Error}");
                            }
                        }
                    }
                    else
                    {
                        importErrors.Add($"{code}: JSON file not found after parsing");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing specialization: {Specialization}", specializationInfo);
                    importErrors.Add($"{specializationInfo}: {ex.Message}");
                }
            }

            if (importErrors.Any())
            {
                _logger.LogWarning("Import completed with errors: {Errors}", string.Join("; ", importErrors));
            }

            _logger.LogInformation("CMKP import completed. Imported {Count} specializations", importedIds.Count);
            return Result<List<int>>.Success(importedIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing from CMKP website");
            return Result<List<int>>.Failure($"Error importing from CMKP: {ex.Message}");
        }
    }

    private SpecializationTemplateDto MapToDto(SpecializationTemplateDefinition template)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<SpecializationTemplateDto>(
                template.JsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto != null)
            {
                dto.Id = template.Id;
                dto.CreatedAt = template.CreatedAt;
                dto.UpdatedAt = template.UpdatedAt;
                dto.IsActive = template.IsActive;
            }

            return dto ?? new SpecializationTemplateDto
            {
                Id = template.Id,
                Code = template.Code,
                Name = template.Name,
                Version = template.Version,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                IsActive = template.IsActive
            };
        }
        catch
        {
            // Fallback if JSON parsing fails
            return new SpecializationTemplateDto
            {
                Id = template.Id,
                Code = template.Code,
                Name = template.Name,
                Version = template.Version,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                IsActive = template.IsActive
            };
        }
    }
}