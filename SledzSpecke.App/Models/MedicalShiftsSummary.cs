using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class MedicalShiftsSummary
    {
        public int TotalHours { get; set; }

        public int TotalMinutes { get; set; }

        public int ApprovedHours { get; set; }

        public int ApprovedMinutes { get; set; }

        public void NormalizeTime()
        {
            if (this.TotalMinutes >= 60)
            {
                this.TotalHours += this.TotalMinutes / 60;
                this.TotalMinutes %= 60;
            }

            if (this.ApprovedMinutes >= 60)
            {
                this.ApprovedHours += this.ApprovedMinutes / 60;
                this.ApprovedMinutes %= 60;
            }
        }

        public static MedicalShiftsSummary CalculateFromShifts(List<MedicalShift> shifts)
        {
            var summary = new MedicalShiftsSummary();

            foreach (var shift in shifts)
            {
                summary.TotalHours += shift.Hours;
                summary.TotalMinutes += shift.Minutes;

                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    summary.ApprovedHours += shift.Hours;
                    summary.ApprovedMinutes += shift.Minutes;
                }
            }

            summary.NormalizeTime();

            return summary;
        }
    }
}