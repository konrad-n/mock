using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ICourseService
    {
        // Istniejące metody
        Task<List<Course>> GetUserCoursesAsync();
        Task<Course> GetCourseAsync(int id);
        Task<Course> RegisterForCourseAsync(Course course);
        Task<bool> CompleteCourseAsync(int courseId, CourseDocument certificate);
        
        // Nowe metody
        Task<List<CourseDefinition>> GetRequiredCoursesAsync();
        Task<List<CourseDefinition>> GetCoursesByYearAsync(int year);
        Task<List<string>> GetCourseTopicsAsync(int courseDefinitionId);
        Task<double> GetCourseProgressAsync();
        Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync();
        Task<List<CourseDefinition>> GetRecommendedCoursesForCurrentYearAsync();
    }
}
