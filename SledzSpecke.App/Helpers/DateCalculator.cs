using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public class DateCalculator
    {
        public static DateTime CalculateSpecializationEndDate(
            DateTime startDate,
            int durationDays,
            List<Absence> absences)
        {
            DateTime plannedEndDate = startDate.AddDays(durationDays - 1);
            int extensionDays = 0;

            foreach (var absence in absences)
            {
                if (absence.Type == AbsenceType.Sick ||
                    absence.Type == AbsenceType.Maternity ||
                    absence.Type == AbsenceType.Paternity)
                {
                    int totalDays = (absence.EndDate - absence.StartDate).Days + 1;
                    extensionDays += totalDays;
                }
            }

            int reductionDays = absences
                .Where(a => a.Type == AbsenceType.Recognition)
                .Sum(a => (a.EndDate - a.StartDate).Days + 1);

            return plannedEndDate.AddDays(extensionDays - reductionDays);
        }

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
