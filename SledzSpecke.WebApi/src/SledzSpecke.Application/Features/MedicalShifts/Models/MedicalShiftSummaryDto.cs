namespace SledzSpecke.Application.Features.MedicalShifts.Models;

public class MedicalShiftSummaryDto
{
    public int TotalHours { get; set; }
    public int TotalMinutes { get; set; }
    public int ApprovedHours { get; set; }
    public int ApprovedMinutes { get; set; }

    public void NormalizeTime()
    {
        if (TotalMinutes >= 60)
        {
            TotalHours += TotalMinutes / 60;
            TotalMinutes %= 60;
        }

        if (ApprovedMinutes >= 60)
        {
            ApprovedHours += ApprovedMinutes / 60;
            ApprovedMinutes %= 60;
        }
    }
}