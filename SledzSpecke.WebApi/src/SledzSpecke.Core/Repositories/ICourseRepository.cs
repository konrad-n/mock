using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(CourseId id);
    Task<IEnumerable<Course>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Course>> GetByModuleIdAsync(ModuleId moduleId);
    Task<IEnumerable<Course>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Course>> GetByModuleAsync(ModuleId moduleId);
    Task<IEnumerable<Course>> GetByTypeAsync(CourseType courseType);
    Task<IEnumerable<Course>> GetPendingApprovalAsync();
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(CourseId id);
}