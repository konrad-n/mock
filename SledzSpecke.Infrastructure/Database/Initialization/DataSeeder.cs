using System;
using System.Collections.Generic;
using SledzSpecke.Core.Models;
using SledzSpecke.Models;

namespace SledzSpecke.Services
{
    public class DataSeeder
    {
        public static Specialization SeedHematologySpecialization()
        {
            var specialization = new Specialization
            {
                Id = 1,
                Name = "Hematologia",
                StartDate = DateTime.Now,
                ExpectedEndDate = DateTime.Now.AddYears(5),
                BaseDurationWeeks = 261, // 5 lat (156 tygodni i 3 dni = 783 dni robocze)
                BasicModuleDurationWeeks = 104, // 2 lata
                SpecialisticModuleDurationWeeks = 157, // 3 lata
                VacationDaysPerYear = 26,
                SelfEducationDaysPerYear = 6,
                StatutoryHolidaysPerYear = 13,
                RequiredDutyHoursPerWeek = (int)(10 + (5 / 60.0)),  // 10 godzin 5 minut
                RequiresPublication = true,
                RequiredConferences = 3
            };



            // Dodanie kursów specjalizacyjnych (moduł specjalistyczny)
            specialization.RequiredCourses.AddRange(new List<Course>
        {
            new Course {
                Id = 1,
                Name = "Wprowadzenie do specjalizacji w dziedzinie hematologii i zagadnienia promocji zdrowia w hematologii",
                DurationDays = 3,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 2,
                Name = "Badanie cytologiczne szpiku oraz histologiczne szpiku i węzłów chłonnych",
                DurationDays = 5,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 3,
                Name = "Diagnostyka immunofenotypowa",
                DurationDays = 1,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 4,
                Name = "Diagnostyka cytogenetyczna i molekularna nowotworów krwi",
                DurationDays = 1,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 5,
                Name = "Zaburzenia hemostazy",
                DurationDays = 3,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 6,
                Name = "Immunohematologia",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 7,
                Name = "Przeszczepianie komórek krwiotwórczych",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 8,
                Name = "Psychologiczne problemy pacjentów z chorobami krwi i układu chłonnego",
                DurationDays = 1,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 9,
                Name = "Onkologia guzów litych dla hematologów",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 10,
                Name = "Hematolog jako konsultant",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Course {
                Id = 11,
                Name = "Kurs atestacyjny (podsumowujący): Hematologia",
                DurationDays = 5,
                IsRequired = true,
                Module = ModuleType.Specialistic
            }
        });

            // Dodanie kursów specjalizacyjnych (moduł podstawowy z chorób wewnętrznych)
            specialization.RequiredCourses.AddRange(new List<Course>
        {
            new Course {
                Id = 12,
                Name = "Diagnostyka obrazowa",
                DurationDays = 3,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 13,
                Name = "Alergologia",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 14,
                Name = "Onkologia kliniczna",
                DurationDays = 4,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 15,
                Name = "Medycyna paliatywna",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 16,
                Name = "Toksykologia",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 17,
                Name = "Geriatria",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 18,
                Name = "Diabetologia",
                DurationDays = 4,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 19,
                Name = "Przetaczanie krwi i jej składników",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 20,
                Name = "Orzecznictwo lekarskie",
                DurationDays = 3,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Course {
                Id = 21,
                Name = "Profilaktyka i promocja zdrowia",
                DurationDays = 2,
                IsRequired = true,
                Module = ModuleType.Basic
            }
        });

            // Dodanie staży (moduł specjalistyczny)
            specialization.RequiredInternships.AddRange(new List<Internship>
        {
            new Internship {
                Id = 1,
                Name = "Staż podstawowy w zakresie hematologii",
                DurationWeeks = 101,
                WorkingDays = 505,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 2,
                Name = "Staż kierunkowy w laboratorium hematologicznym",
                DurationWeeks = 4,
                WorkingDays = 20,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 3,
                Name = "Staż kierunkowy w ośrodku przeszczepiania szpiku akredytowanym do wykonywania przeszczepień allogenicznych",
                DurationWeeks = 4,
                WorkingDays = 20,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 4,
                Name = "Staż kierunkowy w hematologicznym oddziale dziennego leczenia",
                DurationWeeks = 5,
                WorkingDays = 25,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 5,
                Name = "Staż kierunkowy w poradni hematologicznej",
                DurationWeeks = 8,
                WorkingDays = 40,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 6,
                Name = "Staż kierunkowy w Regionalnym Centrum Krwiodawstwa i Krwiolecznictwa",
                DurationWeeks = 1,
                WorkingDays = 5,
                IsRequired = true,
                Module = ModuleType.Specialistic
            },
            new Internship {
                Id = 7,
                Name = "Staż kierunkowy w ośrodku leczenia hemofilii i pokrewnych skaz krwotocznych",
                DurationWeeks = 3,
                WorkingDays = 15,
                IsRequired = true,
                Module = ModuleType.Specialistic
            }
        });

            // Dodanie staży (moduł podstawowy)
            specialization.RequiredInternships.AddRange(new List<Internship>
        {
            new Internship {
                Id = 8,
                Name = "Staż podstawowy w zakresie chorób wewnętrznych",
                DurationWeeks = 67,
                WorkingDays = 335,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Internship {
                Id = 9,
                Name = "Staż kierunkowy w zakresie intensywnej opieki medycznej",
                DurationWeeks = 4,
                WorkingDays = 20,
                IsRequired = true,
                Module = ModuleType.Basic
            },
            new Internship {
                Id = 10,
                Name = "Staż kierunkowy w szpitalnym oddziale ratunkowym",
                DurationWeeks = 12,
                WorkingDays = 60,
                IsRequired = true,
                Module = ModuleType.Basic
            }
        });

            // Dodanie procedur medycznych (moduł specjalistyczny) - staż podstawowy
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
        {
            new MedicalProcedure {
                Id = 1,
                Name = "biopsja szpiku z mostka",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 2,
                Name = "biopsja szpiku z mostka",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 3,
                Name = "biopsja szpiku z kolca tylnego talerza biodrowego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 20,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 4,
                Name = "biopsja szpiku z kolca tylnego talerza biodrowego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 5,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 5,
                Name = "biopsja szpiku z kolca przedniego talerza biodrowego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 6,
                Name = "biopsja szpiku z kolca przedniego talerza biodrowego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 7,
                Name = "trepanobiopsja szpiku",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 10,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 8,
                Name = "trepanobiopsja szpiku",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 5,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 9,
                Name = "prowadzenie intensywnego leczenia chorych na ostre białaczki szpikowe lub ostre białaczki limfoblastyczne",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 7,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 10,
                Name = "prowadzenie leczenia indukującego chorych na chłoniaki agresywne",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 4,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 11,
                Name = "afereza lecznicza",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 1,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 12,
                Name = "zakładanie centralnego cewnika żylnego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 13,
                Name = "punkcja lędźwiowa i dokanałowe podanie cytostatyków",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 10,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            },
            new MedicalProcedure {
                Id = 14,
                Name = "biopsja cienkoigłowa węzła lub biopsja węzła pod kontrolą USG/TK",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Specialistic,
                InternshipId = 1
            }
        });

            // Dodanie procedur medycznych (moduł specjalistyczny) - staże kierunkowe
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
        {
            new MedicalProcedure {
                Id = 15,
                Name = "ocena rozmazu krwi obwodowej",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 30,
                Module = ModuleType.Specialistic,
                InternshipId = 2
            },
            new MedicalProcedure {
                Id = 16,
                Name = "ocena rozmazu krwi obwodowej",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 30,
                Module = ModuleType.Specialistic,
                InternshipId = 2
            },
            new MedicalProcedure {
                Id = 17,
                Name = "ocena mielogramu",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 30,
                Module = ModuleType.Specialistic,
                InternshipId = 2
            },
            new MedicalProcedure {
                Id = 18,
                Name = "ocena mielogramu",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 30,
                Module = ModuleType.Specialistic,
                InternshipId = 2
            },
            new MedicalProcedure {
                Id = 19,
                Name = "przeszczepienie autologicznych komórek krwiotwórczych",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Specialistic,
                InternshipId = 3
            },
            new MedicalProcedure {
                Id = 20,
                Name = "przeszczepienie allogenicznych komórek krwiotwórczych",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 1,
                Module = ModuleType.Specialistic,
                InternshipId = 3
            }
        });

            // Dodanie procedur medycznych (moduł podstawowy) - staż podstawowy
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
        {
            new MedicalProcedure {
                Id = 21,
                Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 22,
                Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 23,
                Name = "nakłucie jamy opłucnej w przypadku płynu",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 10,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 24,
                Name = "nakłucie jamy opłucnej w przypadku płynu",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 25,
                Name = "nakłucie jamy otrzewnej w przypadku wodobrzusza",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 10,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 26,
                Name = "nakłucie jamy otrzewnej w przypadku wodobrzusza",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 27,
                Name = "nakłucia żył obwodowych – iniekcje dożylne, pobrania krwi obwodowej",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 30,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 28,
                Name = "nakłucia żył obwodowych – iniekcje dożylne, pobrania krwi obwodowej",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 5,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 29,
                Name = "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 30,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 30,
                Name = "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 5,
                Module = ModuleType.Basic,
                InternshipId = 8
            }
        });

            // Dodanie pozostałych procedur medycznych (moduł podstawowy)
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
        {
            new MedicalProcedure {
                Id = 31,
                Name = "pomiar ośrodkowego ciśnienia żylnego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 6,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 32,
                Name = "pomiar ośrodkowego ciśnienia żylnego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 33,
                Name = "cewnikowanie pęcherza moczowego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 20,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 34,
                Name = "cewnikowanie pęcherza moczowego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 4,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 35,
                Name = "badanie per rectum",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 20,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 36,
                Name = "badanie per rectum",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 37,
                Name = "przetoczenie krwi lub preparatu krwiopochodnego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 20,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 38,
                Name = "przetoczenie krwi lub preparatu krwiopochodnego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 39,
                Name = "wprowadzenie zgłębnika do żołądka",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 5,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 40,
                Name = "wprowadzenie zgłębnika do żołądka",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 41,
                Name = "wykonanie i interpretacja 12-odprowadzeniowego EKG",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 30,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 42,
                Name = "wykonanie i interpretacja 12-odprowadzeniowego EKG",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 43,
                Name = "badanie palpacyjne gruczołu piersiowego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 10,
                Module = ModuleType.Basic,
                InternshipId = 8
            },
            new MedicalProcedure {
                Id = 44,
                Name = "badanie palpacyjne gruczołu piersiowego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 2,
                Module = ModuleType.Basic,
                InternshipId = 8
            }
        });

            // Procedury w stażach kierunkowych (moduł podstawowy)
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
        {
            new MedicalProcedure {
                Id = 45,
                Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 5,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 46,
                Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 1,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 47,
                Name = "kardiowersja elektryczna",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 48,
                Name = "kardiowersja elektryczna",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 49,
                Name = "intubacja dotchawicza",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 10,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 50,
                Name = "intubacja dotchawicza",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 51,
                Name = "pomiar szczytowego przepływu wydechowego",
                ProcedureType = ProcedureType.TypeA,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 9
            },
            new MedicalProcedure {
                Id = 52,
                Name = "pomiar szczytowego przepływu wydechowego",
                ProcedureType = ProcedureType.TypeB,
                RequiredCount = 3,
                Module = ModuleType.Basic,
                InternshipId = 9
            }
        });

            return specialization;
        }
    }
}