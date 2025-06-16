using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;
using System.Linq.Expressions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Procedure repository using BaseRepository and Specifications pattern.
/// Follows the same architecture as RefactoredSqlMedicalShiftRepository.
/// </summary>
internal sealed class RefactoredSqlProcedureRepository : BaseRepository<ProcedureBase>, IProcedureRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInternshipRepository _internshipRepository;

    public RefactoredSqlProcedureRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork,
        IInternshipRepository internshipRepository) : base(context)
    {
        _unitOfWork = unitOfWork;
        _internshipRepository = internshipRepository;
    }

    public async Task<ProcedureBase?> GetByIdAsync(ProcedureId id)
    {
        return await GetByIdAsync(id.Value, default);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByInternshipIdAsync(int internshipId)
    {
        var specification = new ProcedureByInternshipSpecification(new InternshipId(internshipId));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByUserIdAsync(int userId)
    {
        // First get all internships for the user
        var internshipIds = await GetInternshipIdsForUserAsync(userId);
        
        // Then use specification to get procedures
        var specifications = internshipIds
            .Select(id => new ProcedureByInternshipSpecification(id) as Specification<ProcedureBase>)
            .ToList();
            
        if (!specifications.Any())
            return Enumerable.Empty<ProcedureBase>();
            
        // Combine all specifications with OR logic
        Specification<ProcedureBase> combinedSpec = specifications.First();
        foreach (var spec in specifications.Skip(1))
        {
            combinedSpec = combinedSpec.Or(spec);
        }
        
        return await GetBySpecificationAsync(combinedSpec);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByUserAsync(UserId userId)
    {
        return await GetByUserIdAsync(userId.Value);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByCodeAsync(string code)
    {
        var specification = new ProcedureByCodeSpecification(code);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<ProcedureBase>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int userId)
    {
        // Get internship IDs for user
        var internshipIds = await GetInternshipIdsForUserAsync(userId);
        
        if (!internshipIds.Any())
            return Enumerable.Empty<ProcedureBase>();
        
        // Combine internship specifications with OR
        Specification<ProcedureBase> internshipSpec = new ProcedureByInternshipSpecification(internshipIds.First());
        foreach (var id in internshipIds.Skip(1))
        {
            internshipSpec = internshipSpec.Or(new ProcedureByInternshipSpecification(id));
        }
        
        // Add date range specification with AND
        var dateRangeSpec = new ProcedureByDateRangeSpecification(startDate, endDate);
        var combinedSpec = internshipSpec.And(dateRangeSpec);
        
        return await GetBySpecificationAsync(combinedSpec);
    }

    public async Task<IEnumerable<ProcedureBase>> GetAllAsync()
    {
        return await GetAllAsync(default);
    }

    public async Task<int> AddAsync(ProcedureBase procedure)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (procedure.Id.Value == 0)
        {
            await GenerateIdForEntity(procedure);
        }

        await AddAsync(procedure, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
        return procedure.Id.Value;
    }

    public async Task UpdateAsync(ProcedureBase procedure)
    {
        Update(procedure);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProcedureBase procedure)
    {
        Remove(procedure);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Dictionary<string, int>> GetProcedureCountsByCodeAsync(int internshipId)
    {
        var specification = new ProcedureByInternshipSpecification(new InternshipId(internshipId));
        var procedures = await GetBySpecificationAsync(specification);
        
        return procedures
            .GroupBy(p => p.Code)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<int>> GetUserInternshipIdsAsync(int userId)
    {
        var internshipIds = await GetInternshipIdsForUserAsync(userId);
        return internshipIds.Select(id => id.Value);
    }

    // Additional methods using existing specifications
    public async Task<IEnumerable<ProcedureBase>> GetCompletedProceduresForYearAsync(int year)
    {
        var specification = ProcedureSpecificationExamples.GetCompletedProceduresForYear(year);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<ProcedureBase>> GetUnsyncedTypeAProceduresAsync()
    {
        var specification = ProcedureSpecificationExamples.GetUnsyncedTypeAProcedures();
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<ProcedureBase>> GetProceduresNeedingReviewAsync(DateTime cutoffDate)
    {
        var specification = ProcedureSpecificationExamples.GetProceduresNeedingReview(cutoffDate);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<ProcedureBase>> GetCompletedProceduresInDateRangeForInternshipAsync(
        InternshipId internshipId, DateTime startDate, DateTime endDate)
    {
        var specification = new ProcedureByInternshipSpecification(internshipId)
            .And(new CompletedProceduresInDateRangeSpecification(startDate, endDate));
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> HasProcedureWithCodeAsync(InternshipId internshipId, string code)
    {
        var specification = new ProcedureByInternshipSpecification(internshipId)
            .And(new ProcedureByCodeSpecification(code));
        return await CountBySpecificationAsync(specification) > 0;
    }

    // Private helper methods
    private async Task<List<InternshipId>> GetInternshipIdsForUserAsync(int userId)
    {
        // This could be refactored to use specifications on Internship repository
        var internships = await Context.Internships
            .Join(Context.Specializations,
                i => i.SpecializationId,
                s => s.Id,
                (i, s) => new { Internship = i, Specialization = s })
            .Join(Context.Users,
                x => x.Specialization.Id,
                u => u.SpecializationId,
                (x, u) => new { x.Internship, User = u })
            .Where(x => x.User.Id.Value == userId)
            .Select(x => x.Internship.Id)
            .ToListAsync();

        return internships.Select(id => new InternshipId(id)).ToList();
    }

    private async Task GenerateIdForEntity(ProcedureBase procedure)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Procedures\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new ProcedureId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = procedure.GetType().GetProperty("Id");
        idProperty?.SetValue(procedure, newId);

        await connection.CloseAsync();
    }
}