namespace SledzSpecke.App.Models
{
    public class ProcedureDefinition
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int RequiredCountA { get; set; } // Wymagana liczba procedur jako operator

        public int RequiredCountB { get; set; } // Wymagana liczba procedur jako asysta

        public string Group { get; set; } // Np. staż podstawowy, staż kierunkowy

        public int? InternshipTypeId { get; set; } // Powiązanie z typem stażu
    }
}
