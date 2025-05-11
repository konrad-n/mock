using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Publication> GetPublicationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var publication = await this.database.Table<Publication>().FirstOrDefaultAsync(p => p.PublicationId == id);
                if (publication == null)
                {
                    throw new ResourceNotFoundException(
                        $"Publication with ID {id} not found",
                        $"Nie znaleziono publikacji o ID {id}");
                }
                return publication;
            },
            new Dictionary<string, object> { { "PublicationId", id } },
            $"Nie udało się pobrać publikacji o ID {id}");
        }

        public async Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Publication>();

                if (specializationId.HasValue)
                {
                    query = query.Where(p => p.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(p => p.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy publikacji");
        }

        public async Task<int> SavePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (publication == null)
                {
                    throw new InvalidInputException(
                        "Publication cannot be null",
                        "Publikacja nie może być pusta");
                }

                if (publication.PublicationId != 0)
                {
                    return await this.database.UpdateAsync(publication);
                }
                else
                {
                    return await this.database.InsertAsync(publication);
                }
            },
            new Dictionary<string, object> {
                { "Publication", publication?.PublicationId },
                { "SpecializationId", publication?.SpecializationId },
                { "ModuleId", publication?.ModuleId }
            },
            "Nie udało się zapisać danych publikacji");
        }

        public async Task<int> DeletePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (publication == null)
                {
                    throw new InvalidInputException(
                        "Publication cannot be null",
                        "Publikacja nie może być pusta");
                }

                return await this.database.DeleteAsync(publication);
            },
            new Dictionary<string, object> { { "Publication", publication?.PublicationId } },
            "Nie udało się usunąć publikacji");
        }
    }
}