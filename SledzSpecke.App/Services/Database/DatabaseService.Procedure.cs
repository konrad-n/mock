using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<Procedure> GetProcedureAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var procedure = await this.database.Table<Procedure>().FirstOrDefaultAsync(p => p.ProcedureId == id);
                if (procedure == null)
                {
                    throw new ResourceNotFoundException(
                        $"Procedure with ID {id} not found",
                        $"Nie znaleziono procedury o ID {id}");
                }
                return procedure;
            },
            new Dictionary<string, object> { { "ProcedureId", id } },
            $"Nie udało się pobrać procedury o ID {id}");
        }

        public async Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Procedure>();

                if (internshipId.HasValue)
                {
                    query = query.Where(p => p.InternshipId == internshipId);
                }

                var procedures = await query.ToListAsync();

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLowerInvariant();
                    procedures = procedures.Where(p =>
                        p.Code.ToLowerInvariant().Contains(searchText) ||
                        p.Location.ToLowerInvariant().Contains(searchText) ||
                        (p.PatientInitials != null && p.PatientInitials.ToLowerInvariant().Contains(searchText)) ||
                        (p.ProcedureGroup != null && p.ProcedureGroup.ToLowerInvariant().Contains(searchText))
                    ).ToList();
                }

                return procedures;
            },
            new Dictionary<string, object> { { "InternshipId", internshipId }, { "SearchText", searchText } },
            "Nie udało się pobrać listy procedur");
        }

        public async Task<int> SaveProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                if (procedure.ProcedureId != 0)
                {
                    return await this.database.UpdateAsync(procedure);
                }
                else
                {
                    return await this.database.InsertAsync(procedure);
                }
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId }, { "InternshipId", procedure?.InternshipId } },
            "Nie udało się zapisać danych procedury");
        }

        public async Task<int> DeleteProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                return await this.database.DeleteAsync(procedure);
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId } },
            "Nie udało się usunąć procedury");
        }
    }
}