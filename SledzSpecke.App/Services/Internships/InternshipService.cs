using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.Services.Internships
{
    public class InternshipService : IInternshipService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public InternshipService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService)
        {
            this.databaseService = databaseService;
            this.authService = authService;
            this.specializationService = specializationService;
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null)
        {
            var currentUser = await this.authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return new List<RealizedInternshipNewSMK>();
            }

            int specializationId = currentUser.SpecializationId;
            var realizations = await this.databaseService.GetRealizedInternshipsNewSMKAsync(
                specializationId,
                moduleId,
                internshipRequirementId);

            // Pobieramy informacje o nazwach staży
            foreach (var realization in realizations)
            {
                if (realization.InternshipRequirementId > 0)
                {
                    var requirement = await this.databaseService.GetInternshipAsync(realization.InternshipRequirementId);
                    if (requirement != null)
                    {
                        realization.InternshipName = requirement.InternshipName;
                    }
                }
            }

            return realizations;
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            var realization = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
            if (realization != null && realization.InternshipRequirementId > 0)
            {
                var requirement = await this.databaseService.GetInternshipAsync(realization.InternshipRequirementId);
                if (requirement != null)
                {
                    realization.InternshipName = requirement.InternshipName;
                }
            }
            return realization;
        }

        public async Task<bool> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK realizedInternship)
        {
            if (realizedInternship.SpecializationId <= 0)
            {
                var currentUser = await this.authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return false;
                }
                realizedInternship.SpecializationId = currentUser.SpecializationId;
            }

            int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(realizedInternship);

            if (result > 0 && realizedInternship.ModuleId.HasValue)
            {
                await this.specializationService.UpdateModuleProgressAsync(realizedInternship.ModuleId.Value);
            }

            return result > 0;
        }

        public async Task<bool> DeleteRealizedInternshipNewSMKAsync(int id)
        {
            var realization = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
            if (realization == null)
            {
                return false;
            }

            int result = await this.databaseService.DeleteRealizedInternshipNewSMKAsync(realization);

            if (result > 0 && realization.ModuleId.HasValue)
            {
                await this.specializationService.UpdateModuleProgressAsync(realization.ModuleId.Value);
            }

            return result > 0;
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int year = 0, string internshipName = null)
        {
            var currentUser = await this.authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return new List<RealizedInternshipOldSMK>();
            }

            int specializationId = currentUser.SpecializationId;
            return await this.databaseService.GetRealizedInternshipsOldSMKAsync(
                specializationId,
                year,
                internshipName);
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            return await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
        }

        public async Task<bool> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK realizedInternship)
        {
            if (realizedInternship.SpecializationId <= 0)
            {
                var currentUser = await this.authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return false;
                }
                realizedInternship.SpecializationId = currentUser.SpecializationId;
            }

            int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(realizedInternship);

            // W przypadku starego SMK, aktualizujemy postęp dla całej specjalizacji
            if (result > 0)
            {
                var currentUser = await this.authService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    await this.specializationService.UpdateSpecializationProgressAsync(currentUser.SpecializationId);
                }
            }

            return result > 0;
        }

        public async Task<bool> DeleteRealizedInternshipOldSMKAsync(int id)
        {
            var realization = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
            if (realization == null)
            {
                return false;
            }

            int result = await this.databaseService.DeleteRealizedInternshipOldSMKAsync(realization);

            // W przypadku starego SMK, aktualizujemy postęp dla całej specjalizacji
            if (result > 0)
            {
                var currentUser = await this.authService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    await this.specializationService.UpdateSpecializationProgressAsync(currentUser.SpecializationId);
                }
            }

            return result > 0;
        }

        public async Task<InternshipSummary> GetInternshipSummaryAsync(int internshipRequirementId, int? moduleId = null)
        {
            var summary = new InternshipSummary();

            // Pobieramy wymaganie stażu
            var requirement = await this.databaseService.GetInternshipAsync(internshipRequirementId);
            if (requirement == null)
            {
                return summary;
            }

            summary.RequiredDays = requirement.DaysCount;

            // Pobieramy bieżącego użytkownika
            var currentUser = await this.authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return summary;
            }

            // Pobieramy realizacje dla tego wymagania (dla nowego SMK)
            if (currentUser.SmkVersion == SmkVersion.New)
            {
                var realizations = await this.databaseService.GetRealizedInternshipsNewSMKAsync(
                    currentUser.SpecializationId,
                    moduleId,
                    internshipRequirementId);

                foreach (var realization in realizations)
                {
                    summary.CompletedDays += realization.DaysCount;
                }
            }
            // Dla starego SMK, szukamy po nazwie stażu
            else
            {
                if (requirement.InternshipName != null)
                {
                    var realizations = await this.databaseService.GetRealizedInternshipsOldSMKAsync(
                        currentUser.SpecializationId,
                        0, // wszystkie lata
                        requirement.InternshipName);

                    foreach (var realization in realizations)
                    {
                        summary.CompletedDays += realization.DaysCount;
                    }
                }
            }

            // Pobieramy informacje o dniach uznanych
            var recognitions = await this.databaseService.GetRecognitionsAsync(currentUser.SpecializationId);
            var internshipRecognitions = recognitions.Where(r =>
                r.Type == RecognitionType.SpecializationInternship &&
                r.Description != null &&
                r.Description.Contains(requirement.InternshipName, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var recognition in internshipRecognitions)
            {
                summary.RecognizedDays += recognition.DaysReduction;
            }

            // Możemy również uwzględnić dni samokształcenia, jeśli potrzeba
            // Tutaj dodałbym kod liczenia dni samokształcenia przypisanych do danego stażu

            return summary;
        }
    }
}