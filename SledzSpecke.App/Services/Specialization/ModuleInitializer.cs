using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Services.Specialization
{
    public class ModuleInitializer
    {
        private readonly IDatabaseService databaseService;

        public ModuleInitializer(IDatabaseService databaseService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public async Task<bool> InitializeModulesIfNeededAsync(int specializationId)
        {
            var specialization = await databaseService.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return false;
            }

            var existingModules = await databaseService.GetModulesAsync(specializationId);
            if (existingModules?.Count > 0)
            {
                return true;
            }

            var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                specialization.ProgramCode,
                specialization.StartDate,
                specialization.SmkVersion,
                specializationId);

            databaseService.ClearCache();

            return true;
        }
    }
}