using System;
using System.Collections.Generic;
using System.Linq;
using SledzSpecke.Core.Models.Requirements;

namespace SledzSpecke.Core.Models.Monitoring
{
    public static class DutyMonitoring
    {
        public class Duty
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Type { get; set; }
            public string Location { get; set; }
            public string Supervisor { get; set; }
            public int EmergencyCases { get; set; }
            public List<string> PerformedProcedures { get; set; } = new List<string>();
            public bool WasSupervised { get; set; }
            public List<string> KeyCompetencies { get; set; } = new List<string>();
            public double DurationInHours => (EndTime - StartTime).TotalHours;
        }

        public class DutyStats
        {
            public int TotalDuties { get; set; }
            public double TotalHours { get; set; }
            public int RegularDuties { get; set; }
            public int EmergencyDuties { get; set; }
            public int SupervisedDuties { get; set; }
            public int IndependentDuties { get; set; }
            public int TotalEmergencyCases { get; set; }
            public List<string> ProceduresPerformed { get; set; } = new List<string>();
        }

        public class DutyValidator
        {
            public (bool IsCompliant, List<string> Deficiencies) CheckMonthlyCompliance(
                int specializationId, int specializationYear, List<Duty> monthlyDuties)
            {
                var requirements = DutyRequirements.GetDutyRequirementsBySpecialization(specializationId)
                    .FirstOrDefault(r => r.Year == specializationYear);

                var deficiencies = new List<string>();

                if (requirements == null)
                {
                    deficiencies.Add($"Brak zdefiniowanych wymagań dla roku {specializationYear}");
                    return (false, deficiencies);
                }

                // Sprawdzanie liczby godzin
                var totalHours = monthlyDuties.Sum(d => d.DurationInHours);
                if (totalHours < requirements.MinimumHoursPerMonth)
                {
                    deficiencies.Add($"Brakuje {requirements.MinimumHoursPerMonth - totalHours:F1} godzin dyżurowych");
                }

                // Sprawdzanie liczby dyżurów
                if (monthlyDuties.Count < requirements.MinimumDutiesPerMonth)
                {
                    deficiencies.Add($"Brakuje {requirements.MinimumDutiesPerMonth - monthlyDuties.Count} dyżurów");
                }

                // Sprawdzanie nadzoru
                if (requirements.RequiresSupervision && monthlyDuties.Any(d => !d.WasSupervised))
                {
                    deficiencies.Add("Niektóre dyżury odbyły się bez wymaganego nadzoru");
                }

                return (deficiencies.Count == 0, deficiencies);
            }

            public DutyStats GenerateStatistics(List<Duty> duties)
            {
                return new DutyStats
                {
                    TotalDuties = duties.Count,
                    TotalHours = duties.Sum(d => d.DurationInHours),
                    RegularDuties = duties.Count(d => d.Type == "Regular"),
                    EmergencyDuties = duties.Count(d => d.Type == "Emergency"),
                    SupervisedDuties = duties.Count(d => d.WasSupervised),
                    IndependentDuties = duties.Count(d => !d.WasSupervised),
                    TotalEmergencyCases = duties.Sum(d => d.EmergencyCases),
                    ProceduresPerformed = duties.SelectMany(d => d.PerformedProcedures).ToList()
                };
            }

            public string GenerateReport(DutyStats stats)
            {
                return $@"Raport dyżurowy:
    - Całkowita liczba dyżurów: {stats.TotalDuties}
    - Całkowita liczba godzin: {stats.TotalHours:F1}
    - Dyżury zwykłe: {stats.RegularDuties}
    - Dyżury ostre: {stats.EmergencyDuties}
    - Dyżury pod nadzorem: {stats.SupervisedDuties}
    - Dyżury samodzielne: {stats.IndependentDuties}
    - Przypadki nagłe: {stats.TotalEmergencyCases}
    - Liczba wykonanych procedur: {stats.ProceduresPerformed.Count}";
            }
        }
    }
}
