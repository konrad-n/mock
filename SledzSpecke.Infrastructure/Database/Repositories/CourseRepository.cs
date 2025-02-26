using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(IApplicationDbContext context) : base(context) { }

        public async Task<List<Course>> GetUserCoursesAsync(int userId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<Course>()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId)
        {
            await _context.InitializeAsync();
            return await _connection.Table<CourseDefinition>()
                .Where(c => c.SpecializationId == specializationId && c.IsRequired)
                .OrderBy(c => c.RecommendedYear)
                .ToListAsync();
        }

        public async Task<double> GetCourseProgressAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();
            var requiredCourses = await GetRequiredCoursesAsync(specializationId);
            var userCourses = await GetUserCoursesAsync(userId);
            
            if (requiredCourses.Count == 0)
                return 1.0; // 100% jeśli nie ma wymaganych kursów
            
            var completedCount = userCourses.Count(c => c.IsCompleted);
            return (double)completedCount / requiredCourses.Count;
        }

        public async Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync(int userId, int specializationId)
        {
            await _context.InitializeAsync();
            var requiredCourses = await GetRequiredCoursesAsync(specializationId);
            var userCourses = await GetUserCoursesAsync(userId);
            
            var progressByYear = requiredCourses
                .GroupBy(c => c.RecommendedYear)
                .ToDictionary(
                    g => $"Rok {g.Key}",
                    g => (Required: g.Count(), Completed: 0)
                );
            
            foreach (var course in userCourses.Where(c => c.IsCompleted))
            {
                // Znajdź odpowiadającą definicję kursu
                var definition = requiredCourses.FirstOrDefault(d => d.Id == course.CourseDefinitionId);
                if (definition != null)
                {
                    var year = $"Rok {definition.RecommendedYear}";
                    if (progressByYear.ContainsKey(year))
                    {
                        var current = progressByYear[year];
                        progressByYear[year] = (current.Required, current.Completed + 1);
                    }
                }
            }
            
            return progressByYear;
        }
    }
}
