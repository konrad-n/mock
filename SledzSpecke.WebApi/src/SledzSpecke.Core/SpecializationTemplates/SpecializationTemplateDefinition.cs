using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities.Base;

namespace SledzSpecke.Core.SpecializationTemplates;

public sealed class SpecializationTemplateDefinition : Entity
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Version { get; private set; } = string.Empty; // "CMKP 2014" or "CMKP 2023"
    public string JsonContent { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private SpecializationTemplateDefinition() { } // EF Core

    public static Result<SpecializationTemplateDefinition> Create(
        string code, 
        string name, 
        string version, 
        string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<SpecializationTemplateDefinition>.Failure("Code is required");
            
        if (string.IsNullOrWhiteSpace(name))
            return Result<SpecializationTemplateDefinition>.Failure("Name is required");
            
        if (string.IsNullOrWhiteSpace(version))
            return Result<SpecializationTemplateDefinition>.Failure("Version is required");
            
        if (!IsValidVersion(version))
            return Result<SpecializationTemplateDefinition>.Failure("Version must be 'CMKP 2014' or 'CMKP 2023'");
            
        if (string.IsNullOrWhiteSpace(jsonContent))
            return Result<SpecializationTemplateDefinition>.Failure("JSON content is required");

        var template = new SpecializationTemplateDefinition
        {
            Code = code.ToLowerInvariant(),
            Name = name,
            Version = version,
            JsonContent = jsonContent,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return Result<SpecializationTemplateDefinition>.Success(template);
    }

    public Result<bool> Update(string name, string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<bool>.Failure("Name is required");
            
        if (string.IsNullOrWhiteSpace(jsonContent))
            return Result<bool>.Failure("JSON content is required");

        Name = name;
        JsonContent = jsonContent;
        UpdatedAt = DateTime.UtcNow;

        return Result<bool>.Success(true);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidVersion(string version)
    {
        return version == "CMKP 2014" || version == "CMKP 2023";
    }
}