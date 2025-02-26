using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface ICourseRepository : IBaseRepository<Course>
    {
        Task<List<Course>> GetUserCoursesAsync(int userId);        
        Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId);
        Task<double> GetCourseProgressAsync(int userId, int specializationId);
        Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync(int userId, int specializationId);
    }
}
