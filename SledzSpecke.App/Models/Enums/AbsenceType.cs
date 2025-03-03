namespace SledzSpecke.App.Models.Enums
{
    public enum AbsenceType
    {
        Sick,        // Zwolnienie lekarskie (L4)
        Maternity,   // Urlop macierzyński
        Paternity,   // Urlop ojcowski/rodzicielski
        Vacation,    // Urlop wypoczynkowy (nie wpływa na przedłużenie)
        Unpaid,      // Urlop bezpłatny
        Training,    // Urlop szkoleniowy
        Recognition, // Uznanie (skrócenie specjalizacji)
        Other,       // Inne nieobecności
    }
}
