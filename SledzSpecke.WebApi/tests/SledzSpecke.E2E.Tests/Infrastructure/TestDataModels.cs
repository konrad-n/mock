namespace SledzSpecke.E2E.Tests.Infrastructure;

// DTOs for test data - not domain entities
public class MedicalShiftTestData
{
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Location { get; set; } = "";
    public int Year { get; set; }
    public string ShiftType { get; set; } = "";
}

public class ProcedureTestData
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string IcdCode { get; set; } = "";
    public string ExecutionType { get; set; } = "";
    public string Supervisor { get; set; } = "";
}

public class TestUser
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string SmkVersion { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int Year { get; set; }
}

// Add ShiftData class that was missing
public class ShiftData
{
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Location { get; set; } = "";
    public int Year { get; set; }
    public string ShiftType { get; set; } = "";
    public string Type { get; set; } = ""; // Add for compatibility
}
