using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface ICourseRepository : IBaseRepository<Course>
    {
        // Istniejące metody
        Task<List<Course>> GetUserCoursesAsync(int userId);
        
        // Nowe metody
        Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId);
        Task<List<CourseDefinition>> GetCoursesByYearAsync(int specializationId, int year);
        Task<List<string>> GetCourseTopicsAsync(int courseDefinitionId);
        Task<double> GetCourseProgressAsync(int userId, int specializationId);
        Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync(int userId, int specializationId);
    }
}
