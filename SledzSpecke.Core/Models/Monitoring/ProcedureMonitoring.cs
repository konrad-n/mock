using SledzSpecke.Core.Models.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SledzSpecke.Core.Models.Monitoring
{
    public static class ProcedureMonitoring
    {
        public class ProcedureProgress
        {
            public string ProcedureName { get; set; }
            public int CompletedCount { get; set; }
            public int AssistanceCount { get; set; }
            public int SimulationCount { get; set; }
            public List<ProcedureExecution> Executions { get; set; } = new List<ProcedureExecution>();
        }

        public class ProcedureExecution
        {
            public DateTime ExecutionDate { get; set; }
            public string Type { get; set; }
            public string SupervisorName { get; set; }
            public string Location { get; set; }
            public string PatientId { get; set; }
            public string Notes { get; set; }
        }

        public class ProgressVerification
        {
            private readonly Dictionary<string, List<RequiredProcedure>> _procedureRequirements;
            public ProgressVerification(Dictionary<string, List<RequiredProcedure>> procedureRequirements)
            {
                _procedureRequirements = procedureRequirements;
            }

            public (bool IsComplete, List<string> Deficiencies) VerifyProgress(
                Dictionary<string, ProcedureProgress> completedProcedures)
            {
                var deficiencies = new List<string>();

                foreach (var category in _procedureRequirements.Keys)
                {
                    foreach (var requiredProcedure in _procedureRequirements[category])
                    {
                        if (!completedProcedures.TryGetValue(requiredProcedure.Name, out var progress))
                        {
                            deficiencies.Add($"Brak wykonanych procedur: {requiredProcedure.Name}");
                            continue;
                        }

                        if (progress.CompletedCount < requiredProcedure.RequiredCount)
                        {
                            deficiencies.Add($"Niewystarczająca liczba wykonań {requiredProcedure.Name}: " +
                                             $"wykonano {progress.CompletedCount}/{requiredProcedure.RequiredCount}");
                        }

                        if (progress.AssistanceCount < requiredProcedure.AssistanceCount)
                        {
                            deficiencies.Add($"Niewystarczająca liczba asyst {requiredProcedure.Name}: " +
                                             $"wykonano {progress.AssistanceCount}/{requiredProcedure.AssistanceCount}");
                        }

                        if (requiredProcedure.AllowSimulation)
                        {
                            var maxSimulations = (requiredProcedure.RequiredCount * requiredProcedure.SimulationLimit.Value) / 100;
                            if (progress.SimulationCount > maxSimulations)
                            {
                                deficiencies.Add($"Przekroczony limit symulacji dla {requiredProcedure.Name}: " +
                                                 $"wykonano {progress.SimulationCount}, " +
                                                 $"maksymalnie dozwolone {maxSimulations}");
                            }
                        }
                    }
                }

                return (deficiencies.Count == 0, deficiencies);
            }

            public ProgressSummary GenerateProgressSummary(Dictionary<string, ProcedureProgress> completedProcedures)
            {
                var summary = new ProgressSummary();

                foreach (var category in _procedureRequirements.Keys)
                {
                    var categorySummary = new CategorySummary
                    {
                        CategoryName = category,
                        TotalRequired = _procedureRequirements[category].Sum(p => p.RequiredCount),
                        TotalAssistanceRequired = _procedureRequirements[category].Sum(p => p.AssistanceCount),
                        CompletedCount = 0,
                        AssistanceCount = 0,
                        SimulationCount = 0,
                        CompletionPercentage = 0
                    };

                    foreach (var procedure in _procedureRequirements[category])
                    {
                        if (completedProcedures.TryGetValue(procedure.Name, out var progress))
                        {
                            categorySummary.CompletedCount += progress.CompletedCount;
                            categorySummary.AssistanceCount += progress.AssistanceCount;
                            categorySummary.SimulationCount += progress.SimulationCount;
                        }
                    }

                    var totalRequired = categorySummary.TotalRequired + categorySummary.TotalAssistanceRequired;
                    var totalCompleted = categorySummary.CompletedCount + categorySummary.AssistanceCount;

                    categorySummary.CompletionPercentage = totalRequired > 0
                        ? (totalCompleted * 100.0) / totalRequired
                        : 0;

                    summary.Categories.Add(categorySummary);
                }

                summary.CalculateOverallProgress();
                return summary;
            }
        }

        public class ProgressSummary
        {
            public List<CategorySummary> Categories { get; set; } = new List<CategorySummary>();
            public double OverallCompletionPercentage { get; set; }
            public int TotalProceduresRequired { get; set; }
            public int TotalProceduresCompleted { get; set; }
            public int TotalSimulationsUsed { get; set; }

            public void CalculateOverallProgress()
            {
                TotalProceduresRequired = Categories.Sum(c => c.TotalRequired + c.TotalAssistanceRequired);
                TotalProceduresCompleted = Categories.Sum(c => c.CompletedCount + c.AssistanceCount);
                TotalSimulationsUsed = Categories.Sum(c => c.SimulationCount);

                OverallCompletionPercentage = TotalProceduresRequired > 0
                    ? (TotalProceduresCompleted * 100.0) / TotalProceduresRequired
                    : 0;
            }

            public string GenerateReport()
            {
                var report = new StringBuilder();
                report.AppendLine("Raport postępu wykonania procedur medycznych:");
                report.AppendLine($"Całkowity postęp: {OverallCompletionPercentage:F1}%");
                report.AppendLine($"Wykonane procedury: {TotalProceduresCompleted}/{TotalProceduresRequired}");
                report.AppendLine($"Wykorzystane symulacje: {TotalSimulationsUsed}");
                report.AppendLine();

                foreach (var category in Categories)
                {
                    report.AppendLine($"Kategoria: {category.CategoryName}");
                    report.AppendLine($"- Postęp: {category.CompletionPercentage:F1}%");
                    report.AppendLine($"- Wykonane: {category.CompletedCount}/{category.TotalRequired}");
                    report.AppendLine($"- Asysty: {category.AssistanceCount}/{category.TotalAssistanceRequired}");
                    report.AppendLine($"- Symulacje: {category.SimulationCount}");
                    report.AppendLine();
                }

                return report.ToString();
            }
        }

        public class CategorySummary
        {
            public string CategoryName { get; set; }
            public int TotalRequired { get; set; }
            public int TotalAssistanceRequired { get; set; }
            public int CompletedCount { get; set; }
            public int AssistanceCount { get; set; }
            public int SimulationCount { get; set; }
            public double CompletionPercentage { get; set; }
        }
    }
}
