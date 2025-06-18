namespace SledzSpecke.Application.Features.MedicalShifts.DTOs;

public class MedicalShiftStatisticsDto
{
    public int TotalShifts { get; set; }
    public int TotalHours { get; set; }
    public int TotalMinutes { get; set; }
    public double AverageShiftDuration { get; set; }
    public Dictionary<string, int> ShiftsByLocation { get; set; } = new();
    public Dictionary<string, int> ShiftsByMonth { get; set; } = new();
}