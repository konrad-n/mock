using System.Collections.Generic;
using SledzSpecke.Core.Models;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public static class SpecializationTypeSeeder
    {
        public static List<SpecializationType> SeedSpecializationTypes()
        {
            return new List<SpecializationType>
            {
                new SpecializationType { Id = 1, Name = "Alergologia", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 2, Name = "Anestezjologia i intensywna terapia", BaseDurationWeeks = 312, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 208, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 3, Name = "Angiologia", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 4, Name = "Audiologia i foniatria", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 5, Name = "Balneologia i medycyna fizykalna", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 6, Name = "Chirurgia dziecieca", BaseDurationWeeks = 312, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 208, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },

                // Add more specialization types here - all 77 types
                new SpecializationType { Id = 7, Name = "Chirurgia klatki piersiowej", BaseDurationWeeks = 312, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 208, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
                new SpecializationType { Id = 8, Name = "Chirurgia naczyniowa", BaseDurationWeeks = 312, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 208, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },

                new SpecializationType { Id = 28, Name = "Hematologia", BaseDurationWeeks = 261, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 157, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },

                new SpecializationType { Id = 77, Name = "Zdrowie publiczne", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104, VacationDaysPerYear = 26, SelfEducationDaysPerYear = 6, StatutoryHolidaysPerYear = 13, RequiredDutyHoursPerWeek = 10.0833 },
            };
        }
    }
}
