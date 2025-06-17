using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored Course repository using BaseRepository and Specifications
/// </summary>
internal sealed class RefactoredSqlCourseRepository : BaseRepository<Course>, ICourseRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public RefactoredSqlCourseRepository(
        SledzSpeckeDbContext context,
        IUnitOfWork unitOfWork) : base(context)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Course?> GetByIdAsync(CourseId id)
    {
        var specification = new CourseByIdSpecification(id);
        return await GetSingleBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetBySpecializationIdAsync(SpecializationId specializationId)
    {
        var specification = new CourseBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetByModuleIdAsync(ModuleId moduleId)
    {
        var specification = new CourseByModuleSpecification(moduleId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId)
    {
        // Note: The original implementation only filtered by specialization, not by user
        // This suggests that courses are not directly associated with users
        // Keeping the same behavior for backward compatibility
        var specification = new CourseBySpecializationSpecification(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetByModuleAsync(ModuleId moduleId)
    {
        // This appears to be a duplicate of GetByModuleIdAsync
        return await GetByModuleIdAsync(moduleId);
    }

    public async Task<IEnumerable<Course>> GetByTypeAsync(CourseType courseType)
    {
        var specification = new CourseByTypeSpecification(courseType);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetPendingApprovalAsync()
    {
        var specification = new CoursePendingApprovalSpecification();
        return await GetBySpecificationAsync(specification);
    }

    public async Task AddAsync(Course course)
    {
        // ID generation should be handled by database or a dedicated service
        // For now, keeping the existing logic but moving it to a private method
        if (course.Id.Value == 0)
        {
            await GenerateIdForEntity(course);
        }

        await AddAsync(course, default);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        // But keeping it for backward compatibility
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        Update(course);
        // Note: SaveChangesAsync should be called by Unit of Work, not here
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(CourseId id)
    {
        var course = await GetByIdAsync(id);
        if (course != null)
        {
            Remove(course);
            // Note: SaveChangesAsync should be called by Unit of Work, not here
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Additional methods using specifications
    public async Task<IEnumerable<Course>> GetMandatoryCoursesAsync(SpecializationId specializationId)
    {
        var specification = CourseSpecificationExtensions.GetMandatoryCourses(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetPendingApprovalForSpecializationAsync(SpecializationId specializationId)
    {
        var specification = CourseSpecificationExtensions.GetPendingApprovalForSpecialization(specializationId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetValidCoursesForModuleAsync(ModuleId moduleId)
    {
        var specification = CourseSpecificationExtensions.GetValidCoursesForModule(moduleId);
        return await GetBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<Course>> GetCoursesToSyncAsync()
    {
        var specification = CourseSpecificationExtensions.GetCoursesToSync();
        return await GetBySpecificationAsync(specification);
    }

    public async Task<bool> ExistsAsync(CourseId id)
    {
        var specification = new CourseByIdSpecification(id);
        return await CountBySpecificationAsync(specification) > 0;
    }

    public async Task<int> GetCourseCountForModuleAsync(ModuleId moduleId)
    {
        var specification = new CourseByModuleSpecification(moduleId);
        return await CountBySpecificationAsync(specification);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> GetPagedCoursesAsync(
        SpecializationId specializationId, 
        int pageNumber, 
        int pageSize)
    {
        var specification = new CourseBySpecializationSpecification(specializationId);
        return await GetPagedAsync(specification, pageNumber, pageSize, 
            orderBy: c => c.StartDate, ascending: false);
    }

    // Private helper methods
    private async Task GenerateIdForEntity(Course course)
    {
        // This should ideally be in a separate ID generation service
        var connection = Context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Courses\"";
        var maxId = (int)(await command.ExecuteScalarAsync() ?? 0);

        var newId = new CourseId(maxId + 1);

        // Use reflection to set the ID since it's private
        var idProperty = course.GetType().GetProperty("Id");
        idProperty?.SetValue(course, newId);

        await connection.CloseAsync();
    }
}