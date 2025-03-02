using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class SpecializationDateCalculator : ISpecializationDateCalculator
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<SpecializationDateCalculator> logger;

        public SpecializationDateCalculator(
            IDatabaseService databaseService,
            ILogger<SpecializationDateCalculator> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task<DateTime> CalculateExpectedEndDateAsync(int specializationId)
        {
            try
            {
                var specialization = await this.databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                var absences = await this.databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? AND AffectsSpecializationLength = 1",
                    specializationId);

                DateTime baseEndDate = specialization.StartDate.AddDays(specialization.BaseDurationWeeks * 7);
                int totalAbsenceDays = absences.Sum(a => a.DurationDays);
                DateTime actualEndDate = baseEndDate.AddDays(totalAbsenceDays);
                return actualEndDate;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Błąd podczas obliczania oczekiwanej daty zakończenia specjalizacji");
                throw;
            }
        }

        public async Task<int> GetRemainingEducationDaysForYearAsync(int specializationId, int year)
        {
            try
            {
                var specialization = await this.databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                var educationLeaves = await this.databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? AND Type = ? AND Year = ?",
                    specializationId,
                    AbsenceType.SelfEducationLeave,
                    year);

                int usedDays = educationLeaves.Sum(a => a.DurationDays);
                int remainingDays = specialization.SelfEducationDaysPerYear - usedDays;

                return Math.Max(0, remainingDays);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Błąd podczas obliczania dostępnych dni samokształcenia");
                throw;
            }
        }

        public async Task<List<SpecializationDateInfo>> GetImportantDatesAsync(int specializationId)
        {
            try
            {
                var specialization = await this.databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                var dates = new List<SpecializationDateInfo>();
                DateTime now = DateTime.Now.Date;

                dates.Add(new SpecializationDateInfo
                {
                    Date = specialization.StartDate,
                    Title = "Rozpoczęcie specjalizacji",
                    Description = "Data rozpoczęcia szkolenia specjalizacyjnego",
                    Type = DateType.Start,
                    IsPast = specialization.StartDate < now,
                });

                DateTime basicModuleEndDate = specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7);
                dates.Add(new SpecializationDateInfo
                {
                    Date = basicModuleEndDate,
                    Title = "Zakończenie modułu podstawowego",
                    Description = "Planowana data zakończenia modułu podstawowego",
                    Type = DateType.ModuleEnd,
                    IsPast = basicModuleEndDate < now,
                    DaysRemaining = (int)Math.Max(0, (basicModuleEndDate - now).TotalDays),
                });

                DateTime expectedEndDate = await this.CalculateExpectedEndDateAsync(specializationId);
                dates.Add(new SpecializationDateInfo
                {
                    Date = expectedEndDate,
                    Title = "Zakończenie specjalizacji",
                    Description = "Oczekiwana data zakończenia specjalizacji z uwzględnieniem nieobecności",
                    Type = DateType.End,
                    IsPast = expectedEndDate < now,
                    DaysRemaining = (int)Math.Max(0, (expectedEndDate - now).TotalDays),
                });

                var courses = await this.databaseService.QueryAsync<Course>(
                    "SELECT * FROM Courses WHERE SpecializationId = ? AND ScheduledDate IS NOT NULL AND IsCompleted = 0",
                    specializationId);

                foreach (var course in courses)
                {
                    if (course.ScheduledDate.HasValue)
                    {
                        dates.Add(new SpecializationDateInfo
                        {
                            Date = course.ScheduledDate.Value,
                            Title = $"Kurs: {course.Name}",
                            Description = $"Zaplanowany kurs specjalizacyjny",
                            Type = DateType.Course,
                            IsPast = course.ScheduledDate.Value < now,
                            DaysRemaining = (int)Math.Max(0, (course.ScheduledDate.Value - now).TotalDays),
                            RelatedItemId = course.Id,
                        });
                    }
                }

                var internships = await this.databaseService.QueryAsync<Internship>(
                    "SELECT * FROM Internships WHERE SpecializationId = ? AND StartDate IS NOT NULL AND IsCompleted = 0",
                    specializationId);

                foreach (var internship in internships)
                {
                    if (internship.StartDate.HasValue)
                    {
                        dates.Add(new SpecializationDateInfo
                        {
                            Date = internship.StartDate.Value,
                            Title = $"Staż: {internship.Name}",
                            Description = $"Rozpoczęcie stażu kierunkowego",
                            Type = DateType.Internship,
                            IsPast = internship.StartDate.Value < now,
                            DaysRemaining = (int)Math.Max(0, (internship.StartDate.Value - now).TotalDays),
                            RelatedItemId = internship.Id,
                        });
                    }
                }

                int currentYear = DateTime.Now.Year;
                int remainingEducationDays = await this.GetRemainingEducationDaysForYearAsync(specializationId, currentYear);
                if (remainingEducationDays > 0)
                {
                    DateTime yearEndWarningDate = new DateTime(currentYear, 12, 31, 0, 0, 0, DateTimeKind.Local).AddDays(-14);
                    if (now > yearEndWarningDate)
                    {
                        yearEndWarningDate = now;
                    }

                    dates.Add(new SpecializationDateInfo
                    {
                        Date = yearEndWarningDate,
                        Title = "Dni samokształcenia",
                        Description = $"Pozostało {remainingEducationDays} dni samokształcenia w {currentYear} roku",
                        Type = DateType.Warning,
                        IsPast = false,
                        DaysRemaining = (int)(new DateTime(currentYear, 12, 31, 0, 0, 0, DateTimeKind.Local) - now).TotalDays,
                    });
                }

                return dates.OrderBy(d => d.Date).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Błąd podczas generowania ważnych dat specjalizacji");
                throw;
            }
        }
    }
}