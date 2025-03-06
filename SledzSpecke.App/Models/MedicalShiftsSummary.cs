using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    /// <summary>
    /// Model podsumowania dyżurów medycznych
    /// </summary>
    public class MedicalShiftsSummary
    {
        /// <summary>
        /// Łączna liczba godzin zrealizowanych dyżurów
        /// </summary>
        public int TotalHours { get; set; }

        /// <summary>
        /// Łączna liczba minut zrealizowanych dyżurów
        /// </summary>
        public int TotalMinutes { get; set; }

        /// <summary>
        /// Łączna liczba godzin zatwierdzonych dyżurów
        /// </summary>
        public int ApprovedHours { get; set; }

        /// <summary>
        /// Łączna liczba minut zatwierdzonych dyżurów
        /// </summary>
        public int ApprovedMinutes { get; set; }

        /// <summary>
        /// Przeprowadza normalizację minut (jeśli > 59, dodaje do godzin)
        /// </summary>
        public void NormalizeTime()
        {
            // Normalizacja dla zrealizowanych dyżurów
            if (this.TotalMinutes >= 60)
            {
                this.TotalHours += this.TotalMinutes / 60;
                this.TotalMinutes %= 60;
            }

            // Normalizacja dla zatwierdzonych dyżurów
            if (this.ApprovedMinutes >= 60)
            {
                this.ApprovedHours += this.ApprovedMinutes / 60;
                this.ApprovedMinutes %= 60;
            }
        }

        /// <summary>
        /// Oblicza podsumowanie na podstawie listy dyżurów
        /// </summary>
        /// <param name="shifts">Lista dyżurów</param>
        public static MedicalShiftsSummary CalculateFromShifts(List<MedicalShift> shifts)
        {
            var summary = new MedicalShiftsSummary();

            foreach (var shift in shifts)
            {
                // Dodaj do sumy zrealizowanych dyżurów
                summary.TotalHours += shift.Hours;
                summary.TotalMinutes += shift.Minutes;

                // Dodaj do sumy zatwierdzonych dyżurów jeśli zatwierdzony
                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    summary.ApprovedHours += shift.Hours;
                    summary.ApprovedMinutes += shift.Minutes;
                }
            }

            // Normalizacja czasu
            summary.NormalizeTime();

            return summary;
        }
    }
}