using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public static class ModuleHelper
    {
        public static bool IsModuleSpecialization(string specializationCode)
        {
            // Lista specjalizacji z modułami
            var moduleSpecializations =
                new[]
                {
                    "kardiologia",
                    "nefrologia",
                    "gastroenterologia",
                    "endokrynologia", "diabetologia",
                    "reumatologia",
                };

            return moduleSpecializations.Contains(specializationCode.ToLower());
        }

        public static string GetBasicModuleName(string specializationCode)
        {
            // Dla kardiologii i pokrewnych powraca "choroby wewnętrzne"
            if (new[]
                {
                    "kardiologia",
                    "nefrologia",
                    "gastroenterologia",
                    "endokrynologia",
                    "diabetologia",
                    "reumatologia",
                }
                      .Contains(specializationCode.ToLower()))
            {
                return "internal_medicine";
            }

            // Dla innych specjalizacji...
            return null;
        }

        public static List<Module> CreateModulesForSpecialization(string specializationCode, DateTime startDate)
        {
            if (!IsModuleSpecialization(specializationCode))
            {
                return new List<Module>();
            }

            string basicCode = GetBasicModuleName(specializationCode);

            if (string.IsNullOrEmpty(basicCode))
            {
                return new List<Module>();
            }

            // Tworzenie modułu podstawowego (trwa zwykle 2 lata)
            var basicModule = new Module
            {
                Type = ModuleType.Basic,
                Name = "Moduł podstawowy",
                StartDate = startDate,
                EndDate = startDate.AddYears(2),
                Structure = null, // Zostanie wypełnione później
            };

            // Tworzenie modułu specjalistycznego (zwykle 3 lata lub więcej, w zależności od specjalizacji)
            var specialisticModule = new Module
            {
                Type = ModuleType.Specialistic,
                Name = "Moduł specjalistyczny",
                StartDate = startDate.AddYears(2),
                EndDate = startDate.AddYears(5), // Standardowo 5 lat dla pełnej specjalizacji
                Structure = null, // Zostanie wypełnione później
            };

            return new List<Module> { basicModule, specialisticModule };
        }
    }
}
