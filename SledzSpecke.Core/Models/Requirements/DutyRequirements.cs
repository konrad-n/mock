using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.Core.Models.Requirements
{
    public static class DutyRequirements
    {
        public class DutySpecification
        {
            public string Type { get; set; }
            public int MinimumHoursPerMonth { get; set; }
            public int MinimumDutiesPerMonth { get; set; }
            public bool RequiresSupervision { get; set; }
            public List<string> RequiredCompetencies { get; set; }
            public int Year { get; set; }
            public int SpecializationId { get; set; }
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
                    SpecializationId = 1,
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
                    SpecializationId = 2,
                    RequiredCompetencies = new List<string>
                    {
                        "Ocena stanu pacjenta",
                        "Podstawowe procedury diagnostyczne w medycynie morskiej i tropikalnej",
                        "Postępowanie w stanach nagłych związanych z podróżami i pracą w tropiku",
                        "Kwalifikacja do leczenia w trybie pilnym"
                    }
                },
                new DutySpecification
                    {
                        Year = 3,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = false,
                        Type = "Independent",
                        SpecializationId = 1,
                        RequiredCompetencies = new List<string>
                        {
                            "Samodzielne prowadzenie dyżuru",
                            "Koordynacja pracy zespołu",
                            "Podejmowanie decyzji w stanach zagrożenia życia",
                            "Nadzór nad młodszymi kolegami"
                        }
                    },
                    new DutySpecification
                    {
                        Year = 2,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = false,
                        Type = "Independent",
                        SpecializationId = 2,
                        RequiredCompetencies = new List<string>
                        {
                            "Samodzielne prowadzenie dyżuru",
                            "Zaawansowane procedury w medycynie morskiej i tropikalnej",
                            "Udzielanie porad medycznych drogą radiową",
                            "Koordynacja pracy zespołu dyżurowego"
                        }
                    },
            };
        }

        public static List<DutySpecification> GetDutyRequirementsBySpecialization(int specializationId)
        {
            return GetDutyRequirements().Where(dr => dr.SpecializationId == specializationId).ToList();
        }
    }
}
