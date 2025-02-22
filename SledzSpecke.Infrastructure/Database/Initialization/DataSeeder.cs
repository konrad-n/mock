using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public static class DataSeeder
    {
        public static List<Specialization> GetBasicSpecializations()
        {
            return new List<Specialization>
        {
            new Specialization
            {
                Name = "Psychiatria",
                DurationInWeeks = 312,
                ProgramVersion = "2023",
                // inne dane z pliku dbconcept3
            }
        };
        }

        public static List<ProcedureDefinition> GetBasicProcedures()
        {
            return new List<ProcedureDefinition>
            {
                // Dodaj przykładowe procedury
            };
        }
    }
}
