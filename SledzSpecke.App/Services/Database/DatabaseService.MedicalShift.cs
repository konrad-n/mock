using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<MedicalShift> GetMedicalShiftAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var shift = await this.database.Table<MedicalShift>().FirstOrDefaultAsync(s => s.ShiftId == id);
                if (shift == null)
                {
                    throw new ResourceNotFoundException(
                        $"MedicalShift with ID {id} not found",
                        $"Nie znaleziono dyżuru o ID {id}");
                }
                return shift;
            },
            new Dictionary<string, object> { { "ShiftId", id } },
            $"Nie udało się pobrać dyżuru o ID {id}");
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<MedicalShift>();

                if (internshipId.HasValue)
                {
                    query = query.Where(s => s.InternshipId == internshipId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "InternshipId", internshipId } },
            "Nie udało się pobrać listy dyżurów");
        }

        public async Task<int> SaveMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                if (shift.ShiftId != 0)
                {
                    return await this.database.UpdateAsync(shift);
                }
                else
                {
                    return await this.database.InsertAsync(shift);
                }
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId }, { "InternshipId", shift?.InternshipId } },
            "Nie udało się zapisać danych dyżuru");
        }

        public async Task<int> DeleteMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                return await this.database.DeleteAsync(shift);
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId } },
            "Nie udało się usunąć dyżuru");
        }

        public async Task MigrateShiftDataForModulesAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                bool columnExists = false;

                try
                {
                    var testQuery = "SELECT ModuleId FROM RealizedMedicalShiftNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    columnExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedMedicalShiftNewSMK ADD COLUMN ModuleId INTEGER");
                }

                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE ModuleId IS NULL OR ModuleId = 0";
                var shiftsToUpdate = await this.QueryAsync<RealizedMedicalShiftNewSMK>(query);

                if (shiftsToUpdate.Count == 0)
                {
                    return;
                }

                var specializations = await this.GetAllSpecializationsAsync();

                foreach (var specializationId in specializations.Select(x => x.SpecializationId))
                {
                    var modules = await this.GetModulesAsync(specializationId);
                    if (modules.Count == 0) continue;
                    var specializationShifts = shiftsToUpdate.Where(s => s.SpecializationId == specializationId).ToList();
                    if (specializationShifts.Count == 0) continue;

                    foreach (var shift in specializationShifts)
                    {
                        Module appropriateModule = null;
                        foreach (var module in modules)
                        {
                            if (string.IsNullOrEmpty(module.Structure)) continue;

                            var options = new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                AllowTrailingCommas = true,
                                ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                            };

                            var moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                            if (moduleStructure?.Internships != null &&
                                moduleStructure.Internships.Any(i => i.Id == shift.InternshipRequirementId))
                            {
                                appropriateModule = module;
                                break;
                            }
                        }

                        if (appropriateModule == null && modules.Count > 0)
                        {
                            appropriateModule = modules[0];
                        }

                        if (appropriateModule != null)
                        {
                            shift.ModuleId = appropriateModule.ModuleId;
                            await this.UpdateAsync(shift);
                        }
                    }
                }
            },
            null, "Nie udało się zmigrować danych dyżurów do modułów", 2, 2000);
        }
    }
}