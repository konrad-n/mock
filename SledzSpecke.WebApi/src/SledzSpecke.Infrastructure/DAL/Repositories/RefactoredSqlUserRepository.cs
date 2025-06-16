using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;
using System.Data;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored User repository using BaseRepository and Specifications
/// This implementation demonstrates world-class architecture with:
/// - Specification pattern for composable queries
/// - Inheritance from BaseRepository for common operations
/// - Clean separation of concerns
/// - Maintainable and testable code
/// </summary>
internal sealed class RefactoredSqlUserRepository : BaseRepository<User>, IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlUserRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        // Use the base repository method with conversion
        return await GetByIdAsync(id.Value, default);
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        // Use the base repository method with conversion
        return await GetByIdAsync(id.Value, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(Username username)
    {
        // TODO: Username no longer exists in User entity - this method should be removed
        return null;
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        var specification = new UserByEmailSpecification(email);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await GetAllAsync(default);
    }

    public async Task<UserId> AddAsync(User user)
    {
        // Use PostgreSQL sequence for ID generation
        var newId = await GetNextUserIdAsync();
        user.SetId(newId);
        
        await AddAsync(user, default);
        
        // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
        // Keeping it here for backward compatibility with existing implementation
        await _unitOfWork.SaveChangesAsync();
        
        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        Update(user);
        
        // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserId id)
    {
        var user = await GetByIdAsync(id);
        if (user is not null)
        {
            Remove(user);
            
            // Note: SaveChangesAsync should ideally be called by Unit of Work pattern
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByUsernameAsync(Username username)
    {
        // TODO: Username no longer exists in User entity - this method should be removed
        return false;
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        var specification = new UserByEmailSpecification(email);
        return await AnyAsync(specification);
    }

    // Additional methods leveraging specifications for common queries
    
    /// <summary>
    /// Get users by specialization using specification pattern
    /// </summary>
    public async Task<IEnumerable<User>> GetBySpecializationAsync(SpecializationId specializationId)
    {
        // TODO: User no longer has SpecializationId - need to redesign User-Specialization relationship
        return Enumerable.Empty<User>();
    }

    /// <summary>
    /// Get active users (recently logged in) using specification pattern
    /// </summary>
    public async Task<IEnumerable<User>> GetActiveUsersAsync(int daysThreshold = 30)
    {
        var specification = UserSpecificationExtensions.GetActiveUsers(daysThreshold);
        return await GetBySpecificationAsync(specification);
    }

    /// <summary>
    /// Search users by multiple criteria using composable specifications
    /// </summary>
    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        var specification = UserSpecificationExtensions.SearchUsers(searchTerm);
        return await GetBySpecificationAsync(specification);
    }

    /// <summary>
    /// Get users with complete profiles using specification
    /// </summary>
    public async Task<IEnumerable<User>> GetUsersWithCompleteProfilesAsync()
    {
        // TODO: Update UserByProfileCompleteSpecification for new User entity structure
        // For now, all users have complete profiles as all fields are required
        return await GetAllAsync();
    }

    /// <summary>
    /// Get paginated users using specification with pagination
    /// </summary>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPaginatedUsersAsync(
        ISpecification<User> specification,
        int pageNumber,
        int pageSize)
    {
        var result = await GetPagedAsync(specification, pageNumber, pageSize);
        return (result.Items, result.TotalCount);
    }

    // Private helper methods
    
    /// <summary>
    /// PostgreSQL sequence-based ID generation
    /// This maintains compatibility with existing database schema
    /// </summary>
    private async Task<UserId> GetNextUserIdAsync()
    {
        // Create sequence if it doesn't exist and get next value
        await using var command = Context.Database.GetDbConnection().CreateCommand();
        command.CommandText = @"
            CREATE SEQUENCE IF NOT EXISTS user_id_seq START WITH 2;
            SELECT nextval('user_id_seq')::integer";
        
        await Context.Database.OpenConnectionAsync();
        var result = await command.ExecuteScalarAsync();
        await Context.Database.CloseConnectionAsync();
        
        return new UserId(Convert.ToInt32(result));
    }

    /// <summary>
    /// Helper method to check if any user matches a specification
    /// </summary>
    private async Task<bool> AnyAsync(ISpecification<User> specification)
    {
        return await Context.Users.AnyAsync(specification.ToExpression());
    }
}