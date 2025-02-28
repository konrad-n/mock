using SQLite;
using System;

[Table("ProcedureEntries")]
public class ProcedureEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string PatientId { get; set; } // Patient initials for SMK

    public string Location { get; set; }

    public string SupervisorName { get; set; }

    public string Notes { get; set; }

    [Indexed]
    public int ProcedureId { get; set; }

    // New fields according to SMK manual requirements
    public string PatientGender { get; set; } // Patient gender
    public string FirstAssistantData { get; set; } // First assistant data
    public string SecondAssistantData { get; set; } // Second assistant data
    public string ProcedureGroup { get; set; } // "Procedura z grupy"
    public string InternshipName { get; set; } // Internship name
}