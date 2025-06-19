using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.SpecializationTemplates;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Repositories;

public sealed class SpecializationTemplateRepository : ISpecializationTemplateRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SpecializationTemplateRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<SpecializationTemplateDefinition?> GetByCodeAndVersionAsync(string code, string version)
    {
        return await _context.SpecializationTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.Code == code.ToLowerInvariant() && st.Version == version);
    }

    public async Task<List<SpecializationTemplateDefinition>> GetAllActiveAsync()
    {
        return await _context.SpecializationTemplates
            .AsNoTracking()
            .Where(st => st.IsActive)
            .OrderBy(st => st.Name)
            .ThenBy(st => st.Version)
            .ToListAsync();
    }

    public async Task<List<SpecializationTemplateDefinition>> GetAllAsync()
    {
        return await _context.SpecializationTemplates
            .AsNoTracking()
            .OrderBy(st => st.Name)
            .ThenBy(st => st.Version)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(SpecializationTemplateDefinition template)
    {
        await _context.SpecializationTemplates.AddAsync(template);
        await _context.SaveChangesAsync();
        return template.Id;
    }

    public async Task UpdateAsync(SpecializationTemplateDefinition template)
    {
        _context.SpecializationTemplates.Update(template);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string code, string version)
    {
        return await _context.SpecializationTemplates
            .AnyAsync(st => st.Code == code.ToLowerInvariant() && st.Version == version);
    }

    public async Task DeleteAsync(SpecializationTemplateDefinition template)
    {
        _context.SpecializationTemplates.Remove(template);
        await _context.SaveChangesAsync();
    }
}