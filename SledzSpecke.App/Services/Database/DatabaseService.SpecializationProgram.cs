using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<SpecializationProgram> GetSpecializationProgramAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var program = await this.database.Table<SpecializationProgram>().FirstOrDefaultAsync(p => p.ProgramId == id);
                if (program == null)
                {
                    throw new ResourceNotFoundException(
                        $"SpecializationProgram with ID {id} not found",
                        $"Nie znaleziono programu specjalizacji o ID {id}");
                }
                return program;
            },
            new Dictionary<string, object> { { "ProgramId", id } },
            $"Nie udało się pobrać programu specjalizacji o ID {id}");
        }

        public async Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, SmkVersion smkVersion)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidInputException(
                        "Code cannot be null or empty",
                        "Kod nie może być pusty");
                }

                return await this.database.Table<SpecializationProgram>()
                    .FirstOrDefaultAsync(p => p.Code == code && p.SmkVersion == smkVersion);
            },
            new Dictionary<string, object> { { "Code", code }, { "SmkVersion", smkVersion } },
            $"Nie udało się pobrać programu specjalizacji o kodzie {code}");
        }

        public async Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<SpecializationProgram>().ToListAsync();
            }, null, "Nie udało się pobrać listy programów specjalizacji");
        }

        public async Task<int> SaveSpecializationProgramAsync(SpecializationProgram program)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (program == null)
                {
                    throw new InvalidInputException(
                        "SpecializationProgram cannot be null",
                        "Program specjalizacji nie może być pusty");
                }

                if (program.ProgramId != 0)
                {
                    return await this.database.UpdateAsync(program);
                }
                else
                {
                    return await this.database.InsertAsync(program);
                }
            },
            new Dictionary<string, object> { { "Program", program?.ProgramId } },
            "Nie udało się zapisać danych programu specjalizacji");
        }
    }
}