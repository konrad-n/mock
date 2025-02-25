using System;
using System.Collections.Generic;
using System.Linq;

namespace SledzSpecke.Core.Data.MedicalTerminology
{
    public class SpecializationTerminology
    {
        private static Dictionary<string, Dictionary<string, string>> _terminologyBySpecialty;

        static SpecializationTerminology()
        {
            InitializeTerminology();
        }

        private static void InitializeTerminology()
        {
            _terminologyBySpecialty = new Dictionary<string, Dictionary<string, string>>
            {
                // Psychiatria
                {
                    "Psychiatria", new Dictionary<string, string>
                    {
                        { "Badanie psychiatryczne", "Kompleksowa ocena stanu psychicznego pacjenta" },
                        { "Pierwsze badanie", "Pierwsze badanie psychiatryczne z wywiadem i opisem" },
                        { "Badanie kontrolne", "Badanie psychiatryczne kontrolne" },
                        { "Interwencja kryzysowa", "Interwencja psychiatryczna w sytuacji kryzysowej" },
                        { "Wywiad rodzinny", "Wywiad z rodziną pacjenta" },
                        { "Konsultacja", "Konsultacja psychiatryczna dla innego oddziału" },
                        { "Orzeczenie", "Orzeczenie o stanie zdrowia psychicznego" },
                        { "Terapia", "Sesja psychoterapeutyczna" },
                        { "EW", "Zabieg elektrowstrząsowy" },
                        { "MINIŻ", "Skala Mini zaburzeń psychicznych" },
                        { "MADRS", "Skala Montgomery-Asberg do oceny depresji" },
                        { "HAM-D", "Skala Hamiltona do oceny depresji" },
                        { "HAM-A", "Skala Hamiltona do oceny lęku" },
                        { "YMRS", "Skala Younga do oceny manii" },
                        { "PANSS", "Skala objawów pozytywnych i negatywnych w schizofrenii" }
                    }
                },
                
                // Neurologia
                {
                    "Neurologia", new Dictionary<string, string>
                    {
                        { "Badanie neurologiczne", "Kompleksowe badanie neurologiczne" },
                        { "Nakłucie lędźwiowe", "Procedura nakłucia lędźwiowego z pobraniem płynu mózgowo-rdzeniowego" },
                        { "EEG", "Badanie elektroencefalograficzne" },
                        { "EMG", "Badanie elektromiograficzne" },
                        { "ENG", "Elektroneurografia" },
                        { "TK głowy", "Tomografia komputerowa głowy" },
                        { "MRI głowy", "Rezonans magnetyczny głowy" },
                        { "USG Doppler", "Ultrasonografia dopplerowska naczyń" },
                        { "NIHSS", "Skala udaru NIH" }
                    }
                },
                
                // Chirurgia ogólna
                {
                    "Chirurgia ogólna", new Dictionary<string, string>
                    {
                        { "Appendektomia", "Usunięcie wyrostka robaczkowego" },
                        { "Cholecystektomia", "Usunięcie pęcherzyka żółciowego" },
                        { "Herniotomia", "Operacja przepukliny" },
                        { "Strumektomia", "Usunięcie tarczycy" },
                        { "Mastektomia", "Usunięcie piersi" },
                        { "Laparotomia", "Otwarcie jamy brzusznej" },
                        { "Laparoskopia", "Endoskopowe badanie jamy brzusznej" }
                    }
                }
                
                // Można dodać więcej specjalizacji
            };
        }

        public static Dictionary<string, string> GetTerminologyForSpecialty(string specialtyName)
        {
            if (_terminologyBySpecialty.TryGetValue(specialtyName, out var terminology))
            {
                return terminology;
            }

            return new Dictionary<string, string>();
        }

        public static List<string> GetAllSpecialties()
        {
            return _terminologyBySpecialty.Keys.ToList();
        }

        public static string GetTermDefinition(string specialtyName, string term)
        {
            if (_terminologyBySpecialty.TryGetValue(specialtyName, out var terminology))
            {
                if (terminology.TryGetValue(term, out var definition))
                {
                    return definition;
                }
            }

            return null;
        }

        public static List<string> SearchTerms(string specialtyName, string query)
        {
            if (_terminologyBySpecialty.TryGetValue(specialtyName, out var terminology))
            {
                return terminology.Keys
                    .Where(term => term.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return new List<string>();
        }
    }
}
