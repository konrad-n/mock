using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetUserCoursesAsync(int userId);
        Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId);
        Task<Course> RegisterForCourseAsync(Course course);
        Task<bool> CompleteCourseAsync(int courseId, CourseDocument certificate);
        Task<List<CourseNotification>> GetActiveNotificationsAsync(int specializationId);
        Task<double> GetCourseProgressAsync(int userId);
        Task<CourseDocument> AddDocumentAsync(int courseId, CourseDocument document);
    }
}
