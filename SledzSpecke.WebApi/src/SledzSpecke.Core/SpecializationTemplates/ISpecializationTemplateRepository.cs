namespace SledzSpecke.Core.SpecializationTemplates;

public interface ISpecializationTemplateRepository
{
    Task<SpecializationTemplateDefinition?> GetByCodeAndVersionAsync(string code, string version);
    Task<List<SpecializationTemplateDefinition>> GetAllActiveAsync();
    Task<List<SpecializationTemplateDefinition>> GetAllAsync();
    Task<int> CreateAsync(SpecializationTemplateDefinition template);
    Task UpdateAsync(SpecializationTemplateDefinition template);
    Task<bool> ExistsAsync(string code, string version);
    Task DeleteAsync(SpecializationTemplateDefinition template);
}