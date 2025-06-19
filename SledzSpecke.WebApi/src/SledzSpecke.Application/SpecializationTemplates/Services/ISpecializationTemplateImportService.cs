using SledzSpecke.Application.SpecializationTemplates.DTOs;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.SpecializationTemplates.Services;

public interface ISpecializationTemplateImportService
{
    Task<Result<List<SpecializationTemplateDto>>> GetAllTemplatesAsync();
    Task<Result<SpecializationTemplateDto>> GetTemplateAsync(string code, string version);
    Task<Result<int>> ImportTemplateAsync(SpecializationTemplateDto template);
    Task<Result<List<int>>> ImportFromDirectoryAsync(string directoryPath);
    Task<Result<bool>> ValidateTemplateAsync(SpecializationTemplateDto template);
    Task<Result<int>> UpdateTemplateAsync(string code, string version, SpecializationTemplateDto template);
    Task<Result<bool>> DeleteTemplateAsync(string code, string version);
    Task<Result<List<int>>> ImportFromCmkpWebsiteAsync(string smkVersion);
}