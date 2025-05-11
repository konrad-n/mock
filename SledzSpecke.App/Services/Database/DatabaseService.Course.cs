using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Course> GetCourseAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var course = await this.database.Table<Course>().FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    throw new ResourceNotFoundException(
                        $"Course with ID {id} not found",
                        $"Nie znaleziono kursu o ID {id}");
                }
                return course;
            },
            new Dictionary<string, object> { { "CourseId", id } },
            $"Nie udało się pobrać kursu o ID {id}");
        }

        public async Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Course>();

                if (specializationId.HasValue)
                {
                    query = query.Where(c => c.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(c => c.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy kursów");
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (course == null)
                {
                    throw new InvalidInputException(
                        "Course cannot be null",
                        "Kurs nie może być pusty");
                }

                if (course.CourseId != 0)
                {
                    return await this.database.UpdateAsync(course);
                }
                else
                {
                    return await this.database.InsertAsync(course);
                }
            },
            new Dictionary<string, object> {
                { "Course", course?.CourseId },
                { "SpecializationId", course?.SpecializationId },
                { "ModuleId", course?.ModuleId }
            },
            "Nie udało się zapisać danych kursu");
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (course == null)
                {
                    throw new InvalidInputException(
                        "Course cannot be null",
                        "Kurs nie może być pusty");
                }

                return await this.database.DeleteAsync(course);
            },
            new Dictionary<string, object> { { "Course", course?.CourseId } },
            "Nie udało się usunąć kursu");
        }
    }
}