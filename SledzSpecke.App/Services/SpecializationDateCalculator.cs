using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services
{
    public class SpecializationDateCalculator : ISpecializationDateCalculator
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<SpecializationDateCalculator> _logger;

        public SpecializationDateCalculator(DatabaseService databaseService, ILogger<SpecializationDateCalculator> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// Oblicza oczekiwaną datę zakończenia specjalizacji z uwzględnieniem wszystkich nieobecności
        /// </summary>
        public async Task<DateTime> CalculateExpectedEndDateAsync(int specializationId)
        {
            try
            {
                var specialization = await _databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                // Pobierz wszystkie nieobecności, które wydłużają specjalizację
                var absences = await _databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? AND AffectsSpecializationLength = 1",
                    specializationId);

                // Oblicz bazową datę zakończenia
                DateTime baseEndDate = specialization.StartDate.AddDays(specialization.BaseDurationWeeks * 7);

                // Oblicz sumę dni nieobecności, które przedłużają specjalizację
                int totalAbsenceDays = absences.Sum(a => a.DurationDays);

                // Oblicz rzeczywistą datę zakończenia, uwzględniając dni nieobecności
                DateTime actualEndDate = baseEndDate.AddDays(totalAbsenceDays);

                return actualEndDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas obliczania oczekiwanej daty zakończenia specjalizacji");
                throw;
            }
        }

        /// <summary>
        /// Oblicza dostępne dni samokształcenia w bieżącym roku
        /// </summary>
        public async Task<int> GetRemainingEducationDaysForYearAsync(int specializationId, int year)
        {
            try
            {
                var specialization = await _databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                // Pobierz wszystkie nieobecności związane z samokształceniem w danym roku
                var educationLeaves = await _databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? AND Type = ? AND Year = ?",
                    specializationId, AbsenceType.SelfEducationLeave, year);

                // Oblicz sumę wykorzystanych dni samokształcenia
                int usedDays = educationLeaves.Sum(a => a.DurationDays);

                // Oblicz pozostałe dni (max 6 dni rocznie)
                int remainingDays = specialization.SelfEducationDaysPerYear - usedDays;

                return Math.Max(0, remainingDays);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas obliczania dostępnych dni samokształcenia");
                throw;
            }
        }

        /// <summary>
        /// Generuje listę ważnych dat i terminów dla specjalizacji
        /// </summary>
        public async Task<List<SpecializationDateInfo>> GetImportantDatesAsync(int specializationId)
        {
            try
            {
                var specialization = await _databaseService.GetByIdAsync<Specialization>(specializationId);
                if (specialization == null)
                {
                    throw new ArgumentException("Nie znaleziono specjalizacji o podanym ID");
                }

                var dates = new List<SpecializationDateInfo>();
                DateTime now = DateTime.Now.Date;

                // Dodaj datę rozpoczęcia specjalizacji
                dates.Add(new SpecializationDateInfo
                {
                    Date = specialization.StartDate,
                    Title = "Rozpoczęcie specjalizacji",
                    Description = "Data rozpoczęcia szkolenia specjalizacyjnego",
                    Type = DateType.Start,
                    IsPast = specialization.StartDate < now
                });

                // Dodaj datę zakończenia modułu podstawowego
                DateTime basicModuleEndDate = specialization.StartDate.AddDays(specialization.BasicModuleDurationWeeks * 7);
                dates.Add(new SpecializationDateInfo
                {
                    Date = basicModuleEndDate,
                    Title = "Zakończenie modułu podstawowego",
                    Description = "Planowana data zakończenia modułu podstawowego",
                    Type = DateType.ModuleEnd,
                    IsPast = basicModuleEndDate < now,
                    DaysRemaining = (int)Math.Max(0, (basicModuleEndDate - now).TotalDays)
                });

                // Oblicz i dodaj oczekiwaną datę zakończenia specjalizacji
                DateTime expectedEndDate = await CalculateExpectedEndDateAsync(specializationId);
                dates.Add(new SpecializationDateInfo
                {
                    Date = expectedEndDate,
                    Title = "Zakończenie specjalizacji",
                    Description = "Oczekiwana data zakończenia specjalizacji z uwzględnieniem nieobecności",
                    Type = DateType.End,
                    IsPast = expectedEndDate < now,
                    DaysRemaining = (int)Math.Max(0, (expectedEndDate - now).TotalDays)
                });

                // Dodaj terminy kursów
                var courses = await _databaseService.QueryAsync<Course>(
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
                            RelatedItemId = course.Id
                        });
                    }
                }

                // Dodaj terminy staży
                var internships = await _databaseService.QueryAsync<Internship>(
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
                            RelatedItemId = internship.Id
                        });
                    }
                }

                // Dodaj ostrzeżenie o kończących się dniach samokształcenia
                int currentYear = DateTime.Now.Year;
                int remainingEducationDays = await GetRemainingEducationDaysForYearAsync(specializationId, currentYear);
                if (remainingEducationDays > 0)
                {
                    // Data końca roku lub 2 tygodnie przed nią, jeśli jest mniej niż 2 tygodnie do końca roku
                    DateTime yearEndWarningDate = new DateTime(currentYear, 12, 31).AddDays(-14);
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
                        DaysRemaining = (int)(new DateTime(currentYear, 12, 31) - now).TotalDays
                    });
                }

                return dates.OrderBy(d => d.Date).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas generowania ważnych dat specjalizacji");
                throw;
            }
        }
    }

    public class SpecializationDateInfo
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateType Type { get; set; }
        public bool IsPast { get; set; }
        public int DaysRemaining { get; set; }
        public int? RelatedItemId { get; set; }
    }

    public enum DateType
    {
        Start,
        End,
        ModuleEnd,
        Course,
        Internship,
        Warning,
        Deadline,
        Other
    }
}