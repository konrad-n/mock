using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public class DateCalculator
    {
        // Oblicza datę zakończenia specjalizacji z uwzględnieniem wszystkich nieobecności
        public static DateTime CalculateSpecializationEndDate(
            DateTime startDate,
            int durationDays,
            List<Absence> absences)
        {
            // Planowana data końcowa bez przedłużeń
            DateTime plannedEndDate = startDate.AddDays(durationDays - 1);

            // Obliczenie przedłużenia ze względu na nieobecności
            int extensionDays = 0;

            foreach (var absence in absences)
            {
                if (absence.Type == AbsenceType.Sick ||
                    absence.Type == AbsenceType.Maternity ||
                    absence.Type == AbsenceType.Paternity)
                {
                    // Całkowita liczba dni (włącznie z weekendami)
                    int totalDays = (absence.EndDate - absence.StartDate).Days + 1;
                    extensionDays += totalDays;
                }

                // Urlopy wypoczynkowe i dni ustawowo wolne nie wpływają na przedłużenie
            }

            // Uwzględnienie skrócenia ze względu na uznania
            int reductionDays = absences
                .Where(a => a.Type == AbsenceType.Recognition)
                .Sum(a => (a.EndDate - a.StartDate).Days + 1);

            // Finalna data końcowa z uwzględnieniem przedłużeń i skróceń
            return plannedEndDate.AddDays(extensionDays - reductionDays);
        }

        // Oblicza liczbę dni roboczych między dwiema datami
        public static int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            int days = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    days++;
                }

                currentDate = currentDate.AddDays(1);
            }

            return days;
        }
    }
}
