using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetUserCoursesAsync();
        Task<List<CourseDefinition>> GetRequiredCoursesAsync();
        Task<double> GetCourseProgressAsync();
        Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync();
    }
}
