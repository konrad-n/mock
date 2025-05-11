using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Internship with ID {id} not found",
                        $"Nie znaleziono stażu o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "InternshipId", id } },
            $"Nie udało się pobrać stażu o ID {id}");
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Internship>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId }
            },
            "Nie udało się pobrać listy staży");
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                if (internship.InternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "Internship", internship?.InternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId }
            },
            "Nie udało się zapisać danych stażu");
        }

        public async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "Internship", internship?.InternshipId } },
            "Nie udało się usunąć stażu");
        }
    }
}