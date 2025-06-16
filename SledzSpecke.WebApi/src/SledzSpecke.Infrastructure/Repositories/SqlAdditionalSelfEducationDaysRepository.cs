using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Repositories;

public class SqlAdditionalSelfEducationDaysRepository : IAdditionalSelfEducationDaysRepository
{
    private readonly SledzSpeckeDbContext _context;

    public SqlAdditionalSelfEducationDaysRepository(SledzSpeckeDbContext context)
    {
        _context = context;
    }

    public async Task<AdditionalSelfEducationDays?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.AdditionalSelfEducationDays
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByModuleIdAsync(int moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.AdditionalSelfEducationDays
            .AsNoTracking()
            .Where(x => x.ModuleId == new ModuleId(moduleId))
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByInternshipIdAsync(int internshipId, CancellationToken cancellationToken = default)
    {
        return await _context.AdditionalSelfEducationDays
            .AsNoTracking()
            .Where(x => x.InternshipId == new InternshipId(internshipId))
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdditionalSelfEducationDays>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _context.AdditionalSelfEducationDays
            .AsNoTracking()
            .Where(x => x.StartDate.Year == year || x.EndDate.Year == year)
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalDaysInYearAsync(int moduleId, int year, CancellationToken cancellationToken = default)
    {
        var educationDays = await _context.AdditionalSelfEducationDays
            .AsNoTracking()
            .Where(x => x.ModuleId == new ModuleId(moduleId))
            .Where(x => x.StartDate.Year == year || x.EndDate.Year == year)
            .Where(x => x.IsApproved)
            .ToListAsync(cancellationToken);

        // Calculate total days, accounting for events that span multiple years
        var totalDays = 0;
        foreach (var edu in educationDays)
        {
            var startDate = edu.StartDate.Year == year ? edu.StartDate : new DateTime(year, 1, 1);
            var endDate = edu.EndDate.Year == year ? edu.EndDate : new DateTime(year, 12, 31);
            
            if (startDate <= endDate)
            {
                totalDays += (endDate - startDate).Days + 1;
            }
        }

        return totalDays;
    }

    public async Task AddAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        await _context.AdditionalSelfEducationDays.AddAsync(additionalSelfEducationDays, cancellationToken);
    }

    public Task UpdateAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        _context.AdditionalSelfEducationDays.Update(additionalSelfEducationDays);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AdditionalSelfEducationDays additionalSelfEducationDays, CancellationToken cancellationToken = default)
    {
        _context.AdditionalSelfEducationDays.Remove(additionalSelfEducationDays);
        return Task.CompletedTask;
    }
}