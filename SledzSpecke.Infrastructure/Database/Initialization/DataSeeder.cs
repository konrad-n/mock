using SledzSpecke.Core.Models.Domain;
using System;
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
                    DurationInWeeks = 312, // Na podstawie dokumentacji
                    ProgramVersion = "2023",
                    ApprovalDate = new DateTime(2023, 1, 1),
                    MinimumDutyHours = 1200, // Przykładowa wartość, należy zweryfikować
                    Requirements = "Program specjalizacji w dziedzinie psychiatrii dla lekarzy nieposiadających specjalizacji I lub II stopnia",
                    Description = "Celem szkolenia specjalizacyjnego jest uzyskanie szczególnych kwalifikacji w dziedzinie psychiatrii umożliwiających zgodnie ze współczesną wiedzą medyczną samodzielne rozwiązywanie problemów klinicznych"
                }
            };
        }

        public static List<ProcedureRequirement> GetBasicProcedureRequirements()
        {
            return new List<ProcedureRequirement>
            {
                new ProcedureRequirement
                {
                    Name = "Badanie psychiatryczne",
                    Description = "Przeprowadzenie badania psychiatrycznego i sporządzenie opisu stanu psychicznego osoby badanej",
                    RequiredCount = 20,
                    AssistanceCount = 0,
                    Category = "Podstawowe procedury",
                    SupervisionRequired = true,
                    SpecializationId = 1 // ID psychiatrii
                },
                new ProcedureRequirement
                {
                    Name = "Ocena stanu psychicznego z użyciem skal klinicznych",
                    Description = "Ocena stanu psychicznego za pomocą standaryzowanych skal klinicznych",
                    RequiredCount = 20,
                    AssistanceCount = 0,
                    Category = "Diagnostyka",
                    SupervisionRequired = true,
                    SpecializationId = 1
                },
                new ProcedureRequirement
                {
                    Name = "Zabiegi elektrowstrząsowe",
                    Description = "Kwalifikacja i przygotowanie pacjentów oraz przeprowadzenie zabiegów elektrowstrząsowych",
                    RequiredCount = 5,
                    AssistanceCount = 0,
                    Category = "Zabiegi",
                    SupervisionRequired = true,
                    SpecializationId = 1
                },
                new ProcedureRequirement
                {
                    Name = "Opinie psychiatryczne",
                    Description = "Przygotowanie opinii w sprawie zasadności przyjęcia bez zgody do szpitala psychiatrycznego",
                    RequiredCount = 20,
                    AssistanceCount = 0,
                    Category = "Opiniowanie",
                    SupervisionRequired = true,
                    SpecializationId = 1
                }
            };
        }

        public static List<CourseDefinition> GetBasicCourses()
        {
            return new List<CourseDefinition>
            {
                new CourseDefinition
                {
                    Name = "Wprowadzenie do specjalizacji w dziedzinie psychiatrii",
                    Description = "Kurs wprowadzający przedstawiający podstawową problematykę dotyczącą specyfiki psychiatrii",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "Kurs obowiązkowy w pierwszym roku odbywania szkolenia specjalizacyjnego",
                    CourseTopics = new List<string>
                    {
                        "Wprowadzenie w problematykę zdrowia psychicznego",
                        "Zagadnienia prawne dotyczące zdrowia psychicznego",
                        "Wprowadzenie do psychopatologii",
                        "Klasyfikacje zaburzeń psychicznych",
                        "Podstawy diagnostyki psychiatrycznej"
                    }
                },
                new CourseDefinition
                {
                    Name = "Psychiatria sądowa i opiniowanie sądowo-psychiatryczne",
                    Description = "Problematyka unormowań prawnych dotyczących pogranicza prawa i psychiatrii",
                    DurationInHours = 80,
                    DurationInDays = 10,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    Requirements = "Zalecany po drugim roku szkolenia specjalizacyjnego",
                    CourseTopics = new List<string>
                    {
                        "Wprowadzenie do psychiatrii sądowej",
                        "Poczytalność i niepoczytalność",
                        "Opiniowanie w sprawach karnych",
                        "Opiniowanie w sprawach cywilnych",
                        "Detencja psychiatryczna"
                    }
                },
                new CourseDefinition
                {
                    Name = "Psychiatria środowiskowa i rehabilitacja psychiatryczna",
                    Description = "Przedstawienie roli i specyfiki opieki środowiskowej i rehabilitacji w psychiatrii",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Modele opieki środowiskowej",
                        "Zespoły leczenia środowiskowego",
                        "Rehabilitacja psychiatryczna",
                        "Rola rodziny w opiece środowiskowej",
                        "Stigma i destygmatyzacja"
                    }
                },
                new CourseDefinition
                {
                    Name = "Wybrane zagadnienia z zakresu psychiatrii klinicznej",
                    Description = "Przedstawienie wybranych najistotniejszych aspektów klinicznych w psychiatrii",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 3,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Zaburzenia afektywne - aktualne kierunki leczenia",
                        "Schizofrenia i inne zaburzenia psychotyczne",
                        "Zaburzenia lękowe i stresowe",
                        "Zaburzenia odżywiania",
                        "Zaburzenia psychiczne u osób starszych"
                    }
                }
            };
        }

        public static List<InternshipDefinition> GetBasicInternships()
        {
            return new List<InternshipDefinition>
            {
                new InternshipDefinition
                {
                    Name = "Staż podstawowy w zakresie psychiatrii",
                    Description = "Staż w oddziale ogólnopsychiatrycznym",
                    DurationInWeeks = 148,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "W tym 36 tygodni (180 dni roboczych) w pierwszym roku szkolenia tylko oddział ogólnopsychiatryczny",
                    CompletionRequirements = new List<string>
                    {
                        "Wykonanie 20 badań psychiatrycznych",
                        "Przeprowadzenie 10 interwencji kryzysowych",
                        "Poprowadzenie 15 przypadków pacjentów z zaburzeniami psychotycznymi"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie chorób wewnętrznych",
                    Description = "Nabycie niezbędnych umiejętności dotyczących diagnozy i badania pacjentów z chorobami z zakresu chorób wewnętrznych",
                    DurationInWeeks = 6,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "Zalecany I rok szkolenia",
                    CompletionRequirements = new List<string>
                    {
                        "Wykonanie 10 badań podmiotowych i przedmiotowych",
                        "Interpretacja 15 badań laboratoryjnych",
                        "Interpretacja 5 badań EKG"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie neurologii",
                    Description = "Nabycie niezbędnych umiejętności dotyczących diagnozy i badania pacjentów z chorobami z zakresu neurologii",
                    DurationInWeeks = 4,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "Zalecany I-II rok szkolenia",
                    CompletionRequirements = new List<string>
                    {
                        "Wykonanie 10 badań neurologicznych",
                        "Interpretacja 5 badań MRI/CT głowy",
                        "Interpretacja 5 badań EEG"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie psychiatrii dzieci i młodzieży",
                    Description = "Nabycie niezbędnych umiejętności dotyczących diagnozy i badania pacjentów z zaburzeniami psychicznymi dotyczącymi dzieci i młodzieży",
                    DurationInWeeks = 8,
                    IsRequired = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    Requirements = "II lub III poziom referencyjności",
                    CompletionRequirements = new List<string>
                    {
                        "Wykonanie 10 badań psychiatrycznych dzieci",
                        "Wykonanie 10 badań psychiatrycznych młodzieży",
                        "Opracowanie 5 planów terapeutycznych"
                    }
                }
            };
        }
        
        public static List<InternshipModule> GetInternshipModules()
        {
            return new List<InternshipModule>
            {
                new InternshipModule
                {
                    InternshipDefinitionId = 1, // Staż podstawowy w zakresie psychiatrii
                    Name = "Moduł psychiatrii ogólnej",
                    DurationInWeeks = 36,
                    Location = "Oddział ogólnopsychiatryczny",
                    Requirements = "Oddział musi zapewniać dostęp do wszystkich metod terapeutycznych",
                    RequiredSkills = new List<string>
                    {
                        "Diagnostyka psychiatryczna",
                        "Leczenie farmakologiczne",
                        "Interwencje kryzysowe"
                    },
                    RequiredProcedures = new Dictionary<string, int>
                    {
                        {"Badanie psychiatryczne", 10},
                        {"Monitorowanie leczenia farmakologicznego", 15},
                        {"Interwencja kryzysowa", 5}
                    }
                },
                new InternshipModule
                {
                    InternshipDefinitionId = 1, // Staż podstawowy w zakresie psychiatrii
                    Name = "Moduł psychiatrii środowiskowej",
                    DurationInWeeks = 12,
                    Location = "Zespół leczenia środowiskowego",
                    Requirements = "Jednostka musi posiadać zespół leczenia środowiskowego",
                    RequiredSkills = new List<string>
                    {
                        "Praca w środowisku pacjenta",
                        "Rehabilitacja psychiatryczna",
                        "Współpraca z rodziną pacjenta"
                    },
                    RequiredProcedures = new Dictionary<string, int>
                    {
                        {"Wizyta domowa", 10},
                        {"Opracowanie planu rehabilitacji", 5},
                        {"Psychoedukacja rodziny", 5}
                    }
                }
            };
        }
        
        public static List<DutyRequirement> GetDutyRequirements()
        {
            return new List<DutyRequirement>
            {
                new DutyRequirement
                {
                    SpecializationId = 1,
                    Type = "Dyżur ogólnopsychiatryczny",
                    RequiredHours = 600,
                    Description = "Dyżury w oddziale ogólnopsychiatrycznym",
                    RequiredYear = 1,
                    RequiresSupervision = true,
                    MinimumHoursPerMonth = 40,
                    MinimumDutiesPerMonth = 4,
                    RequiredCompetencies = new List<string>
                    {
                        "Ocena stanu psychicznego w trybie dyżurowym",
                        "Podjęcie decyzji o hospitalizacji",
                        "Interwencja w stanach nagłych"
                    }
                },
                new DutyRequirement
                {
                    SpecializationId = 1,
                    Type = "Dyżur na izbie przyjęć",
                    RequiredHours = 400,
                    Description = "Dyżury na izbie przyjęć psychiatrycznej",
                    RequiredYear = 2,
                    RequiresSupervision = true,
                    MinimumHoursPerMonth = 30,
                    MinimumDutiesPerMonth = 3,
                    RequiredCompetencies = new List<string>
                    {
                        "Kwalifikacja do hospitalizacji psychiatrycznej",
                        "Opiniowanie o przyjęciu bez zgody",
                        "Interwencja kryzysowa"
                    }
                },
                new DutyRequirement
                {
                    SpecializationId = 1,
                    Type = "Dyżur w psychiatrii konsultacyjnej",
                    RequiredHours = 200,
                    Description = "Dyżury w ramach zespołu konsultacyjnego w szpitalu ogólnym",
                    RequiredYear = 3,
                    RequiresSupervision = false,
                    MinimumHoursPerMonth = 20,
                    MinimumDutiesPerMonth = 2,
                    RequiredCompetencies = new List<string>
                    {
                        "Konsultacje psychiatryczne na oddziałach niepsychiatrycznych",
                        "Diagnostyka różnicowa somatyczno-psychiatryczna",
                        "Psychiatria konsultacyjna"
                    }
                }
            };
        }
    }
}
