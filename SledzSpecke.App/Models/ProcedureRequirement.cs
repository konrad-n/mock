namespace SledzSpecke.App.Models
{
    public class ProcedureRequirement
    {
        public string ProcedureCode { get; set; } = string.Empty;

        public string ProcedureName { get; set; } = string.Empty;

        public int RequiredCountA { get; set; } // Wymagana liczba procedur jako operator

        public int RequiredCountB { get; set; } // Wymagana liczba procedur jako asysta

        public string InternshipType { get; set; } = string.Empty; // Typ stażu, podczas którego ma być wykonana

        public bool IsMandatory { get; set; } // Czy procedura jest obowiązkowa
    }
}
