// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSeeder.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Seeder do bazy danych. Absolutne źródło wiedzy i prawdy dla całej aplikacji
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    /// <summary>
    /// Klasa do zasilenia bazy danych specjalizacjami.
    /// Dane pochodzą z oficjalnej dokumentacji.
    /// </summary>
    [SuppressMessage("SonarCloud", "S1192", Justification = "DataSeeder")]
    public static class DataSeeder
    {
        /// <summary>
        /// Zasila bazę danych specjalizacją z zakresu hematologii.
        /// </summary>
        /// <returns>Obiekt Specialization reprezentujący specjalizację z zakresu hematologii.</returns>
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
                RequiredDutyHoursPerWeek = (int)(10 + (double)(5 / 60)),  // 10 godzin 5 minut
                RequiresPublication = true,
                RequiredConferences = 3,
            };

            // Dodanie kursów specjalizacyjnych (moduł specjalistyczny)
            specialization.RequiredCourses.AddRange(new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Name = "Wprowadzenie do specjalizacji w dziedzinie hematologii i zagadnienia promocji zdrowia w hematologii",
                    DurationDays = 3,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 2,
                    Name = "Badanie cytologiczne szpiku oraz histologiczne szpiku i węzłów chłonnych",
                    DurationDays = 5,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 3,
                    Name = "Diagnostyka immunofenotypowa",
                    DurationDays = 1,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 4,
                    Name = "Diagnostyka cytogenetyczna i molekularna nowotworów krwi",
                    DurationDays = 1,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 5,
                    Name = "Zaburzenia hemostazy",
                    DurationDays = 3,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 6,
                    Name = "Immunohematologia",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 7,
                    Name = "Przeszczepianie komórek krwiotwórczych",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 8,
                    Name = "Psychologiczne problemy pacjentów z chorobami krwi i układu chłonnego",
                    DurationDays = 1,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 9,
                    Name = "Onkologia guzów litych dla hematologów",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 10,
                    Name = "Hematolog jako konsultant",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 11,
                    Name = "Kurs atestacyjny (podsumowujący): Hematologia",
                    DurationDays = 5,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
            });

            // Dodanie kursów specjalizacyjnych (moduł podstawowy z chorób wewnętrznych)
            specialization.RequiredCourses.AddRange(new List<Course>
            {
                new Course
                {
                    Id = 12,
                    Name = "Diagnostyka obrazowa",
                    DurationDays = 3,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 13,
                    Name = "Alergologia",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 14,
                    Name = "Onkologia kliniczna",
                    DurationDays = 4,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 15,
                    Name = "Medycyna paliatywna",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 16,
                    Name = "Toksykologia",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 17,
                    Name = "Geriatria",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 18,
                    Name = "Diabetologia",
                    DurationDays = 4,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 19,
                    Name = "Przetaczanie krwi i jej składników",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 20,
                    Name = "Orzecznictwo lekarskie",
                    DurationDays = 3,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Course
                {
                    Id = 21,
                    Name = "Profilaktyka i promocja zdrowia",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
            });

            // Dodanie staży (moduł specjalistyczny)
            specialization.RequiredInternships.AddRange(new List<Internship>
            {
                new Internship
                {
                    Id = 1,
                    Name = "Staż podstawowy w zakresie hematologii",
                    DurationWeeks = 101,
                    WorkingDays = 505,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 2,
                    Name = "Staż kierunkowy w laboratorium hematologicznym",
                    DurationWeeks = 4,
                    WorkingDays = 20,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 3,
                    Name = "Staż kierunkowy w ośrodku przeszczepiania szpiku akredytowanym do wykonywania przeszczepień allogenicznych",
                    DurationWeeks = 4,
                    WorkingDays = 20,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 4,
                    Name = "Staż kierunkowy w hematologicznym oddziale dziennego leczenia",
                    DurationWeeks = 5,
                    WorkingDays = 25,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 5,
                    Name = "Staż kierunkowy w poradni hematologicznej",
                    DurationWeeks = 8,
                    WorkingDays = 40,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 6,
                    Name = "Staż kierunkowy w Regionalnym Centrum Krwiodawstwa i Krwiolecznictwa",
                    DurationWeeks = 1,
                    WorkingDays = 5,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 7,
                    Name = "Staż kierunkowy w ośrodku leczenia hemofilii i pokrewnych skaz krwotocznych",
                    DurationWeeks = 3,
                    WorkingDays = 15,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
            });

            // Dodanie staży (moduł podstawowy)
            specialization.RequiredInternships.AddRange(new List<Internship>
            {
                new Internship
                {
                    Id = 8,
                    Name = "Staż podstawowy w zakresie chorób wewnętrznych",
                    DurationWeeks = 67,
                    WorkingDays = 335,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Internship
                {
                    Id = 9,
                    Name = "Staż kierunkowy w zakresie intensywnej opieki medycznej",
                    DurationWeeks = 4,
                    WorkingDays = 20,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
                new Internship
                {
                    Id = 10,
                    Name = "Staż kierunkowy w szpitalnym oddziale ratunkowym",
                    DurationWeeks = 12,
                    WorkingDays = 60,
                    IsRequired = true,
                    Module = ModuleType.Basic,
                },
            });

            // Dodanie procedur medycznych (moduł specjalistyczny) - staż podstawowy
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 1,
                    Name = "biopsja szpiku z mostka",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 2,
                    Name = "biopsja szpiku z mostka",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 3,
                    Name = "biopsja szpiku z kolca tylnego talerza biodrowego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 4,
                    Name = "biopsja szpiku z kolca tylnego talerza biodrowego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 5,
                    Name = "biopsja szpiku z kolca przedniego talerza biodrowego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 6,
                    Name = "biopsja szpiku z kolca przedniego talerza biodrowego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 7,
                    Name = "trepanobiopsja szpiku",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 8,
                    Name = "trepanobiopsja szpiku",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 9,
                    Name = "prowadzenie intensywnego leczenia chorych na ostre białaczki szpikowe lub ostre białaczki limfoblastyczne",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 7,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 10,
                    Name = "prowadzenie leczenia indukującego chorych na chłoniaki agresywne",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 4,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 11,
                    Name = "afereza lecznicza",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 1,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 12,
                    Name = "zakładanie centralnego cewnika żylnego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 13,
                    Name = "punkcja lędźwiowa i dokanałowe podanie cytostatyków",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 14,
                    Name = "biopsja cienkoigłowa węzła lub biopsja węzła pod kontrolą USG/TK",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
            });

            // Dodanie procedur medycznych (moduł specjalistyczny) - staże kierunkowe
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 15,
                    Name = "ocena rozmazu krwi obwodowej",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2,
                },
                new MedicalProcedure
                {
                    Id = 16,
                    Name = "ocena rozmazu krwi obwodowej",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 30,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2,
                },
                new MedicalProcedure
                {
                    Id = 17,
                    Name = "ocena mielogramu",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2,
                },
                new MedicalProcedure
                {
                    Id = 18,
                    Name = "ocena mielogramu",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 30,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2,
                },
                new MedicalProcedure
                {
                    Id = 19,
                    Name = "przeszczepienie autologicznych komórek krwiotwórczych",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Specialistic,
                    InternshipId = 3,
                },
                new MedicalProcedure
                {
                    Id = 20,
                    Name = "przeszczepienie allogenicznych komórek krwiotwórczych",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 1,
                    Module = ModuleType.Specialistic,
                    InternshipId = 3,
                },
            });

            // Dodanie procedur medycznych (moduł podstawowy) - staż podstawowy
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 21,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 22,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 23,
                    Name = "nakłucie jamy opłucnej w przypadku płynu",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 24,
                    Name = "nakłucie jamy opłucnej w przypadku płynu",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 25,
                    Name = "nakłucie jamy otrzewnej w przypadku wodobrzusza",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 26,
                    Name = "nakłucie jamy otrzewnej w przypadku wodobrzusza",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 27,
                    Name = "nakłucia żył obwodowych – iniekcje dożylne, pobrania krwi obwodowej",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 28,
                    Name = "nakłucia żył obwodowych – iniekcje dożylne, pobrania krwi obwodowej",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 29,
                    Name = "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 30,
                    Name = "nakłucie tętnicy obwodowej w celu pobrania krwi do badania gazometrycznego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
            });

            // Dodanie pozostałych procedur medycznych (moduł podstawowy)
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 31,
                    Name = "pomiar ośrodkowego ciśnienia żylnego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 6,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 32,
                    Name = "pomiar ośrodkowego ciśnienia żylnego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 33,
                    Name = "cewnikowanie pęcherza moczowego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 34,
                    Name = "cewnikowanie pęcherza moczowego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 4,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 35,
                    Name = "badanie per rectum",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 36,
                    Name = "badanie per rectum",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 37,
                    Name = "przetoczenie krwi lub preparatu krwiopochodnego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 38,
                    Name = "przetoczenie krwi lub preparatu krwiopochodnego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 39,
                    Name = "wprowadzenie zgłębnika do żołądka",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 40,
                    Name = "wprowadzenie zgłębnika do żołądka",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 41,
                    Name = "wykonanie i interpretacja 12-odprowadzeniowego EKG",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 42,
                    Name = "wykonanie i interpretacja 12-odprowadzeniowego EKG",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 43,
                    Name = "badanie palpacyjne gruczołu piersiowego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
                new MedicalProcedure
                {
                    Id = 44,
                    Name = "badanie palpacyjne gruczołu piersiowego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Basic,
                    InternshipId = 8,
                },
            });

            // Procedury w stażach kierunkowych (moduł podstawowy)
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 45,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 46,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 1,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 47,
                    Name = "kardiowersja elektryczna",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 48,
                    Name = "kardiowersja elektryczna",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 49,
                    Name = "intubacja dotchawicza",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 50,
                    Name = "intubacja dotchawicza",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 51,
                    Name = "pomiar szczytowego przepływu wydechowego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
                new MedicalProcedure
                {
                    Id = 52,
                    Name = "pomiar szczytowego przepływu wydechowego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 3,
                    Module = ModuleType.Basic,
                    InternshipId = 9,
                },
            });

            return specialization;
        }

        /// <summary>
        /// Zasila bazę danych specjalizacją z zakresu alergologii.
        /// </summary>
        /// <returns>/Obiekt Specialization reprezentujący specjalizację z zakresu alergologii.</returns>
        public static Specialization SeedAllergologySpecialization()
        {
            var specialization = new Specialization
            {
                Id = 2,
                Name = "Alergologia",
                StartDate = DateTime.Now,
                ExpectedEndDate = DateTime.Now.AddYears(3),
                BaseDurationWeeks = 156, // 3 lata (156 tygodni i 3 dni = 783 dni robocze)
                BasicModuleDurationWeeks = 104, // 2 lata (wcześniej zrealizowany moduł podstawowy chorób wewnętrznych)
                SpecialisticModuleDurationWeeks = 156, // 3 lata
                VacationDaysPerYear = 26,
                SelfEducationDaysPerYear = 6,
                StatutoryHolidaysPerYear = 13,
                RequiredDutyHoursPerWeek = (int)(10 + (double)(5 / 60)),  // 10 godzin 5 minut
                RequiresPublication = true,
                RequiredConferences = 0, // Brak konkretnej informacji o wymaganej liczbie konferencji
            };

            // Dodanie kursów specjalizacyjnych
            specialization.RequiredCourses.AddRange(new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Name = "Wprowadzenie do specjalizacji w dziedzinie alergologii",
                    DurationDays = 1,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 2,
                    Name = "Diagnostyka chorób alergicznych",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 3,
                    Name = "Profilaktyka i leczenie chorób alergicznych",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 4,
                    Name = "Immunoterapia alergenowa chorób alergicznych",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 5,
                    Name = "Alergia zawodowa",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 6,
                    Name = "Terapia inhalacyjna w alergologii",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 7,
                    Name = "Podstawy immunologii klinicznej i alergologii",
                    DurationDays = 3,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 8,
                    Name = "Choroby alergiczne górnych dróg oddechowych i oczu",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 9,
                    Name = "Alergologia w chorobach wewnętrznych. Choroby alergiczne układu oddechowego, POChP i anafilaksja",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 10,
                    Name = "Choroby alergiczne skóry, obrzęki naczynioruchowe, mastocytoza",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 11,
                    Name = "Odrębności chorób alergicznych u dzieci",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 12,
                    Name = "Nadwrażliwość na leki",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 13,
                    Name = "Nadwrażliwość na pokarmy oraz dodatki do żywności",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 14,
                    Name = "Leczenie biologiczne w alergologii",
                    DurationDays = 2,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Course
                {
                    Id = 15,
                    Name = "Kurs atestacyjny (podsumowujący): Alergologia",
                    DurationDays = 5,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
            });

            // Dodanie staży specjalizacyjnych
            specialization.RequiredInternships.AddRange(new List<Internship>
            {
                new Internship
                {
                    Id = 1,
                    Name = "Staż podstawowy w zakresie alergologii",
                    DurationWeeks = 94,
                    WorkingDays = 470,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 2,
                    Name = "Staż kierunkowy w zakresie diagnostyki laboratoryjnej",
                    DurationWeeks = 1,
                    WorkingDays = 5,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 3,
                    Name = "Staż kierunkowy w zakresie dermatologii i wenerologii",
                    DurationWeeks = 10,
                    WorkingDays = 50,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 4,
                    Name = "Staż kierunkowy w zakresie otorynolaryngologii",
                    DurationWeeks = 10,
                    WorkingDays = 50,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
                new Internship
                {
                    Id = 5,
                    Name = "Staż kierunkowy w zakresie pediatrii",
                    DurationWeeks = 10,
                    WorkingDays = 50,
                    IsRequired = true,
                    Module = ModuleType.Specialistic,
                },
            });

            // Dodanie procedur medycznych
            specialization.RequiredProcedures.AddRange(new List<MedicalProcedure>
            {
                new MedicalProcedure
                {
                    Id = 1,
                    Name = "wykonanie i interpretacja punktowych testów skórnych",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 2,
                    Name = "wykonanie i interpretacja punktowych testów skórnych",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 80,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 3,
                    Name = "wykonanie i interpretacja testów śródskórnych",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 4,
                    Name = "wykonanie i interpretacja testów śródskórnych",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 5,
                    Name = "wykonanie i interpretacja testów kontaktowych",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 6,
                    Name = "wykonanie i interpretacja testów kontaktowych",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 7,
                    Name = "wykonanie i interpretacja spirometrii",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 30,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 8,
                    Name = "wykonanie i interpretacja spirometrii",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 50,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 9,
                    Name = "wykonanie iniekcji podskórnych preparatów odczulających",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 10,
                    Name = "wykonanie iniekcji podskórnych preparatów odczulających",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 200,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 11,
                    Name = "wykonanie pomiaru PEF z edukacją pacjenta",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 12,
                    Name = "wykonanie pomiaru PEF z edukacją pacjenta",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 13,
                    Name = "wykonanie i interpretacja próby rozkurczowej lub próby prowokacji oskrzeli",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 14,
                    Name = "wykonanie i interpretacja próby rozkurczowej lub próby prowokacji oskrzeli",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 15,
                    Name = "podanie leku biologicznego",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 16,
                    Name = "podanie leku biologicznego",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 17,
                    Name = "przeprowadzenie edukacji chorego dotyczącej właściwego sposobu przyjmowania leków wziewnych – wybór inhalatora/nebulizatora",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 18,
                    Name = "przeprowadzenie edukacji chorego dotyczącej właściwego sposobu przyjmowania leków wziewnych – wybór inhalatora/nebulizatora",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 19,
                    Name = "wykonanie i interpretacja testów śródskórnych w diagnostyce na jady owadów lub na leki",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 20,
                    Name = "wykonanie i interpretacja testów śródskórnych w diagnostyce na jady owadów lub na leki",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 21,
                    Name = "wykonanie i interpretacja wyniku próby prowokacji oskrzeli",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 2,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 22,
                    Name = "wykonanie i interpretacja wyniku próby prowokacji oskrzeli",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 23,
                    Name = "wykonanie iniekcji podskórnych w okresie wstępnym odczulania na jady owadów",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 24,
                    Name = "wykonanie iniekcji podskórnych w okresie wstępnym odczulania na jady owadów",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 25,
                    Name = "interpretacja wyniku gazometrii krwi tętniczej",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 26,
                    Name = "interpretacja wyniku gazometrii krwi tętniczej",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 27,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 0,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 28,
                    Name = "prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 2,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 29,
                    Name = "przeprowadzenie edukacji chorego dotyczącej rozpoznawania i leczenia wstrząsu anafilaktycznego wraz ze szkoleniem dotyczącym podawania adrenaliny",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 30,
                    Name = "przeprowadzenie edukacji chorego dotyczącej rozpoznawania i leczenia wstrząsu anafilaktycznego wraz ze szkoleniem dotyczącym podawania adrenaliny",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 31,
                    Name = "zaplanowanie diagnostyki i interpretacja wyników u chorego z obrzękiem naczynioruchowym",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 32,
                    Name = "zaplanowanie diagnostyki i interpretacja wyników u chorego z obrzękiem naczynioruchowym",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 33,
                    Name = "zaplanowanie diagnostyki i interpretacja wyników u chorego diagnozowanego w kierunku mastocytozy",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },
                new MedicalProcedure
                {
                    Id = 34,
                    Name = "zaplanowanie diagnostyki i interpretacja wyników u chorego diagnozowanego w kierunku mastocytozy",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 1,
                },

                // Procedury dla staży kierunkowych (w dokumentacji zaznaczono, że nie podlegają rozliczeniu w EKS)
                new MedicalProcedure
                {
                    Id = 35,
                    Name = "interpretacja wyników badań diagnostyki molekularnej, w tym typu multipleks",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2, // Staż w zakresie diagnostyki laboratoryjnej
                },
                new MedicalProcedure
                {
                    Id = 36,
                    Name = "interpretacja wyników badań diagnostyki molekularnej, w tym typu multipleks",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 2,
                },
                new MedicalProcedure
                {
                    Id = 37,
                    Name = "ustalenie planu żywieniowego (zasady stosowania diety hipoalergicznej) u dziecka z alergią na pokarmy",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 5, // Staż w zakresie pediatrii
                },
                new MedicalProcedure
                {
                    Id = 38,
                    Name = "ustalenie planu żywieniowego (zasady stosowania diety hipoalergicznej) u dziecka z alergią na pokarmy",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 5,
                },
                new MedicalProcedure
                {
                    Id = 39,
                    Name = "zagadnienia dotyczące pielęgnacji skóry w atopowym zapaleniu skóry",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 3, // Staż w zakresie dermatologii
                },
                new MedicalProcedure
                {
                    Id = 40,
                    Name = "zagadnienia dotyczące pielęgnacji skóry w atopowym zapaleniu skóry",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 3,
                },
                new MedicalProcedure
                {
                    Id = 41,
                    Name = "wziernikowanie nosa",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4, // Staż w zakresie otorynolaryngologii
                },
                new MedicalProcedure
                {
                    Id = 42,
                    Name = "wziernikowanie nosa",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 43,
                    Name = "ocena tomografii komputerowej zatok",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 44,
                    Name = "ocena tomografii komputerowej zatok",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 20,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 45,
                    Name = "badanie węchu",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 46,
                    Name = "badanie węchu",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 10,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 47,
                    Name = "rynomanometria",
                    ProcedureType = ProcedureType.TypeA,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
                new MedicalProcedure
                {
                    Id = 48,
                    Name = "rynomanometria",
                    ProcedureType = ProcedureType.TypeB,
                    RequiredCount = 5,
                    Module = ModuleType.Specialistic,
                    InternshipId = 4,
                },
            });

            return specialization;
        }
    }
}