using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public class ProgressCalculator
    {
        // Aktualizacja statystyk postępu dla modułu
        public static async Task UpdateModuleProgressAsync(
            IDatabaseService database,
            int moduleId)
        {
            var module = await database.GetModuleAsync(moduleId);
            if (module == null) return;

            // Pobranie internships dla modułu
            var internships = await database.GetInternshipsAsync(moduleId: moduleId);
            var completedInternships = internships.Count(i => i.IsCompleted);

            // Pobranie kursów dla modułu
            var courses = await database.GetCoursesAsync(moduleId: moduleId);

            // Pobranie procedur powiązanych z internships w module
            var procedures = new List<Procedure>();
            foreach (var internship in internships)
            {
                var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                procedures.AddRange(internshipProcedures);
            }

            // Zliczanie procedur typu A i B
            var proceduresA = procedures.Count(p => p.OperatorCode == "A");
            var proceduresB = procedures.Count(p => p.OperatorCode == "B");

            // Wczytanie definicji z programu modułu
            var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure);

            // Aktualizacja statystyk modułu
            module.CompletedInternships = completedInternships;
            module.TotalInternships = moduleStructure.Internships?.Count ?? 0;
            module.CompletedCourses = courses.Count;
            module.TotalCourses = moduleStructure.Courses?.Count ?? 0;
            module.CompletedProceduresA = proceduresA;
            module.TotalProceduresA = moduleStructure.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
            module.CompletedProceduresB = proceduresB;
            module.TotalProceduresB = moduleStructure.Procedures?.Sum(p => p.RequiredCountB) ?? 0;

            // Zapisanie zaktualizowanego modułu
            await database.UpdateModuleAsync(module);

            // Aktualizacja globalnych statystyk dla specjalizacji
            await UpdateSpecializationProgressAsync(database, module.SpecializationId);
        }

        // Aktualizacja statystyk dla całej specjalizacji
        public static async Task UpdateSpecializationProgressAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null) return;

            if (specialization.HasModules)
            {
                // Jeśli specjalizacja ma moduły, agreguj dane z modułów
                var modules = await database.GetModulesAsync(specializationId);

                specialization.CompletedInternships = modules.Sum(m => m.CompletedInternships);
                specialization.TotalInternships = modules.Sum(m => m.TotalInternships);
                specialization.CompletedCourses = modules.Sum(m => m.CompletedCourses);
                specialization.TotalCourses = modules.Sum(m => m.TotalCourses);
            }
            else
            {
                // Jeśli nie ma modułów, oblicz statystyki bezpośrednio
                var internships = await database.GetInternshipsAsync(specializationId: specializationId);
                specialization.CompletedInternships = internships.Count(i => i.IsCompleted);

                var courses = await database.GetCoursesAsync(specializationId: specializationId);
                specialization.CompletedCourses = courses.Count;

                // Wczytanie definicji z programu specjalizacji dla wymaganych wartości
                var specializationStructure = JsonSerializer.Deserialize<SpecializationStructure>(
                    specialization.ProgramStructure);

                specialization.TotalInternships = specializationStructure.Internships?.Count ?? 0;
                specialization.TotalCourses = specializationStructure.Courses?.Count ?? 0;
            }

            // Zapisanie zaktualizowanej specjalizacji
            await database.UpdateSpecializationAsync(specialization);
        }
    }
}
