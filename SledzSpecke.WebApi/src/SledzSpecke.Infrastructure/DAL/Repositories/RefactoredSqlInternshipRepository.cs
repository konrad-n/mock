using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;
using System.Data;
using System.Linq.Expressions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Internship repository using BaseRepository and Specifications
/// This implementation demonstrates world-class architecture with:
/// - Specification pattern for composable queries
/// - Inheritance from BaseRepository for common operations
/// - Clean separation of concerns
/// - Maintainable and testable code
/// </summary>
internal sealed class RefactoredSqlInternshipRepository : BaseRepository<Internship>, IInternshipRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlInternshipRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Internship?> GetByIdAsync(InternshipId id)
    {
        // Use the base repository method with includes
        return await Context.Internships
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .SingleOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Internship>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new InternshipBySpecializationSpecification(specializationId);
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    public async Task<IEnumerable<Internship>> GetByModuleIdAsync(ModuleId moduleId)
    {
        // Create specification for module filter
        var specification = new InternshipByModuleSpecification(moduleId);
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    public async Task<IEnumerable<Internship>> GetByUserAndSpecializationAsync(
        UserId userId, 
        SpecializationId specializationId)
    {
        // Note: The current domain model doesn't have a direct user association
        // This would typically use a specification combining user and specialization
        var specification = new InternshipBySpecializationSpecification(specializationId);
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    public async Task<IEnumerable<Internship>> GetByModuleAsync(ModuleId moduleId)
    {
        // This is a duplicate of GetByModuleIdAsync - keeping for interface compatibility
        return await GetByModuleIdAsync(moduleId);
    }

    public async Task<IEnumerable<Internship>> GetPendingApprovalAsync()
    {
        var specification = new CompletedNotApprovedInternshipSpecification();
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    public async Task AddAsync(Internship internship)
    {
        // Use PostgreSQL sequence for ID generation
        if (internship.InternshipId.Value == 0)
        {
            var newId = await GetNextInternshipIdAsync();
            
            // Use reflection to set the ID since it's private
            var idProperty = internship.GetType().GetProperty("InternshipId");
            idProperty?.SetValue(internship, newId);
        }

        await base.AddAsync(internship, default);
        
        // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
        // Keeping it here for backward compatibility with existing implementation
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Internship internship)
    {
        Update(internship);
        
        // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(InternshipId id)
    {
        var internship = await GetByIdAsync(id);
        if (internship is not null)
        {
            Remove(internship);
            
            // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods leveraging specifications for common queries
    
    /// <summary>
    /// Get active internships using specification pattern
    /// </summary>
    public async Task<IEnumerable<Internship>> GetActiveInternshipsAsync()
    {
        var specification = new ActiveInternshipSpecification();
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    /// <summary>
    /// Get internships within a date range using specification pattern
    /// </summary>
    public async Task<IEnumerable<Internship>> GetInternshipsInDateRangeAsync(
        DateTime startDate, 
        DateTime endDate)
    {
        var specification = new InternshipInDateRangeSpecification(startDate, endDate);
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    /// <summary>
    /// Get internships that need syncing using specification pattern
    /// </summary>
    public async Task<IEnumerable<Internship>> GetInternshipsNeedingSyncAsync()
    {
        var specification = new InternshipNeedsSyncSpecification();
        return await GetBySpecificationWithIncludesAsync(specification);
    }

    /// <summary>
    /// Get paginated internships using specification with pagination
    /// </summary>
    public async Task<(IEnumerable<Internship> Internships, int TotalCount)> GetPaginatedInternshipsAsync(
        ISpecification<Internship> specification,
        int pageNumber,
        int pageSize)
    {
        var result = await GetPagedAsync(specification, pageNumber, pageSize);
        return (result.Items, result.TotalCount);
    }

    // Private helper methods
    
    /// <summary>
    /// Helper method to get internships with includes
    /// </summary>
    private async Task<IEnumerable<Internship>> GetBySpecificationWithIncludesAsync(
        ISpecification<Internship> specification)
    {
        return await Context.Internships
            .Where(specification.ToExpression())
            .Include(i => i.MedicalShifts)
            .Include(i => i.Procedures)
            .ToListAsync();
    }

    /// <summary>
    /// PostgreSQL sequence-based ID generation
    /// This maintains compatibility with existing database schema
    /// </summary>
    private async Task<InternshipId> GetNextInternshipIdAsync()
    {
        // Create sequence if it doesn't exist and get next value
        await using var command = Context.Database.GetDbConnection().CreateCommand();
        command.CommandText = @"
            CREATE SEQUENCE IF NOT EXISTS internship_id_seq START WITH 1;
            SELECT nextval('internship_id_seq')::integer";
        
        await Context.Database.OpenConnectionAsync();
        var result = await command.ExecuteScalarAsync();
        await Context.Database.CloseConnectionAsync();
        
        return new InternshipId(Convert.ToInt32(result));
    }
}

/// <summary>
/// Specification for finding internships by module
/// This specification was missing from the core specifications
/// </summary>
public sealed class InternshipByModuleSpecification : Specification<Internship>
{
    private readonly ModuleId? _moduleId;

    public InternshipByModuleSpecification(ModuleId? moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<Internship, bool>> ToExpression()
    {
        return internship => internship.ModuleId == _moduleId;
    }
}