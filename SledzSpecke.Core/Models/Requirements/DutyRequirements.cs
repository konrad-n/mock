using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.Core.Models.Requirements
{
    public static class DutyRequirements
    {
        public class DutySpecification
        {
            public string Type { get; set; } // Regular, Emergency, Supervised, Independent
            public int MinimumHoursPerMonth { get; set; }
            public int MinimumDutiesPerMonth { get; set; }
            public bool RequiresSupervision { get; set; }
            public List<string> RequiredCompetencies { get; set; }
            public int Year { get; set; }
            public int SpecializationId { get; set; } // ID specjalizacji, do której należy specyfikacja
        }

        public static List<DutySpecification> GetDutyRequirements()
        {
            return new List<DutySpecification>
            {
                new DutySpecification
                {
                    Year = 1,
                    MinimumHoursPerMonth = 40,
                    MinimumDutiesPerMonth = 4,
                    RequiresSupervision = true,
                    Type = "Supervised",
                    SpecializationId = 1, // Hematologia
                    RequiredCompetencies = new List<string>
                    {
                        "Ocena stanu pacjenta",
                        "Postępowanie w gorączce neutropenicznej",
                        "Postępowanie w zespole lizy guza",
                        "Kwalifikacja do przetoczenia składników krwi"
                    }
                },
                new DutySpecification
                {
                    Year = 1,
                    MinimumHoursPerMonth = 40,
                    MinimumDutiesPerMonth = 4,
                    RequiresSupervision = true,
                    Type = "Supervised",
                    SpecializationId = 2, // Medycyna morska i tropikalna
                    RequiredCompetencies = new List<string>
                    {
                        "Ocena stanu pacjenta",
                        "Podstawowe procedury diagnostyczne w medycynie morskiej i tropikalnej",
                        "Postępowanie w stanach nagłych związanych z podróżami i pracą w tropiku",
                        "Kwalifikacja do leczenia w trybie pilnym"
                    }
                },
                // Pozostałe specyfikacje...
            };
        }

        public static List<DutySpecification> GetDutyRequirementsBySpecialization(int specializationId)
        {
            return GetDutyRequirements().Where(dr => dr.SpecializationId == specializationId).ToList();
        }
    }
}
