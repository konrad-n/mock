namespace SledzSpecke.Application.Features.MedicalShifts;

public static class MedicalShiftConstants
{
    // From SMK documentation - standard shift durations
    public static class StandardDurations
    {
        public const int DayShiftHours = 8;
        public const int NightShiftHours = 12;
        public const int ExtendedShiftHours = 24;
        public const int WeeklyAverageHours = 10;
        public const int WeeklyAverageMinutes = 5;
    }

    // Common locations from SMK PDFs
    public static class Locations
    {
        public const string EmergencyDepartment = "Szpitalny Oddział Ratunkowy";
        public const string IntensiveCare = "Oddział Anestezjologii i Intensywnej Terapii";
        public const string InternalMedicine = "Oddział Chorób Wewnętrznych";
        public const string Cardiology = "Oddział Kardiologii";
        public const string Surgery = "Blok Operacyjny";
        public const string Pediatrics = "Oddział Pediatryczny";
        public const string Neurology = "Oddział Neurologii";
        public const string Psychiatry = "Oddział Psychiatryczny";
        public const string Gynecology = "Oddział Ginekologiczno-Położniczy";
        public const string Orthopedics = "Oddział Ortopedyczny";
    }

    // Validation limits
    public static class Limits
    {
        public const int MaxDailyHours = 24;
        public const int MaxWeeklyHours = 60;
        public const int MinShiftMinutes = 60;
        public const int MonthlyMinimumHours = 160; // From SMK requirements
        public const int WeeklyMaximumHours = 48; // From SMK regulations
    }

    // Shift types based on SMK documentation
    public static class ShiftTypes
    {
        public const string Regular = "Dyżur zwykły";
        public const string Night = "Dyżur nocny";
        public const string Weekend = "Dyżur weekendowy";
        public const string Holiday = "Dyżur świąteczny";
        public const string Emergency = "Dyżur w SOR";
        public const string Intensive = "Dyżur w OIT";
    }
}