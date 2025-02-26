using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SledzSpecke.Core.Models.Domain;
using static SledzSpecke.Infrastructure.Database.Initialization.DataSeeder;

namespace SledzSpecke.Infrastructure.Database.Initialization
{
    public static class DataSeeder
    {
        public class RequiredProcedure
        {
            public string Name { get; set; }
            public int RequiredCount { get; set; }
            public int AssistanceCount { get; set; }
            public string? Description { get; set; }
            public bool AllowSimulation { get; set; } = false;
            public int? SimulationLimit { get; set; } // procent procedur możliwych do wykonania na symulatorach
        }

        public static List<Specialization> GetBasicSpecializations()
        {
            return new List<Specialization>
            {
                new Specialization
                {
                    Id = 1, // Kolejny numer po wcześniejszych specjalizacjach
                    Name = "Hematologia",
                    DurationInWeeks = 156, // 3 lata (52 tygodnie × 3)
                    ProgramVersion = "2023",
                    ApprovalDate = new DateTime(2023, 1, 1),
                    MinimumDutyHours = 480, // 10 godzin 5 minut na tydzień
                    Requirements = "Program specjalizacji w dziedzinie hematologii dla lekarzy, którzy zrealizowali i zaliczyli moduł podstawowy w zakresie chorób wewnętrznych",
                    Description = "Celem szkolenia specjalizacyjnego jest opanowanie wiedzy teoretycznej i umiejętności praktycznych umożliwiających diagnozowanie, profilaktykę i leczenie chorób na poziomie zapewniającym samodzielne udzielanie świadczeń zdrowotnych według najwyższych standardów."
                },
                new Specialization
                {
                    Id = 2, // Kolejny ID po psychiatrii, chirurgii onkologicznej i intensywnej terapii
                    Name = "Medycyna morska i tropikalna",
                    DurationInWeeks = 104, // 2 lata modułu specjalistycznego
                    ProgramVersion = "2023",
                    ApprovalDate = new DateTime(2023, 1, 1),
                    MinimumDutyHours = 480, // 10 godzin 5 minut na tydzień
                    Requirements = "Program specjalizacji w dziedzinie medycyny morskiej i tropikalnej dla lekarzy, którzy zrealizowali i zaliczyli moduł podstawowy w zakresie chorób wewnętrznych",
                    Description = "Celem szkolenia specjalizacyjnego jest uzyskanie wiedzy teoretycznej i umiejętności praktycznych w diagnozowaniu, profilaktyce i leczeniu chorób z zakresu medycyny morskiej i tropikalnej. Obszar specjalizacji obejmuje potrzeby zdrowotne pracowników gospodarki morskiej (w tym osób poddanych hiperbarii), osób podróżujących do odmiennych warunków klimatycznych, fizjograficznych i sanitarnych oraz zatrudnionych/pełniących służbę w tych warunkach."
                },
            };
        }

        public static List<CourseDefinition> GetBasicCourses()
        {
            return new List<CourseDefinition>
            {
                new CourseDefinition
                {
                    Name = "Wprowadzenie do specjalizacji w dziedzinie hematologii i zagadnienia promocji zdrowia w hematologii",
                    Description = "Nabycie przez lekarzy wiedzy na temat organizacyjnych i prawnych zasad funkcjonowania hematologa, podstaw naukowych hematologii oraz zasad prowadzenia badań klinicznych.",
                    DurationInHours = 24,
                    DurationInDays = 3,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Wprowadzenie w problematykę, cele i obszar działania hematologii",
                        "Zadania, kompetencje i oczekiwane wyniki szkolenia specjalisty w tej dziedzinie",
                        "Formalnoprawne podstawy doskonalenia zawodowego lekarzy z ukierunkowaniem na hematologię",
                        "Nadzór specjalistyczny w hematologii",
                        "Referencyjność w hematologii",
                        "Specjalizacja z hematologii oraz kształcenie ustawiczne w hematologii w Unii Europejskiej",
                        "System kontraktowania świadczeń medycznych w hematologii",
                        "Przepisy o przeszczepianiu komórek, tkanek i narządów",
                        "Przepisy o publicznej służbie krwi",
                        "Zagadnienia bezpieczeństwa w opiece zdrowotnej dotyczące bezpieczeństwa pacjentów i lekarzy",
                        "Podstawy dobrej praktyki lekarskiej, w tym zasady praktyki opartej na rzetelnych i aktualnych publikacjach",
                        "Zasady badań klinicznych: badania I, II i III fazy, interpretacja wyników, metaanaliza, sposoby oceny, badania wieloośrodkowe, finansowanie badań",
                        "Wprowadzenie do przedmiotów klinicznych objętych programem danego szkolenia specjalizacyjnego",
                        "Podstawy onkologii",
                        "Epidemiologia chorób krwi: częstość występowania w populacji europejskiej i polskiej",
                        "Hematopoeza: definicja komórki macierzystej, podścielisko krwiotwórcze, kinetyka komórek, apoptoza, geny regulujące krwiotworzenie, onkogeny i antyonkogeny, regulacja hematopoezy przez cytokiny i cząsteczki adhezyjne",
                        "Immunologia: rodzaje komórek odpornościowych, odporność nieswoista i swoista, budowa przeciwciał, receptorów, mechanizmy",
                        "Odporności przeciwzakaźnej i przeciwnowotworowej, autoagresja",
                        "Immunogenetyka i immunologia przeszczepowa",
                        "Podstawy farmakoekonomiki",
                        "Farmakologia i farmakokinetyka leków wykorzystywanych w hematologii",
                        "Mechanizmy działania, wchłanianie i metabolizm, usuwanie leków, szczególnie znajomość cytostatyków wykorzystywanych w hematologii oraz tzw. środków biologicznych",
                        "Leki immunomodulujące, przeciwciała monoklonalne, terapia celowana, szczepionki przeciwnowotworowe",
                        "Farmakologia i farmakokinetyka leków przeciwinfekcyjnych: przeciwbakteryjnych, przeciwgrzybiczych i przeciwwirusowych, stosowanie szczepionek u pacjentów z chorobami krwi",
                        "Podstawy terapii genowej w leczeniu hemofilii, wykorzystanie komórek modyfikowanych genetycznie w hematologii (zastosowanie komórek CAR)"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Badanie cytologiczne szpiku oraz histologiczne szpiku i węzłów chłonnych",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej na temat zaburzeń w morfologii krwi obwodowej, biopsji aspiracyjnej szpiku kostnego, trepanobiopsji szpiku oraz badaniu histopatologicznym węzła chłonnego w przebiegu chorób krwi nowotworowych i nienowotworowych.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = false,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Budowa i obsługa mikroskopu, sposób przygotowania go do pracy, ustawienie kondensora, dobór powiększenia, wykonywanie dokumentacji fotograficznej",
                        "Warunki, którym musi odpowiadać prawidłowo wykonany rozmaz szpiku, sposób barwienia, cytochemia",
                        "Formy komórkowe spotykane w szpiku kostnym: definicje i cechy charakterystyczne umożliwiające ich rozróżnienie",
                        "Ocena cytologiczna rozmazu krwi obwodowej i szpiku kostnego w przebiegu cytopenii, infekcji oraz schorzeń innych narządów, w tym przerzutów nowotworowych w szpiku",
                        "Ocena cytologiczna rozmazu krwi obwodowej i szpiku kostnego w nowotworach układu krwiotwórczego i chłonnego",
                        "Ocena histopatologiczna szpiku i węzłów chłonnych w nowotworach układu krwiotwórczego i chłonnego",
                        "Trepanobioptat szpiku kostnego – wskazania, komplementarność w stosunku do biopsji aspiracyjnej, wstępna ocena czy preparat nadaje się do oceny histopatologicznej",
                        "Obraz szpiku kostnego po przeszczepieniu krwiotwórczych komórek macierzystych"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Diagnostyka immunofenotypowa",
                    Description = "Nabycie przez lekarzy wiedzy na temat zastosowania technik cytometrycznych w hematologii oraz zasad diagnostyki chorób układu krwiotwórczego i chłonnego w oparciu o badania immunofenotypowe.",
                    DurationInHours = 8,
                    DurationInDays = 1,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Techniczne podstawy cytometrii przepływowej",
                        "Markery powierzchniowe wykorzystywane w diagnostyce hematologicznej",
                        "Diagnostyka immunofenotypowa chorób układu krwiotwórczego i chłonnego",
                        "Ocena ilościowa komórek CD34-dodatnich",
                        "Diagnostyka choroby resztkowej w hematologii",
                        "Badanie subpopulacji limfocytów",
                        "Badanie płytek krwi"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Diagnostyka cytogenetyczna i molekularna nowotworów krwi",
                    Description = "Nabycie przez lekarzy wiedzy na temat zastosowania technik genetycznych w hematologii oraz zasad diagnostyki chorób układu krwiotwórczego i chłonnego w oparciu o badania cytogenetyczne i molekularne.",
                    DurationInHours = 8,
                    DurationInDays = 1,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Aktualna klasyfikacja nowotworów układu krwiotwórczego i chłonnego według WHO, rola diagnostyki cytogenetycznej i molekularnej",
                        "Diagnostyka cytogenetyczna nowotworów układu krwiotwórczego (klasyczna, prążkowa i FISH)",
                        "Diagnostyka cytogenetyczna nowotworów układu chłonnego (klasyczna, prążkowa i FISH)",
                        "Diagnostyka molekularna nowotworów układu krwiotwórczego (PCR, mikromacierze, NGS)",
                        "Diagnostyka molekularna nowotworów układu chłonnego (PCR, mikromacierze, NGS)",
                        "Monitorowanie molekularne odpowiedzi na leczenie: ocena remisji, ocena choroby resztkowej, wykorzystanie markerów molekularnych",
                        "Ocena chimeryzmu po przeszczepieniu szpiku"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Zaburzenia hemostazy",
                    Description = "Nabycie przez lekarzy wiedzy na temat nabytych i wrodzonych zaburzeń hemostazy, w tym interpretacji wyników badań układu krzepnięcia.",
                    DurationInHours = 24,
                    DurationInDays = 3,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Mechanizmy krzepnięcia i fibrynolizy",
                        "Podejście diagnostyczne do pacjenta krwawiącego",
                        "Choroba von Willebranda",
                        "Hemofilia A i B oraz hemofilia powikłana inhibitorem",
                        "Osoczopochodne i rekombinowane koncentraty czynników krzepnięcia, inne leki stosowane wleczeniu hemofilii (p/ciała monoklonalne) oraz preparaty „omijające\"",
                        "Rzadkie wrodzone niedobory czynników krzepnięcia",
                        "Organizacja opieki nad chorym na hemofilię i inne skazy krwotoczne w Polsce",
                        "Nabyte skazy krwotoczne: zaburzenia krzepnięcia w chorobach serca, nerek, wątroby i po transplantacjach, rozsiane krzepnięcie wewnątrznaczyniowe, nabyte inhibitory krzepnięcia – nabyta hemofilia, przedawkowanie leków przeciwkrzepliwych",
                        "Małopłytkowości – diagnostyka różnicowa i leczenie; małopłytkowość rzekoma",
                        "Stany nadkrzepliwości (trombofilie wrodzone i nabyte) – przyczyny, diagnostyka, leczenie i profilaktyka",
                        "Zakrzepowa plamica małopłytkowa",
                        "Żylna choroba zakrzepowo-zatorowa – diagnostyka, leczenie i profilaktyka; zespół Budda i Chiariego",
                        "Leczenie przeciwkrzepliwe, przeciwpłytkowe, fibrynolityczne i trombolityczne",
                        "Laboratoryjna kontrola leczenia przeciwzakrzepowego",
                        "Ciąża i poród u kobiet z zaburzeniami hemostazy"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Immunohematologia",
                    Description = "Nabycie przez lekarzy podstawowej wiedzy na temat transfuzjologii laboratoryjnej oraz zasad diagnostyki laboratoryjnej w cytopeniach immunologicznych.",
                    DurationInHours = 16,
                    DurationInDays = 2,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Immunologia komórek krwi i ich prekursorów",
                        "Cytopenie spowodowane immunizacją",
                        "Ocena defektów fenotypów/genotypów komórek krwi dla rozpoznania różnych typów cytopenii",
                        "Zasady dobierania krwinek czerwonych w celu uniknięcia alloimmunizacji oraz postępowanie u chorych alloimmunizowanych",
                        "Zasad dobierania płytek krwi do przetoczenia, unikanie oporności odpornościowej na przetaczanie płytek oraz postępowanie u chorych immunizowanych",
                        "Zasady przetaczania preparatów krwi u chorych po przeszczepieniu komórek krwiotwórczych oraz allo- i autoimmunizacja u tych chorych",
                        "Poprzetoczeniowe powikłania hemolityczne",
                        "Poprzetoczeniowe powikłania niehemolityczne",
                        "Konflikty matczyno-płodowe w zakresie antygenów erytrocytów, płytek i granulocytów oraz choroby płodów/noworodków wynikające z wytwarzania alloprzeciwciał u matek – zapobieganie, diagnostyka inwazyjna i nieinwazyjna, postępowanie lecznicze"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Przeszczepianie komórek krwiotwórczych",
                    Description = "Nabycie przez lekarzy wiedzy na temat przeszczepień krwiotwórczych komórek macierzystych, zasad opieki nad pacjentem oraz profilaktyki i leczenia powikłań.",
                    DurationInHours = 16,
                    DurationInDays = 2,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Kwalifikacja chorego do transplantacji komórek krwiotwórczych, zasady zgłaszania chorego do przeszczepienia i zakresu badań wymaganych do kwalifikacji",
                        "Wskazania do przeszczepienia allogenicznych i autologicznych komórek krwiotwórczych",
                        "Rodzaje transplantacji komórek krwiotwórczych",
                        "Zasady poszukiwania dawcy komórek krwiotwórczych wśród rodziny chorego, w tym przeszczepienia haploidentyczne",
                        "Zasady poszukiwania niespokrewnionego dawcy szpiku",
                        "Rejestry niespokrewnionych dawców szpiku",
                        "Źródła komórek krwiotwórczych, ich zalety i ograniczenia",
                        "Zasady intensywnej opieki medycznej po leczeniu mieloablacyjnym i przeszczepieniu komórek krwiotwórczych",
                        "Zasady opieki nad rekonwalescentem po przeszczepieniu komórek krwiotwórczych",
                        "Powikłania po przeszczepieniu krwiotwórczych komórek macierzystych w tym ostra i przewlekła choroba przeszczep przeciw gospodarzowi, powikłania infekcyjne po przeszczepieniu szpiku",
                        "Zasady dotyczące szczepień u pacjentów po transplantacji komórek krwiotwórczych"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Psychologiczne problemy pacjentów z chorobami krwi i układu chłonnego",
                    Description = "Nabycie przez lekarzy wiedzy na temat wpływu chorób krwi na psychofizyczne funkcjonowanie pacjenta oraz zasad poprawnej komunikacji pomiędzy lekarzem, pacjentem i rodziną.",
                    DurationInHours = 8,
                    DurationInDays = 1,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Istota kryzysu w sytuacji choroby przewlekłej i choroby śmiertelnej oraz jego wpływ na funkcjonowanie psychofizyczne pacjenta",
                        "Wpływ choroby nowotworowej na funkcjonowanie systemu rodzinnego",
                        "Zasady budowania poprawnej komunikacji między lekarzem, pacjentem i rodziną pacjenta",
                        "Informowanie pacjenta o chorobie, zasady uzyskiwania świadomej zgody na leczenie",
                        "Adaptacja psychospołeczna pacjenta z chorobą nowotworową po leczeniu, w tym po przeszczepieniu komórek krwiotwórczych",
                        "Problemy psychologiczne zespołu leczącego"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Onkologia guzów litych dla hematologów",
                    Description = "Nabycie przez lekarzy podstawowej wiedzy na temat zasad diagnostyku i leczenia guzów litych.",
                    DurationInHours = 16,
                    DurationInDays = 2,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Biologiczne odmienności guzów litych w stosunku do nowotworów krwi",
                        "Epidemiologia guzów litych",
                        "Cele chirurgicznego leczenia guzów litych",
                        "Zasady radioterapii guzów litych",
                        "Zasady chemioterapii i leczenia celowanego guzów litych",
                        "Powikłania hematologiczne u chorych na nowotwory",
                        "Hematologiczne skutki uboczne radio-i chemioterapii nowotworów",
                        "Algorytmy postępowania w najczęstszych guzach litych (rak płuca, rak piersi, rak przewodu pokarmowego, rak stercza, czerniak, guzy mózgu, guzy o nieznanym punkcie wyjścia)",
                        "Guzy lite współistniejące z nowotworami lub innymi chorobami krwi",
                        "Organizacja leczenia guzów litych w Polsce i na świecie",
                        "Referencyjność w onkologii"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Hematolog jako konsultant",
                    Description = "Nabycie przez lekarzy wiedzy na temat zaburzeń hematologicznych i zakrzepowo-zatorowych w przebiegu innych chorób oraz zasad leczenia chorób krwi w okresie ciąży i połogu.",
                    DurationInHours = 16,
                    DurationInDays = 2,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    CourseTopics = new List<string>
                    {
                        "Hematolog jako konsultant oddziałów chirurgii ogólnej",
                        "Hematolog jako konsultant oddziałów położniczo-ginekologicznych",
                        "Hematolog jako konsultant zabiegów transplantacji narządów",
                        "Powikłania zakrzepowo-zatorowe w chirurgii, położnictwie i ginekologii",
                        "Przygotowanie do zabiegów chirurgicznych chorych na: hemofilię i inne osoczowe skazy krwotoczne, małopłytkowość i nadpłytkowość",
                        "Opieka hematologiczna w okresie ciąży i połogu: konsultacje prenatalne w zakresie chorób hematologicznych uwarunkowanych genetycznie",
                        "Fizjologiczne zmiany w funkcjonowaniu układu krwiotwórczego oraz układu krzepnięcia w okresie ciąży i połogu",
                        "Postępowanie w niedokrwistości i małopłytkowości w przebiegu ciąży",
                        "Profilaktyka śródciążowa i okołoporodowa konfliktu serologicznego, postępowanie okołoporodowe w skazach krwotocznych"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Kurs atestacyjny (podsumowujący): Hematologia",
                    Description = "Usystematyzowanie przez lekarzy wiedzy teoretycznej i praktycznej na temat diagnostyki i leczenia chorób krwi nowotworowych i nienowotworowych.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 3,
                    SpecializationId = 1,
                    Requirements = "W ostatnim roku odbywania szkolenia specjalizacyjnego przed przystąpieniem do PES",
                    CourseTopics = new List<string>
                    {
                        "Nienowotworowe choroby hematologiczne – zasady diagnostyki i leczenia (np. niedokrwistości, leukopenie, granulocytopenie, małopłytkowości, aplazja szpiku)",
                        "Pierwotne i wtórne niedobory odporności",
                        "Patogeneza nowotworów układu krwiotwórczego i chłonnego",
                        "Klasyfikacja i kryteria diagnostyczne nowotworów układu krwiotwórczego i chłonnego",
                        "Nowotwory układu krwiotwórczego – zasady diagnostyki i leczenia (np. ostre białaczki szpikowe, zespoły mielodysplastyczne, nowotwory mieloproliferacyjne, nowotwory mielodysplastyczno-mieloproliferacyjne, nowotwory mieloidalne z eozynofilią i nieprawidłowościami genów PDGFRA, PDGFRB lub FGFR1)",
                        "Nowotwory układu chłonnego – zasady diagnostyki i leczenia (ostre białaczki limfoblastyczne/ chłoniaki limfoblastyczne, chłoniaki nie-Hodgkina B i T-komórkowe, chłoniak Hodgkina, przewlekła białaczka limfocytowa, białaczka włochatokomórkowa i inne rzadsze postacie przewlekłych białaczek limfoidalnych, nowotwory z komórek plazmatycznych, zespół POEMS i gammapatie monoklonalne)",
                        "Profilaktyka i leczenie granulocytopenii, rozpoznawanie i leczenie gorączki neutropenicznej",
                        "Powikłania infekcyjne: zakażenia bakteryjne, zakażenia grzybicze, zakażenia wirusowe ze szczególnym uwzględnieniem wirusów cytomegalii (CMV) i wirusa SARS-CoV-2",
                        "Wrodzone i nabyte skazy krwotoczne",
                        "Stany bezpośredniego zagrożenia życia i leczenie wspomagające w hematologii: hiperleukocytoza, zespół nadlepkości, rozsiane krzepnięcie wewnątrznaczyniowe zespół lizy guza, ucisk rdzenia kręgowego, zespół żyły głównej górnej, profilaktyka i leczenie zapalenia błon śluzowych, nudności i wymiotów, prowadzenie leczenia przeciwbólowego, żywienie dojelitowe i pozajelitowe",
                        "Zespół hemofagocytowy",
                        "Porfirie",
                        "Postępowanie diagnostyczne i terapeutyczne w chorobach nowotworowych u kobiet w ciąży",
                        "Odległe działania niepożądane leczenia cytostatycznego i radioterapii (powikłania kardiologiczne, pulmonologiczne, endokrynologiczne, wpływ na funkcje rozrodcze)",
                        "Opieka nad chorym w stanie terminalnym"
                    },
                    CompletionRequirements = "Zaliczenie kolokwium sprawdzającego wiedzę nabytą w trakcie szkolenia specjalizacyjnego oraz zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                new CourseDefinition
                {
                    Name = "Wprowadzenie do specjalizacji z medycyny morskiej i tropikalnej",
                    Description = "Zaznajomienie lekarza z problematyką licznych aspektów medyczno-społecznych, organizacyjno-prawnych, higieną życia i pracy na morzu oraz w krajach o odmiennym klimacie i zagrożeniach zdrowotnych.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    CourseTopics = new List<string>
                    {
                        "Podstawy dobrej praktyki lekarskiej w tym zasady praktyki opartej na rzetelnych i aktualnych publikacjach",
                        "Rola i zadania Państwowej Inspekcji Pracy, Państwowej Inspekcji Sanitarnej oraz służb bezpieczeństwa i higieny pracy",
                        "Podstawy farmakoekonomiki",
                        "Psychologiczne aspekty życia i pracy na statku, platformach wiertniczych oraz w krajach klimatu gorącego",
                        "Międzynarodowe i krajowe przepisy sanitarne dotyczące chorób transmisyjnych, zakaźnych i pasożytniczych oraz ich kontroli w żegludze i portach",
                        "Formalnoprawne podstawy doskonalenia zawodowego lekarzy",
                        "Zasady radio-poradnictwa, tele-medycyna, udzielanie pierwszej pomocy przedlekarskiej i pomocy lekarskiej na statku, dokumentacja medyczna, zasób środków i urządzeń medycznych na statku",
                        "Medyczne aspekty ratowania rozbitków, organizacja Morskiej Służby Poszukiwania i Ratownictwa (Służba SAR)",
                        "Wpływ klimatu tropikalnego na zachorowalność i chorobowość",
                        "Epidemiologia, klinika, diagnostyka różnicowa, terapia i profilaktyka wybranych chorób tropikalnych",
                        "Wprowadzenie do przedmiotów klinicznych objętych programem danego szkolenia specjalizacyjnego",
                        "Choroby tropikalne jako choroby zawodowe",
                        "Zagadnienia bezpieczeństwa w opiece zdrowotnej dotyczące bezpieczeństwa pacjentów i lekarzy",
                        "Podstawy onkologii"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                // Kurs: Medycyna morska
                new CourseDefinition
                {
                    Name = "Medycyna morska",
                    Description = "Przygotowanie przyszłego specjalisty do właściwego przestrzegania zasad konwencji morskich, udzielania porad medycznych bezpośrednio na statku morskim lub drogą radiową, zasad nadzoru sanitarnego oraz badań profilaktycznych i orzecznictwa.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    CourseTopics = new List<string>
                    {
                        "Międzynarodowe konwencje morskie dotyczące ochrony zdrowia, warunków bytowych załóg oraz warunków sanitarnych na statkach, platformach wiertniczych i w portach",
                        "Transport chorych na morzu, ewakuacja chorych ze statku",
                        "Pomoc medyczna w zatruciach, w szczególności spowodowanych przez ładunki na statku",
                        "Zasady sterylizacji, dezynfekcji, dezynsekcji i deratyzacji",
                        "Epidemiologia zachorowalności marynarzy i rybaków",
                        "Higiena i fizjologia oraz ergonomia i psychologia pracy i życia na statku, zagadnienie zdrowia psychicznego oraz higieny odżywiania w czasie rejsów",
                        "Ekspozycja zawodowa na czynniki fizyczne, chemiczne i biologiczne występujące na statkach morskich, śródlądowych i rybackich oraz psychoemocjonalne związane z organizacją pracy na statku",
                        "Zagadnienia związane z zaopatrywaniem statku w wodę i żywność oraz z usuwaniem ścieków i odpadków",
                        "Patofizjologia utonięcia w wodach morskich i słodkich",
                        "Choroba morska",
                        "Zasady nadzoru sanitarnego w portach morskich ze szczególnym uwzględnieniem pracy portowej stacji sanitarno-epidemiologicznej",
                        "Przepisy dotyczące badań profilaktycznych marynarzy, rybaków, nurków i pracowników platform wiertniczych"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                // Kurs: Medycyna nurkowa z elementami medycyny hiperbarycznej
                new CourseDefinition
                {
                    Name = "Medycyna nurkowa z elementami medycyny hiperbarycznej",
                    Description = "Przekazanie podstawowej wiedzy na temat wpływu podwyższonego ciśnienia na ustrój człowieka, bezpieczeństwo i higiena pracy nurka, etiologia i patogeneza chorób nurkowych, podstawy dekompresji, pierwsza pomoc w wypadkach nurkowych, organizacja zabezpieczenia medycznego podczas nurkowania zawodowego i rekreacyjnego, orzecznictwo w następstwach wypadków nurkowych.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    CourseTopics = new List<string>
                    {
                        "Międzynarodowe i krajowe przepisy dotyczące nurkowania zawodowego, służbowego, sportowego i rekreacyjnego",
                        "Wpływ podwyższonego ciśnienia na organizm człowieka",
                        "Bezpieczeństwo i higiena pracy nurka",
                        "Higiena i fizjologia oraz ergonomia i psychologia pracy i życia w obiekcie hiperbarycznym, zagadnienie zdrowia psychicznego",
                        "Żywienie nurków i pracowników obiektów hiperbarycznych",
                        "Przepisy dotyczące badań profilaktycznych nurków",
                        "Etiologia chorób nurkowych",
                        "Podstawy teorii dekompresji",
                        "Choroba dekompresyjna",
                        "Uraz ciśnieniowy płuc",
                        "Problemy laryngologiczne związane z pobytem pod zwiększonym ciśnieniem – inne urazy ciśnieniowe",
                        "Działanie tlenu hiperbarycznego i jego zastosowanie w medycynie",
                        "Działanie innych gazów w warunkach podwyższonego ciśnienia",
                        "Organizacja zabezpieczenia medycznego różnych typów nurkowania",
                        "Termoregulacja w warunkach podwyższonego ciśnienia",
                        "Inne zagrożenia związane z wykonywaniem pracy, bądź turystyka podwodną",
                        "Pierwsza pomoc w wypadkach nurkowych",
                        "Leczenie przyczynowe wypadków nurkowych",
                        "Następstwa zdrowotne chorób nurkowych i pracy w warunkach podwyższonego ciśnienia otoczenia"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                // Kurs: Medycyna tropikalna
                new CourseDefinition
                {
                    Name = "Medycyna tropikalna",
                    Description = "Zapoznanie ze znajomością metod diagnostycznych, symptomatologii chorób tropikalnych i pasożytniczych, wybranych chorób zakaźnych, opanowanie podstaw stosowania określonych chemioterapeutyków, zalecanych szczepień ochronnych oraz innych metod profilaktyki.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    CourseTopics = new List<string>
                    {
                        "Choroby tropikalne i pasożytnicze (wybrane jednostki chorobowe)",
                        "Choroby skórne i weneryczne (wybrane jednostki chorobowe)",
                        "Choroby z przegrzania ustroju",
                        "Higiena i epidemiologia krajów tropikalnych",
                        "Higiena osobista oraz higiena pracy w warunkach tropikalnych",
                        "Higiena żywienia i problemy zaopatrzenia w wodę",
                        "Fizjologia i psychologia pracy w krajach tropikalnych",
                        "Niebezpieczne rośliny i zwierzęta w tropiku",
                        "Wektory, choroby transmisyjne i zoonozy w tropiku",
                        "Zasady diagnostyki laboratoryjnej chorób tropikalnych",
                        "Pobieranie, zabezpieczanie i przesyłanie materiałów do badań serologicznych, bakteriologicznych i parazytologicznych",
                        "Zasady i kryteria kwalifikacji zdrowotnej osób wyjeżdżających do krajów tropikalnych",
                        "Problemy bioterroryzmu",
                        "Uchodźcy, masowe migracje i konflikty zbrojne w krajach tropikalnych"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                // Kurs: Medycyna podróży
                new CourseDefinition
                {
                    Name = "Medycyna podróży",
                    Description = "Przygotowanie lekarzy specjalistów w tej dziedzinie do racjonalnego i zrozumiałego informowania osób decydujące się na odbycie podróży w rejony endemicznego występowania wielu chorób inwazyjnych, zasad higieny życia i żywienia w tropiku, zagrożeń związanych z podróżami, zwłaszcza przez osoby cierpiące na różne przewlekłe schorzenia, kontynuacja zaleceń profilaktycznych oraz konieczności badań kontrolnych w przypadkach koniecznych.",
                    DurationInHours = 24,
                    DurationInDays = 3,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    CourseTopics = new List<string>
                    {
                        "Sytuacja epidemiologiczna na świecie w świetle aktualnych zagrożeń zdrowotnych",
                        "Przygotowania do podróży (porada lekarska, szczepienia ochronna, chemioprofilaktyka i przeciwmalaryczna i przeciwbiegunkowa, apteczka podróżna)",
                        "Profilaktyka zdrowotna w podróży, ochrona przed stanami zapalnymi dróg oddechowych, ukłuciami owadów, higiena żywności i żywienia, środki ostrożności przed i działania po pogryzieniu przez zwierzęta, inne działania profilaktyczne)",
                        "Problemy zdrowotne w transporcie powietrznym, lądowym i morskim (Jet lag, zespół klasy ekonomicznej, wypadki komunikacyjne)",
                        "Aklimatyzacja, urazy cieplne, warunki wysokogórskie, niebezpieczna fauna lądowa i morska",
                        "Kobieta ciężarna i dziecko w podróży",
                        "Podróżny w podeszłym wieku i z upośledzoną odpornością",
                        "Podróże ekstremalne",
                        "Zaburzenia żołądkowo – jelitowe u podróżujących",
                        "Stany gorączkowe niewiadomego pochodzenia u podróżujących",
                        "Stany zapalne dróg oddechowych u podróżujących",
                        "Choroby skóry i przenoszone drogą płciową u podróżujących",
                        "Problemy zdrowotne po powrocie z podróży w aspekcie postępowania diagnostyczno-terapeutycznego"
                    },
                    CompletionRequirements = "Zaliczenie sprawdzianu z zakresu wiedzy objętej programem kursu"
                },
                // Kurs atestacyjny
                new CourseDefinition
                {
                    Name = "Kurs atestacyjny (podsumowujący): Medycyna morska i tropikalna",
                    Description = "Przedstawienie w sposób skondensowany niezbędnych wiadomości, obejmujących problematykę zagrożeń zdrowotnych podczas pobytu, pracy i podróży w strefach odmiennego klimatu, pracy na morzu, w portach, stoczniach i na platformach wiertniczych. Kurs obejmuje również wiedzę na temat niezbędnej diagnostyki po wystąpieniu niepokojących objawów u tych osób.",
                    DurationInHours = 40,
                    DurationInDays = 5,
                    IsRequired = true,
                    CanBeRemote = true,
                    RecommendedYear = 2,
                    SpecializationId = 2,
                    Requirements = "W ostatnim roku szkolenia specjalizacyjnego przed przystąpieniem do PES",
                    CourseTopics = new List<string>
                    {
                        "Problemy zdrowotne wśród marynarzy i rybaków morskich",
                        "Zagrożenia zdrowotne wśród pracowników portów morskich oraz stoczni",
                        "Zagrożenia zdrowotne związane z pracą na platformach wiertniczych oraz prac podwodnych",
                        "Choroby przenoszone drogą pokarmową",
                        "Tropikalne choroby wektorowe",
                        "Inwazyjne choroby układu oddechowego",
                        "Problemy kardiologiczne w klimacie gorącym",
                        "Choroby układu nerwowego spowodowane czynnikami inwazyjnymi",
                        "\"Owoce morza\" jako przyczyna chorób",
                        "Stany zagrożenia życia w tropiku oraz po powrocie z krajów endemicznego występowania niebezpiecznych patogenów",
                        "Niepokojące objawy u osób powracających z tropiku – o czym należy pamiętać"
                    },
                    CompletionRequirements = "Zaliczenie kolokwium z zakresu wiedzy objętej programem kursu"
                },
            };
        }

        public static List<InternshipDefinition> GetBasicInternships()
        {
            return new List<InternshipDefinition>
            {
                new InternshipDefinition
                {
                    Name = "Staż podstawowy w zakresie hematologii",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej w zakresie diagnostyki i leczenia chorób krwi nowotworowych i nienowotworowych.",
                    DurationInWeeks = 101,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "Oddział hematologii posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie hematologii.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Hematologia kliniczna - choroby nienowotworowe",
                            RequiredSkills = new List<string>
                            {
                                "Diagnozowanie i leczenie niedokrwistości (niedoborowych, hemolitycznych, aplastycznych i innych)",
                                "Diagnozowanie i leczenie małopłytkowości i nadpłytkowości",
                                "Diagnozowanie i leczenie granulocytopenii i agranulocytozy",
                                "Diagnozowanie i leczenie zespołów niedoborów odporności",
                                "Diagnostyka i leczenie aplazji szpiku",
                                "Postępowanie w nocnej napadowej hemoglobinurii",
                                "Postępowanie w porfiriach",
                                "Hematologia konsultacyjna"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Hematologia kliniczna - nowotwory mieloidalne",
                            RequiredSkills = new List<string>
                            {
                                "Diagnozowanie i leczenie ostrych białaczek szpikowych",
                                "Diagnozowanie i leczenie nowotworów mieloproliferacyjnych",
                                "Diagnozowanie i leczenie zespołów mielodysplastycznych",
                                "Diagnozowanie i leczenie mastocytozy",
                                "Diagnostyka i leczenie przewlekłej białaczki szpikowej"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Hematologia kliniczna - nowotwory układu chłonnego",
                            RequiredSkills = new List<string>
                            {
                                "Diagnozowanie i leczenie ostrych białaczek limfoblastycznych",
                                "Diagnozowanie i leczenie przewlekłej białaczki limfocytowej",
                                "Diagnozowanie i leczenie chłoniaków nie-Hodgkina",
                                "Diagnozowanie i leczenie chłoniaka Hodgkina",
                                "Diagnozowanie i leczenie szpiczaka plazmocytowego i innych chorób z komórek plazmatycznych",
                                "Diagnozowanie i leczenie skórnych chłoniaków T-komórkowych"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Zaburzenia hemostazy",
                            RequiredSkills = new List<string>
                            {
                                "Diagnozowanie i leczenie wrodzonych zaburzeń krzepnięcia",
                                "Diagnozowanie i leczenie nabytych zaburzeń krzepnięcia",
                                "Diagnozowanie i leczenie trombofili",
                                "Postępowanie w żylnej chorobie zakrzepowo-zatorowej",
                                "Diagnostyka i leczenie małopłytkowości",
                                "Zasady stosowania leków przeciwkrzepliwych i przeciwpłytkowych",
                                "Kontrola laboratoryjna leczenia przeciwkrzepliwego"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Leczenie wspomagające i stany nagłe",
                            RequiredSkills = new List<string>
                            {
                                "Postępowanie w gorączce neutropenicznej",
                                "Diagnostyka i leczenie powikłań infekcyjnych u chorych z zaburzeniami odporności",
                                "Postępowanie w zespole lizy guza",
                                "Postępowanie w hiperleukocytozie",
                                "Postępowanie w zespole nadlepkości",
                                "Postępowanie w zespole żyły głównej górnej",
                                "Leczenie przeciwbólowe w hematologii",
                                "Zasady żywienia dojelitowego i pozajelitowego"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Przeszczepianie komórek krwiotwórczych",
                            RequiredSkills = new List<string>
                            {
                                "Kwalifikacja chorych do przeszczepienia komórek krwiotwórczych",
                                "Przygotowanie do transplantacji komórek krwiotwórczych",
                                "Opieka nad chorym po przeszczepieniu komórek krwiotwórczych",
                                "Diagnostyka i leczenie powikłań poprzeszczepowych"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w laboratorium hematologicznym",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej na temat zastosowania różnych technik diagnostycznych stosowanych w diagnostyce chorób krwi nowotworowych i nienowotworowych.",
                    DurationInWeeks = 4,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 1,
                    Requirements = "Laboratorium posiadające akredytacje do prowadzenia ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Diagnostyka hematologiczna",
                            RequiredSkills = new List<string>
                            {
                                "Wykonanie i zabarwienie rozmazów szpiku i krwi obwodowej",
                                "Ocena rozmazu krwi obwodowej w różnych chorobach krwi",
                                "Ocena mielogramu w różnych chorobach krwi",
                                "Antykoagulanty, umiejętność przygotowania próbki osocza i surowicy do badania"
                            },
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Ocena rozmazu krwi obwodowej", 30},
                                {"Ocena mielogramu", 30}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w ośrodku przeszczepiania szpiku akredytowanym do wykonywania przeszczepień allogenicznych",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej na temat przeszczepień krwiotwórczych komórek macierzystych, zasad opieki nad pacjentem oraz profilaktyki i leczenia powikłań.",
                    DurationInWeeks = 4,
                    IsRequired = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    Requirements = "Oddział hematologii wykonujący przeszczepienia allogenicznych komórek krwiotwórczych posiadający akredytacje do prowadzenia szkolenia specjalizacyjnego w dziedzinie hematologii lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Przeszczepianie komórek krwiotwórczych",
                            RequiredSkills = new List<string>
                            {
                                "Interpretacja badań HLA w doborze dawcy i biorcy komórek krwiotwórczych",
                                "Stosowanie zasad odwrotnej izolacji chorych",
                                "Pobieranie szpiku do przeszczepienia",
                                "Przeszczepienie autologicznych komórek krwiotwórczych",
                                "Przeszczepienie allogenicznych komórek krwiotwórczych",
                                "Umiejętność doboru antybiotyków u chorych po przeszczepieniu komórek krwiotwórczych",
                                "Umiejętność prowadzenia monitorowania i leczenia wyprzedzającego (preemptive therapy) w zakażeniach wirusem cytomegalii",
                                "Umiejętność prowadzenia chorego z przewlekłą chorobą przeszczep przeciw gospodarzowi"
                            },
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Przeszczepienie autologicznych komórek krwiotwórczych", 2},
                                {"Przeszczepienie allogenicznych komórek krwiotwórczych", 1}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w hematologicznym oddziale dziennego leczenia",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej w zakresie zasad leczenia pacjentów z chorobami krwi w oddziale leczenia dziennego.",
                    DurationInWeeks = 5,
                    IsRequired = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    Requirements = "Oddział dzienny hematologii posiadający akredytację do prowadzenia ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Leczenie w trybie dziennym",
                            RequiredSkills = new List<string>
                            {
                                "Umiejętność zaplanowania leczenia systemowego u chorych leczonych w oddziale dziennym",
                                "Umiejętność zaplanowania obserwacji chorych po zakończeniu leczenia cytostatycznego",
                                "Umiejętność wykonania upustu krwi u chorego na czerwienicę prawdziwą"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w poradni hematologicznej",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej w zakresie zasad leczenia pacjentów z chorobami krwi w poradni hematologicznej.",
                    DurationInWeeks = 8,
                    IsRequired = true,
                    RecommendedYear = 2,
                    SpecializationId = 1,
                    Requirements = "Poradnia hematologii posiadająca akredytację do prowadzenia ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Leczenie ambulatoryjne",
                            RequiredSkills = new List<string>
                            {
                                "Badanie podmiotowe i przedmiotowe wykonywane pod kątem rozpoznawania chorób krwi",
                                "Interpretacja wyników badań morfologii krwi, badań biochemicznych krwi i badań obrazowych",
                                "Interpretacja mielogramu i trepannobiopsji",
                                "Planowanie i prowadzenie leczenia chorych na niedokrwistości hemolityczne",
                                "Planowanie i prowadzenie leczenia chorych na niedokrwistości hipoplastyczne",
                                "Planowanie obserwacji i leczenie pacjentów z małopłytkowością lub granulocytopenią",
                                "Planowanie i prowadzenie leczenia u chorych na nowotwory mieloproliferacyjne",
                                "Planowanie obserwacji chorych na nowotwory limfoproliferacyjne pod kątem właściwego ustalenia wskazań do leczenia",
                                "Zasady obserwacji ambulatoryjnej pacjentów po zakończeniu chemioterapii/immunochemioterapii i po przeszczepieniach komórek krwiotwórczych",
                                "Prowadzenie leczenia przeciwkrzepliwego w warunkach ambulatoryjnych"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w Regionalnym Centrum Krwiodawstwa i Krwiolecznictwa",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej w zakresie zasad krwiodawstwa i krwiolecznictwa ze szczególnym uwzględnieniem chorób krwi.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 3,
                    SpecializationId = 1,
                    Requirements = "Regionalne Centrum Krwiodawstwa i Krwiolecznictwa posiadające akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie transfuzjologii klinicznej lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Krwiodawstwo i krwiolecznictwo",
                            RequiredSkills = new List<string>
                            {
                                "Dobieranie krwinek czerwonych w celu uniknięcia alloimmunizacji oraz postępowanie u chorych alloimmunizowanych",
                                "Dobieranie płytek krwi do przetoczenia, unikanie oporności odpornościowej na przetaczanie płytek oraz postępowanie u chorych immunizowanych",
                                "Zasady przetaczania preparatów krwi u chorych po przeszczepieniu komórek krwiotwórczych oraz allo- i autoimmunizacja u tych chorych",
                                "Zbadanie grup krwi i dobranie krwi do przetoczenia"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w ośrodku leczenia hemofilii i pokrewnych skaz krwotocznych",
                    Description = "Nabycie przez lekarzy wiedzy teoretycznej i praktycznej w zakresie zasad leczenia chorych na hemofilie i pokrewne skazy krwotoczne.",
                    DurationInWeeks = 3,
                    IsRequired = true,
                    RecommendedYear = 3,
                    SpecializationId = 1,
                    Requirements = "Oddział hematologii zajmujący się leczeniem hemofilii lub prowadzący ośrodek leczenia hemofilii, posiadający akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie hematologii lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Leczenie hemofilii i pokrewnych skaz krwotocznych",
                            RequiredSkills = new List<string>
                            {
                                "Wskazania do podania i dawkowanie koncentratów czynników krzepnięcia oraz innych leków wpływających na hemostazę w różnych wrodzonych osoczowych skazach krwotocznych",
                                "Postępowanie u chorych na nabytą hemofilię",
                                "Zabezpieczenie małych i dużych zabiegów chirurgicznych u chorych na hemofilię o różnym stopniu ciężkości",
                                "Postępowanie w stanach zagrożenia życia u chorych na osoczowe skazy krwotoczne"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych - potwierdzenie wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż podstawowy
                new InternshipDefinition
                {
                    Name = "Staż podstawowy w zakresie medycyny morskiej i tropikalnej",
                    Description = "Umożliwienie osobie realizującej program specjalizacji czynnego uczestnictwa w pracach zespołu medycznego, zatrudnionego w akredytowanej/wybranej placówce prowadzącej działalność orzeczniczą, diagnostyczną i leczniczą oraz wykonującą badania przed wyjazdem i po powrocie ze stref klimatu gorącego.",
                    DurationInWeeks = 59,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Uniwersyteckie Centrum Medycyny Morskiej i Tropikalnej w Gdyni lub inna jednostka, która posiada akredytację do prowadzenia specjalizacji w dziedzinie medycyny morskiej i tropikalnej.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Opieka medyczna i orzecznictwo w medycynie morskiej i tropikalnej",
                            RequiredSkills = new List<string>
                            {
                                "Uczestniczenie w ocenie stanu zdrowia i orzekaniu o przydatności do pracy w tropikach",
                                "Uczestniczenie w ocenie stanu zdrowia i orzekaniu o przydatności do pracy na morzu",
                                "Prowadzenie dokumentacji medycznej",
                                "Uczestniczenie w ocenie stanu zdrowia i orzekaniu o przydatności do pracy w warunkach hiperbarii",
                                "Znajomość zasad współpracy z morską służbą ratownictwa i poszukiwań SAR",
                                "Poradnictwo profilaktyczne przed wyjazdem do klimatu gorącego",
                                "Uczestniczenie w badaniach i konsultacjach osób powracających z pobytu w strefie klimatu gorącego",
                                "Uczestniczenie w konsultacjach osób z podejrzeniem chorób pasożytniczych i innych inwazyjnych"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu kierunkowego",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika specjalizacji wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie medycyny morskiej
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie medycyny morskiej",
                    Description = "Umożliwienie lekarzowi odbywającemu specjalizacje na bezpośrednie uczestnictwo w procesach orzecznictwa o przydatności do pracy na morzu, orzekaniu o skutkach wypadków w pracy, procesach diagnostyczno-terapeutycznych w czasie hospitalizacji osób tego wymagających.",
                    DurationInWeeks = 3,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Klinika Chorób Wewnętrznych i Zawodowych oraz Przychodnia z Poradnią Medycyny Pracy Uniwersyteckiego Centrum Medycyny Morskiej i Tropikalnej w Gdyni lub inna jednostka, która posiada akredytację do prowadzenia ww. stażu prowadząca poradnię medycyny pracy na rzecz stoczni, portów, marynarzy i rybaków pełnomorskich.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Medycyna morska teoretyczna",
                            RequiredSkills = new List<string>
                            {
                                "Znajomość morskiego środowiska pracy i konwencji morskich",
                                "Znajomość wymagań zdrowotnych od kandydatów i osób pracujących na morzu",
                                "Znajomość zagrożeń zdrowotnych, zjawisk chorobowych i wypadkowych podczas pracy na statku",
                                "Znajomość zasad udzielania pomocy przedlekarskiej i lekarskiej osobom pracującym na statku, udzielania pomocy drogą radiową, zasad ewakuacji i repatriacji"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Medycyna morska praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Ocena stanu zdrowia kandydatów i pracowników, wydawanie orzeczenia lekarskiego o zdolności do pracy na statkach morskich", 30},
                                {"Ocena następstw chorób i wypadków na zachowanie zdolności do pracy na morzu", 10},
                                {"Prowadzenie dokumentacji medycznej z udzielania pomocy drogą radiową", 20}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie medycyny morskiej w Granicznej Stacji Sanitarno-Epidemiologicznej
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie medycyny morskiej w Granicznej Stacji Sanitarno-Epidemiologicznej",
                    Description = "Możliwość bezpośredniego uczestniczenia w kontroli sanitarnej statków, przewożonych produktów oraz zasad współpracy z Strażą Graniczną w portach morskich.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Graniczna Stacja Sanitarno-Epidemiologiczna obejmująca swoim zasięgiem morskie przejście graniczne.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Nadzór sanitarny w porcie morskim",
                            RequiredSkills = new List<string>
                            {
                                "Znajomość ustaw i rozporządzeń dotyczących bezpieczeństwa pracy i zdrowia w środowisku morskim i portach morskich oraz w zakresie ratownictwa morskiego",
                                "Znajomość wymagań sanitarnych dotycząca statków morskich i ładunków"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Praktyka w GSSE",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Prowadzenie sanitarnej kontroli statku pod nadzorem Inspektora GSSE", 3},
                                {"Znajomość zasad pracy portu i służb portowych", 3},
                                {"Znajomość zasad współpracy z morską służbą ratownictwa i poszukiwań SAR", 3}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie medycyny tropikalnej
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie medycyny tropikalnej",
                    Description = "Zdobycie umiejętności pobierania krwi do wykonywania rozmazów i wymazów do badań mikrobiologicznych, parazytologicznych oraz serologicznych oraz zdobycie umiejętności ich interpretacji.",
                    DurationInWeeks = 7,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Jednostka, która posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie medycyny morskiej i tropikalnej lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Medycyna tropikalna teoretyczna",
                            RequiredSkills = new List<string>
                            {
                                "Znajomość sytuacji epidemiologicznej chorób zakaźnych, pasożytniczych i tropikalnych na świecie",
                                "Współczesne zagrożenia epidemiologiczne",
                                "Znajomość zasad wykrywania zakażeń i zarażeń – badania mikrobiologiczne, serologiczne, diagnostyka molekularna",
                                "Znajomość zasad pobierania, przechowywania i transportu materiału biologicznego",
                                "Znajomość etiologii, obrazu klinicznego, diagnostyki oraz zasad leczenia chorób tropikalnych i pasożytniczych",
                                "Znajomość zasad postępowania profilaktycznego w stosunku do osób wyjeżdżających i powracających z tropiku",
                                "Znajomość regulacji prawnych dotyczących zapobiegania i zwalczania chorób zakaźnych, tropikalnych i pasożytniczych",
                                "Znajomość zasad orzecznictwa o przydatności zdrowotnej do pracy w tropiku i po powrocie do kraju"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Medycyna tropikalna praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Umiejętność pobierania krwi oraz wykonywania wymazów i rozmazów do badań mikrobiologicznych, parazytologicznych oraz serologicznych", 20},
                                {"Umiejętność interpretacji wyników tych badań", 20},
                                {"Umiejętność oceny stanu zdrowia osób udających się do krajów klimatu gorącego oraz powracających, wydawanie orzeczeń lekarskich", 40},
                                {"Umiejętność kwalifikowania do profilaktyki poekspozycyjnej – tężec, wścieklizna, HBV, HCV, HIV", 10}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie chorób zakaźnych
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie chorób zakaźnych",
                    Description = "Umożliwienie bezpośredniego udziału lekarza specjalizującego się w procesach diagnostyczno-terapeutycznych chorób zakaźnych, z którymi może mieć do czynienia w swojej praktyce zawodowej.",
                    DurationInWeeks = 2,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Oddział chorób zakaźnych, który posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie chorób zakaźnych lub ww. stażu oraz poradnia będąca w strukturze oddziału/jednostki.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Choroby zakaźne teoretyczne",
                            RequiredSkills = new List<string>
                            {
                                "Sytuacja epidemiologiczna chorób zakaźnych w Polsce i na świecie, współczesne zagrożenia epidemiologiczne",
                                "Metody wykrywania zakażeń (badania mikrobiologiczne, serologiczne, próby śródskórne, diagnostyka molekularna)",
                                "Zasady pobierania, przechowywania i transport materiału biologicznego",
                                "Leczenie etiotropowe chorób zakaźnych – antybiotyki, chemioterapeutyki, surowice",
                                "Zakażenia bakteriami Gram(+) i Gram(-)",
                                "Gruźlica, mikobakteriozy, riketsjozy, chlamdiozy, krętkowice",
                                "Grzybice głębokie",
                                "Zakażenia wirusowe, w tym HIV, AIDS",
                                "Wirusowe zapalenie wątroby",
                                "Zoonozy",
                                "Zespoły pozakaźne"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Choroby zakaźne praktyczne",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Pobieranie krwi oraz innych płynów ustrojowych oraz wymazów do badań mikrobiologicznych", 10},
                                {"Kwalifikacja do profilaktyki poekspozycyjnej (tężec, wścieklizna, HBV, HCV, HIV)", 10}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z zakresu wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie medycyny nurkowej z elementami medycyny hiperbarycznej
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie medycyny nurkowej z elementami medycyny hiperbarycznej",
                    Description = "Umożliwienie lekarzowi specjalizującemu się na uczestnictwie w orzekaniu o przydatności do pracy w charakterze nurka, badaniach okresowych oraz sesjach terapii hiperbarycznej.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Krajowy Ośrodek Medycyny Hiperbarycznej Instytutu Medycyny Morskiej i Tropikalnej GUMed w Gdyni, który posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie medycyny morskiej i tropikalnej lub inna jednostka, która uzyska akredytację do prowadzenia ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Medycyna hiperbaryczna teoretyczna",
                            RequiredSkills = new List<string>
                            {
                                "Wpływ ciśnienia na organizm człowieka, działanie gazów pod zwiększonym ciśnieniem, narkoza azotowa, toksyczność tlenowa",
                                "Urazy ciśnieniowe – objawy, leczenie, profilaktyka",
                                "Choroba dekompresyjna – objawy kliniczne, diagnostyka, leczenie",
                                "Tabele dekompresyjne",
                                "Dysbaryczna martwica kości – epidemiologia, diagnostyka, postępowanie lecznicze",
                                "Rekompresja lecznicza – tabele lecznicze, powikłania, bezpieczeństwo",
                                "Wskazania do terapii tlenem hiperbarycznym, przeciwwskazania względne i bezwzględne, powikłania"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Medycyna hiperbaryczna praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Rozpoznawanie urazów ciśnieniowych", 5},
                                {"Rozpoznawanie choroby dekompresyjnej", 5},
                                {"Posługiwanie się tabelą dekompresyjną", 5},
                                {"Postępowanie lekarskie w wypadku nurkowym", 5},
                                {"Udział w przeprowadzeniu sesji hiperbarycznych (bez konieczności pobytu pod ciśnieniem)", 10}
                            },
                            AssistantProcedures = new Dictionary<string, int>
                            {
                                {"Udział w procedurach orzekania o przydatności do pracy w charakterze nurka lub innych osób pracujących w podwyższonym ciśnieniem otoczenia", 15}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie ortopedii i traumatologii
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie ortopedii i traumatologii",
                    Description = "Umożliwienie lekarzowi realizującemu program specjalizacji uczestniczenie w udzielaniu pierwszej pomocy medycznej w zdarzeniach ortopedycznych i innych urazach.",
                    DurationInWeeks = 2,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Poradnia i/lub oddział ortopedyczno-chirurgiczny w jednostce, która posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie ortopedii i traumatologii narządu ruchu lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Ortopedia i traumatologia",
                            RequiredSkills = new List<string>
                            {
                                "Zdobywa wiedzę o możliwych skutkach wypadków/urazów podczas pracy na morzu, platformach wiertniczych czy komunikacyjnych",
                                "Zdobywa wiedzę odnośnie niezbędnych badań diagnostycznych w tych przypadkach",
                                "Asystuje podczas małych zabiegów ortopedycznych, szyciu ran, zakładaniu opatrunków gipsowych",
                                "Zdobywa umiejętność wykonania prostych zabiegów"
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie neurologii
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie neurologii",
                    Description = "Umożliwienie osobie specjalizującej się udziału w postępowaniu diagnostyczno-terapeutycznym u pacjentów ze schorzeniami neurologicznymi.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Poradnia i/lub oddział neurologii w jednostce, która posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie neurologii lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Neurologia",
                            RequiredSkills = new List<string>
                            {
                                "Umiejętność przeprowadzenia badania neurologicznego",
                                "Umiejętność wykonania wybranych zabiegów znieczulania miejscowego oraz nakłucia lędźwiowego pod kontrolą",
                                "Znajomość objawów klinicznych w chorobach układu nerwowego",
                                "Znajomość potencjalnych powikłań po urazach głowy oraz udarach mózgu",
                                "Znajomość czynników biologicznych mogących być przyczyną chorób układu nerwowego",
                                "Znajomość niezbędnych badań diagnostycznych w chorobach układu nerwowego"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Procedury neurologiczne",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Wykonanie badania neurologicznego", 20}
                            },
                            AssistantProcedures = new Dictionary<string, int>
                            {
                                {"Wykonanie nakłucia lędźwiowego", 3}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie psychologii pracy
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie psychologii pracy",
                    Description = "Umożliwienie lekarzowi realizującemu program tej specjalizacji bezpośredni udział w badaniach psychologicznych, kwalifikujących do określonych prac i zawodów oraz w badaniach orzeczniczych.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Pracownia lub poradnia psychologii pracy. Możliwość odbycia stażu w Poradni Psychologii Pracy UCMMiT w Gdyni.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Psychologia pracy",
                            RequiredSkills = new List<string>
                            {
                                "Podstawowych wiadomości odnośnie stresu związanego z wykonywaniem określonego zawodu",
                                "Znajomości objawów świadczących o problemach natury psychologiczno-psychiatrycznej, szczególnie podczas długich rejsów czy pracy w trudnych warunkach klimatu gorącego"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Procedury psychologiczne",
                            AssistantProcedures = new Dictionary<string, int>
                            {
                                {"Udział w procedurach badań psychologicznych i psychotechnicznych", 10}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie dermatologii i wenerologii
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie dermatologii i wenerologii",
                    Description = "Uzyskanie podstawowych wiadomości w zakresie metod diagnostycznych i zasad leczenia wybranych chorób skóry oraz chorób przenoszonych drogą płciową. Uzyskanie podstawowych umiejętności pobieraniu materiału (wymazów, zeskrobin) z miejsca zmian chorobowych do badań diagnostycznych.",
                    DurationInWeeks = 2,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Poradnia i/lub oddział dermatologiczno-wenerologiczny w jednostce, która posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie dermatologii i wenerologii lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Dermatologia i wenerologia teoretyczna",
                            RequiredSkills = new List<string>
                            {
                                "Choroby skóry o etiologii zakaźnej (bakteryjne, wirusowe, grzybicze, pasożytnicze)",
                                "Objawy skórne w chorobach zakaźnych i pasożytniczych",
                                "Nowotwory skóry",
                                "Kiła – obraz kliniczny, diagnostyka, leczenie",
                                "Rzeżączka – obraz kliniczny, diagnostyka, leczenie",
                                "Nierzeżączkowe stany zapalne dolnych odcinków układu moczowo – płciowego"
                            }
                        },
                        new InternshipModule
                        {
                            Name = "Dermatologia i wenerologia praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Pobieranie wycinków/skrawków skóry do badań mikroskopowych oraz PCR", 5},
                                {"Wykonywanie testów naskórkowych", 10}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie okulistyki
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie okulistyki",
                    Description = "Uzyskanie podstawowych umiejętności w udzielaniu pierwszej pomocy medycznej w nagłych przypadkach chorób i urazów narządu wzroku.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Poradnia i/lub oddział okulistyczny w jednostce, która posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie okulistyki lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Okulistyka praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Określanie ostrości wzroku do dali i bliży bez korekcji i z korekcją", 10},
                                {"Ocenianie zdolności rozpoznawania barw", 10},
                                {"Ocenianie pola widzenia metodą orientacyjną i za pomocą perymetru", 5},
                                {"Usuwanie ciała obcego z oka", 5}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
                // Staż kierunkowy w zakresie otolaryngologii
                new InternshipDefinition
                {
                    Name = "Staż kierunkowy w zakresie otolaryngologii",
                    Description = "Uzyskanie podstawowych umiejętności w udzielaniu pierwszej pomocy medycznej w nagłych przypadkach otolaryngologicznych.",
                    DurationInWeeks = 1,
                    IsRequired = true,
                    RecommendedYear = 1,
                    SpecializationId = 2,
                    Requirements = "Oddział otorynolaryngologii, który posiada akredytację do prowadzenia szkolenia specjalizacyjnego w dziedzinie otorynolaryngologii lub ww. stażu.",
                    DetailedStructure = new List<InternshipModule>
                    {
                        new InternshipModule
                        {
                            Name = "Otolaryngologia praktyczna",
                            RequiredProcedures = new Dictionary<string, int>
                            {
                                {"Badanie ostrości słuchu szeptem i mową", 10},
                                {"Ocenianie badania audiometrycznego słuchu", 5},
                                {"Usuwanie ciała obcego/woskowiny z ucha", 3},
                                {"Badanie czynności narządu równowagi przy pomocy obserwacji oczopląsu samoistnego i w okularach Frenzla", 5}
                            }
                        }
                    },
                    CompletionRequirements = new List<string>
                    {
                        "Złożenie kolokwium z wiedzy teoretycznej objętej programem stażu",
                        "Zaliczenie sprawdzianu z umiejętności praktycznych – potwierdzenie przez kierownika stażu wykonanych przez lekarza zabiegów lub procedur medycznych objętych programem stażu"
                    }
                },
            };
        }

        public static Dictionary<string, List<RequiredProcedure>> GetRequiredProcedures()
        {
            return new Dictionary<string, List<RequiredProcedure>>
            {
                { // SpecializationId = 1
                    "Diagnostyka hematologiczna", new List<RequiredProcedure>
                    {
                        new RequiredProcedure
                        {
                            Name = "Biopsja szpiku z mostka",
                            RequiredCount = 3,
                            AssistanceCount = 3
                        },
                        new RequiredProcedure
                        {
                            Name = "Biopsja szpiku z kolca tylnego talerza biodrowego",
                            RequiredCount = 20,
                            AssistanceCount = 5
                        },
                        new RequiredProcedure
                        {
                            Name = "Biopsja szpiku z kolca przedniego talerza biodrowego",
                            RequiredCount = 3,
                            AssistanceCount = 3
                        },
                        new RequiredProcedure
                        {
                            Name = "Trepanobiopsja szpiku",
                            RequiredCount = 10,
                            AssistanceCount = 5
                        },
                        new RequiredProcedure
                        {
                            Name = "Biopsja cienkoigłowa węzła lub biopsja węzła pod kontrolą USG/TK",
                            RequiredCount = 0,
                            AssistanceCount = 3
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocena rozmazu krwi obwodowej",
                            RequiredCount = 30,
                            AssistanceCount = 30
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocena mielogramu",
                            RequiredCount = 30,
                            AssistanceCount = 30
                        }
                    }
                },
                { // SpecializationId = 1
                    "Leczenie nowotworów krwi", new List<RequiredProcedure>
                    {
                        new RequiredProcedure
                        {
                            Name = "Prowadzenie intensywnego leczenia chorych na ostre białaczki szpikowe lub ostre białaczki limfoblastyczne",
                            RequiredCount = 7,
                            AssistanceCount = 0
                        },
                        new RequiredProcedure
                        {
                            Name = "Prowadzenie leczenia indukującego chorych na chłoniaki agresywne",
                            RequiredCount = 4,
                            AssistanceCount = 0
                        },
                        new RequiredProcedure
                        {
                            Name = "Punkcja lędźwiowa i dokanałowe podanie cytostatyków",
                            RequiredCount = 0,
                            AssistanceCount = 10
                        }
                    }
                },
                { // SpecializationId = 1
                    "Zabiegi specjalistyczne", new List<RequiredProcedure>
                    {
                        new RequiredProcedure
                        {
                            Name = "Zakładanie centralnego cewnika żylnego",
                            RequiredCount = 0,
                            AssistanceCount = 3
                        },
                        new RequiredProcedure
                        {
                            Name = "Afereza lecznicza",
                            RequiredCount = 0,
                            AssistanceCount = 1
                        },
                        new RequiredProcedure
                        {
                            Name = "Przeszczepienie autologicznych komórek krwiotwórczych",
                            RequiredCount = 0,
                            AssistanceCount = 2
                        },
                        new RequiredProcedure
                        {
                            Name = "Przeszczepienie allogenicznych komórek krwiotwórczych",
                            RequiredCount = 0,
                            AssistanceCount = 1
                        }
                    }
                },
                // Podczas stażu podstawowego w zakresie medycyny morskiej i tropikalnej
                { // SpecializationId = 2
                    "Staż podstawowy",
                    new List<RequiredProcedure>
                    {
                        new RequiredProcedure
                        {
                            Name = "Uczestniczenie w ocenie stanu zdrowia kandydatów i pracowników, wydawanie orzeczenia lekarskiego o zdolności do pracy na statkach morskich",
                            RequiredCount = 0,
                            AssistanceCount = 20,
                            Description = "Uczestniczenie w badaniach, ocenach oraz wydawaniu orzeczeń"
                        },
                        new RequiredProcedure
                        {
                            Name = "Prowadzenie dokumentacji medycznej",
                            RequiredCount = 0,
                            AssistanceCount = 10,
                            Description = "Uczestniczenie w prowadzeniu dokumentacji medycznej"
                        },
                        new RequiredProcedure
                        {
                            Name = "Znajomość zasad współpracy z morską służbą ratownictwa i poszukiwań SAR",
                            RequiredCount = 0,
                            AssistanceCount = 3,
                            Description = "Zapoznanie się z zasadami współpracy z morską służbą ratownictwa i poszukiwań SAR"
                        },
                        new RequiredProcedure
                        {
                            Name = "Uczestniczenie w ocenie stanu zdrowia osób udających się do krajów klimatu gorącego oraz powracających, wydawanie orzeczeń lekarskich",
                            RequiredCount = 0,
                            AssistanceCount = 20,
                            Description = "Uczestniczenie w badaniach, ocenach oraz wydawaniu orzeczeń"
                        },
                        new RequiredProcedure
                        {
                            Name = "Uczestniczenie w procedurach orzekania o przydatności do pracy w charakterze nurka lub innych osób pracujących w podwyższonym ciśnieniu otoczenia",
                            RequiredCount = 0,
                            AssistanceCount = 5,
                            Description = "Uczestniczenie w badaniach, ocenach oraz wydawaniu orzeczeń"
                        }
                    }
                },
                // Podczas staży kierunkowych
                { // SpecializationId = 2
                    "Staże kierunkowe",
                    new List<RequiredProcedure>
                    {
                        // Medycyna morska
                        new RequiredProcedure
                        {
                            Name = "Ocena stanu zdrowia kandydatów i pracowników, wydawanie orzeczenia lekarskiego o zdolności do pracy na statkach morskich",
                            RequiredCount = 30,
                            AssistanceCount = 0,
                            Description = "Samodzielne badanie, ocena oraz wydawanie orzeczeń"
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocena następstw chorób i wypadków na zachowanie zdolności do pracy na morzu",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielna ocena następstw chorób i wypadków"
                        },
                        new RequiredProcedure
                        {
                            Name = "Prowadzenie dokumentacji medycznej z udzielaniem pomocy drogą radiową",
                            RequiredCount = 20,
                            AssistanceCount = 0,
                            Description = "Samodzielne prowadzenie dokumentacji medycznej"
                        },
                        // Medycyna morska w GSSE
                        new RequiredProcedure
                        {
                            Name = "Prowadzenie sanitarnej kontroli statku pod nadzorem Inspektora GSSE",
                            RequiredCount = 3,
                            AssistanceCount = 0,
                            Description = "Samodzielne prowadzenie kontroli sanitarnej statku"
                        },
                        new RequiredProcedure
                        {
                            Name = "Znajomość zasad pracy portu i służb portowych",
                            RequiredCount = 3,
                            AssistanceCount = 0,
                            Description = "Zapoznanie się z zasadami pracy portu i służb portowych"
                        },
                        new RequiredProcedure
                        {
                            Name = "Znajomość zasad współpracy z morską służbą ratownictwa i poszukiwań SAR",
                            RequiredCount = 3,
                            AssistanceCount = 0,
                            Description = "Zapoznanie się z zasadami współpracy z morską służbą ratownictwa i poszukiwań SAR"
                        },
                        // Medycyna tropikalna
                        new RequiredProcedure
                        {
                            Name = "Umiejętność pobierania krwi oraz wykonywania wymazów i rozmazów do badań mikrobiologicznych, parazytologicznych oraz serologicznych",
                            RequiredCount = 20,
                            AssistanceCount = 0,
                            Description = "Samodzielne pobieranie materiału biologicznego"
                        },
                        new RequiredProcedure
                        {
                            Name = "Umiejętność interpretacji wyników tych badań",
                            RequiredCount = 20,
                            AssistanceCount = 0,
                            Description = "Samodzielna interpretacja wyników badań"
                        },
                        new RequiredProcedure
                        {
                            Name = "Umiejętność oceny stanu zdrowia osób udających się do krajów klimatu gorącego oraz powracających, wydawanie orzeczeń lekarskich",
                            RequiredCount = 40,
                            AssistanceCount = 0,
                            Description = "Samodzielna ocena stanu zdrowia i wydawanie orzeczeń"
                        },
                        // Choroby zakaźne
                        new RequiredProcedure
                        {
                            Name = "Pobieranie krwi oraz innych płynów ustrojowych oraz wymazów do badań mikrobiologicznych",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielne pobieranie materiału biologicznego"
                        },
                        new RequiredProcedure
                        {
                            Name = "Kwalifikacja do profilaktyki poekspozycyjnej (tężec, wścieklizna, HBV, HCV, HIV)",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielna kwalifikacja do profilaktyki poekspozycyjnej"
                        },
                        // Medycyna nurkowa z elementami medycyny hiperbarycznej
                        new RequiredProcedure
                        {
                            Name = "Udział w przeprowadzaniu sesji hiperbarycznych (bez konieczności pobytu pod ciśnieniem)",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielny udział w przeprowadzaniu sesji hiperbarycznych"
                        },
                        new RequiredProcedure
                        {
                            Name = "Udział w procedurach orzekania o przydatności do pracy w charakterze nurka lub innych osób pracujących w podwyższonym ciśnieniu otoczenia",
                            RequiredCount = 0,
                            AssistanceCount = 15,
                            Description = "Uczestniczenie w procedurach orzekania"
                        },
                        // Neurologia
                        new RequiredProcedure
                        {
                            Name = "Wykonanie badania neurologicznego",
                            RequiredCount = 20,
                            AssistanceCount = 0,
                            Description = "Samodzielne wykonanie badania neurologicznego"
                        },
                        new RequiredProcedure
                        {
                            Name = "Wykonanie nakłucia lędźwiowego",
                            RequiredCount = 0,
                            AssistanceCount = 3,
                            Description = "Uczestniczenie w wykonaniu nakłucia lędźwiowego"
                        },
                        // Psychologia pracy
                        new RequiredProcedure
                        {
                            Name = "Udział w procedurach badań psychologicznych i psychotechnicznych",
                            RequiredCount = 0,
                            AssistanceCount = 10,
                            Description = "Uczestniczenie w procedurach badań psychologicznych i psychotechnicznych"
                        },
                        // Dermatologia i wenerologia
                        new RequiredProcedure
                        {
                            Name = "Pobieranie wycinków/skrawków skóry do badań mikroskopowych oraz PCR",
                            RequiredCount = 5,
                            AssistanceCount = 0,
                            Description = "Samodzielne pobieranie wycinków/skrawków skóry"
                        },
                        new RequiredProcedure
                        {
                            Name = "Wykonywanie testów naskórkowych",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielne wykonywanie testów naskórkowych"
                        },
                        // Okulistyka
                        new RequiredProcedure
                        {
                            Name = "Określanie ostrości wzroku do dali i bliży bez korekcji i z korekcją",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielne określanie ostrości wzroku"
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocenianie zdolności rozpoznawania barw",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielne ocenianie zdolności rozpoznawania barw"
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocenianie pola widzenia metodą orientacyjną i za pomocą perymetru",
                            RequiredCount = 5,
                            AssistanceCount = 0,
                            Description = "Samodzielne ocenianie pola widzenia"
                        },
                        new RequiredProcedure
                        {
                            Name = "Usuwanie ciała obcego z oka",
                            RequiredCount = 5,
                            AssistanceCount = 0,
                            Description = "Samodzielne usuwanie ciała obcego z oka"
                        },
                        // Otolaryngologia
                        new RequiredProcedure
                        {
                            Name = "Badanie ostrości słuchu szeptem i mową",
                            RequiredCount = 10,
                            AssistanceCount = 0,
                            Description = "Samodzielne badanie ostrości słuchu"
                        },
                        new RequiredProcedure
                        {
                            Name = "Ocenianie badania audiometrycznego słuchu",
                            RequiredCount = 5,
                            AssistanceCount = 0,
                            Description = "Samodzielne ocenianie badania audiometrycznego słuchu"
                        },
                        new RequiredProcedure
                        {
                            Name = "Usuwanie ciała obcego/woskowiny z ucha",
                            RequiredCount = 3,
                            AssistanceCount = 0,
                            Description = "Samodzielne usuwanie ciała obcego/woskowiny z ucha"
                        },
                        new RequiredProcedure
                        {
                            Name = "Badanie czynności narządu równowagi przy pomocy obserwacji oczopląsu samoistnego i w okularach Frenzla",
                            RequiredCount = 5,
                            AssistanceCount = 0,
                            Description = "Samodzielne badanie czynności narządu równowagi"
                        }
                    }
                },
            };
        }
    }

    public static class ProcedureMonitoring
    {
        public class ProcedureProgress
        {
            public string ProcedureName { get; set; }
            public int CompletedCount { get; set; }
            public int AssistanceCount { get; set; }
            public int SimulationCount { get; set; }
            public List<ProcedureExecution> Executions { get; set; } = new List<ProcedureExecution>();
        }

        public class ProcedureExecution
        {
            public DateTime ExecutionDate { get; set; }
            public string Type { get; set; } // "Wykonanie", "Asysta", "Symulacja"
            public string SupervisorName { get; set; }
            public string Location { get; set; }
            public string PatientId { get; set; } // Zanonimizowany identyfikator
            public string Notes { get; set; }
        }

        public class ProgressVerification
        {
            public (bool IsComplete, List<string> Deficiencies) VerifyProgress(
                Dictionary<string, ProcedureProgress> completedProcedures)
            {
                var requirements = GetRequiredProcedures();
                var deficiencies = new List<string>();

                foreach (var category in requirements)
                {
                    foreach (var requiredProcedure in category.Value)
                    {
                        if (!completedProcedures.TryGetValue(requiredProcedure.Name, out var progress))
                        {
                            deficiencies.Add($"Brak wykonanych procedur: {requiredProcedure.Name}");
                            continue;
                        }

                        // Sprawdzenie liczby wykonań
                        if (progress.CompletedCount < requiredProcedure.RequiredCount)
                        {
                            deficiencies.Add($"Niewystarczająca liczba wykonań {requiredProcedure.Name}: " +
                                             $"wykonano {progress.CompletedCount}/{requiredProcedure.RequiredCount}");
                        }

                        // Sprawdzenie liczby asyst
                        if (progress.AssistanceCount < requiredProcedure.AssistanceCount)
                        {
                            deficiencies.Add($"Niewystarczająca liczba asyst {requiredProcedure.Name}: " +
                                             $"wykonano {progress.AssistanceCount}/{requiredProcedure.AssistanceCount}");
                        }

                        // Weryfikacja limitu symulacji
                        if (requiredProcedure.AllowSimulation)
                        {
                            var maxSimulations = (requiredProcedure.RequiredCount * requiredProcedure.SimulationLimit.Value) / 100;
                            if (progress.SimulationCount > maxSimulations)
                            {
                                deficiencies.Add($"Przekroczony limit symulacji dla {requiredProcedure.Name}: " +
                                                 $"wykonano {progress.SimulationCount}, " +
                                                 $"maksymalnie dozwolone {maxSimulations}");
                            }
                        }
                    }
                }

                return (deficiencies.Count == 0, deficiencies);
            }

            public ProgressSummary GenerateProgressSummary(Dictionary<string, ProcedureProgress> completedProcedures)
            {
                var requirements = GetRequiredProcedures();
                var summary = new ProgressSummary();

                foreach (var category in requirements)
                {
                    var categorySummary = new CategorySummary
                    {
                        CategoryName = category.Key,
                        TotalRequired = category.Value.Sum(p => p.RequiredCount),
                        TotalAssistanceRequired = category.Value.Sum(p => p.AssistanceCount),
                        CompletedCount = 0,
                        AssistanceCount = 0,
                        SimulationCount = 0,
                        CompletionPercentage = 0
                    };

                    foreach (var procedure in category.Value)
                    {
                        if (completedProcedures.TryGetValue(procedure.Name, out var progress))
                        {
                            categorySummary.CompletedCount += progress.CompletedCount;
                            categorySummary.AssistanceCount += progress.AssistanceCount;
                            categorySummary.SimulationCount += progress.SimulationCount;
                        }
                    }

                    var totalRequired = categorySummary.TotalRequired + categorySummary.TotalAssistanceRequired;
                    var totalCompleted = categorySummary.CompletedCount + categorySummary.AssistanceCount;

                    categorySummary.CompletionPercentage = totalRequired > 0
                        ? (totalCompleted * 100.0) / totalRequired
                        : 0;

                    summary.Categories.Add(categorySummary);
                }

                summary.CalculateOverallProgress();
                return summary;
            }
        }

        public class ProgressSummary
        {
            public List<CategorySummary> Categories { get; set; } = new List<CategorySummary>();
            public double OverallCompletionPercentage { get; set; }
            public int TotalProceduresRequired { get; set; }
            public int TotalProceduresCompleted { get; set; }
            public int TotalSimulationsUsed { get; set; }

            public void CalculateOverallProgress()
            {
                TotalProceduresRequired = Categories.Sum(c => c.TotalRequired + c.TotalAssistanceRequired);
                TotalProceduresCompleted = Categories.Sum(c => c.CompletedCount + c.AssistanceCount);
                TotalSimulationsUsed = Categories.Sum(c => c.SimulationCount);

                OverallCompletionPercentage = TotalProceduresRequired > 0
                    ? (TotalProceduresCompleted * 100.0) / TotalProceduresRequired
                    : 0;
            }

            public string GenerateReport()
            {
                var report = new StringBuilder();
                report.AppendLine("Raport postępu wykonania procedur medycznych:");
                report.AppendLine($"Całkowity postęp: {OverallCompletionPercentage:F1}%");
                report.AppendLine($"Wykonane procedury: {TotalProceduresCompleted}/{TotalProceduresRequired}");
                report.AppendLine($"Wykorzystane symulacje: {TotalSimulationsUsed}");
                report.AppendLine();

                foreach (var category in Categories)
                {
                    report.AppendLine($"Kategoria: {category.CategoryName}");
                    report.AppendLine($"- Postęp: {category.CompletionPercentage:F1}%");
                    report.AppendLine($"- Wykonane: {category.CompletedCount}/{category.TotalRequired}");
                    report.AppendLine($"- Asysty: {category.AssistanceCount}/{category.TotalAssistanceRequired}");
                    report.AppendLine($"- Symulacje: {category.SimulationCount}");
                    report.AppendLine();
                }

                return report.ToString();
            }
        }

        public class CategorySummary
        {
            public string CategoryName { get; set; }
            public int TotalRequired { get; set; }
            public int TotalAssistanceRequired { get; set; }
            public int CompletedCount { get; set; }
            public int AssistanceCount { get; set; }
            public int SimulationCount { get; set; }
            public double CompletionPercentage { get; set; }
        }
    }

    public static class DutyRequirements
    {
        public class DutySpecification
        {
            public string Type { get; set; } // Regular, Emergency, Supervised, Independent
            public int MinimumHoursPerMonth { get; set; }
            public int MinimumDutiesPerMonth { get; set; }
            public bool RequiresSupervision { get; set; }
            public List<string> RequiredCompetencies { get; set; }
            public int Year { get; set; }
        }

        public static List<DutySpecification> GetDutyRequirements()
        {
            return new List<DutySpecification>
                {
                    new DutySpecification  // SpecializationId = 1
                    {
                        Year = 1,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = true,
                        Type = "Supervised",
                        RequiredCompetencies = new List<string>
                        {
                            "Ocena stanu pacjenta",
                            "Postępowanie w gorączce neutropenicznej",
                            "Postępowanie w zespole lizy guza",
                            "Kwalifikacja do przetoczenia składników krwi"
                        }
                    },
                    new DutySpecification // SpecializationId = 2
                    {
                        Year = 1,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = true,
                        Type = "Supervised",
                        RequiredCompetencies = new List<string>
                        {
                            "Ocena stanu pacjenta",
                            "Podstawowe procedury diagnostyczne w medycynie morskiej i tropikalnej",
                            "Postępowanie w stanach nagłych związanych z podróżami i pracą w tropiku",
                            "Kwalifikacja do leczenia w trybie pilnym"
                        }
                    },
                    new DutySpecification // SpecializationId = 1
                    {
                        Year = 3,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = false,
                        Type = "Independent",
                        RequiredCompetencies = new List<string>
                        {
                            "Samodzielne prowadzenie dyżuru",
                            "Koordynacja pracy zespołu",
                            "Podejmowanie decyzji w stanach zagrożenia życia",
                            "Nadzór nad młodszymi kolegami"
                        }
                    },
                    new DutySpecification // SpecializationId = 2
                    {
                        Year = 2,
                        MinimumHoursPerMonth = 40,
                        MinimumDutiesPerMonth = 4,
                        RequiresSupervision = false,
                        Type = "Independent",
                        RequiredCompetencies = new List<string>
                        {
                            "Samodzielne prowadzenie dyżuru",
                            "Zaawansowane procedury w medycynie morskiej i tropikalnej",
                            "Udzielanie porad medycznych drogą radiową",
                            "Koordynacja pracy zespołu dyżurowego"
                        }
                    }
                };
        }
    }

    public static class DutyMonitoring
    {
        public class Duty
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Type { get; set; }
            public string Location { get; set; }
            public string Supervisor { get; set; }
            public int EmergencyCases { get; set; }
            public List<string> PerformedProcedures { get; set; } = new List<string>();
            public bool WasSupervised { get; set; }
            public List<string> KeyCompetencies { get; set; } = new List<string>();
            public double DurationInHours => (EndTime - StartTime).TotalHours;
        }

        public class DutyStats
        {
            public int TotalDuties { get; set; }
            public double TotalHours { get; set; }
            public int RegularDuties { get; set; }
            public int EmergencyDuties { get; set; }
            public int SupervisedDuties { get; set; }
            public int IndependentDuties { get; set; }
            public int TotalEmergencyCases { get; set; }
            public List<string> ProceduresPerformed { get; set; } = new List<string>();
        }

        public class DutyValidator
        {
            public (bool IsCompliant, List<string> Deficiencies) CheckMonthlyCompliance(
                int specializationYear,
                List<Duty> monthlyDuties)
            {
                var requirements = DutyRequirements.GetDutyRequirements()
                    .FirstOrDefault(r => r.Year == specializationYear);

                var deficiencies = new List<string>();

                if (requirements == null)
                {
                    deficiencies.Add($"Brak zdefiniowanych wymagań dla roku {specializationYear}");
                    return (false, deficiencies);
                }

                // Sprawdzanie liczby godzin
                var totalHours = monthlyDuties.Sum(d => d.DurationInHours);
                if (totalHours < requirements.MinimumHoursPerMonth)
                {
                    deficiencies.Add($"Brakuje {requirements.MinimumHoursPerMonth - totalHours:F1} godzin dyżurowych");
                }

                // Sprawdzanie liczby dyżurów
                if (monthlyDuties.Count < requirements.MinimumDutiesPerMonth)
                {
                    deficiencies.Add($"Brakuje {requirements.MinimumDutiesPerMonth - monthlyDuties.Count} dyżurów");
                }

                // Sprawdzanie nadzoru
                if (requirements.RequiresSupervision && monthlyDuties.Any(d => !d.WasSupervised))
                {
                    deficiencies.Add("Niektóre dyżury odbyły się bez wymaganego nadzoru");
                }

                return (deficiencies.Count == 0, deficiencies);
            }

            public DutyStats GenerateStatistics(List<Duty> duties)
            {
                return new DutyStats
                {
                    TotalDuties = duties.Count,
                    TotalHours = duties.Sum(d => d.DurationInHours),
                    RegularDuties = duties.Count(d => d.Type == "Regular"),
                    EmergencyDuties = duties.Count(d => d.Type == "Emergency"),
                    SupervisedDuties = duties.Count(d => d.WasSupervised),
                    IndependentDuties = duties.Count(d => !d.WasSupervised),
                    TotalEmergencyCases = duties.Sum(d => d.EmergencyCases),
                    ProceduresPerformed = duties.SelectMany(d => d.PerformedProcedures).ToList()
                };
            }

            public string GenerateReport(DutyStats stats)
            {
                return $@"Raport dyżurowy:
    - Całkowita liczba dyżurów: {stats.TotalDuties}
    - Całkowita liczba godzin: {stats.TotalHours:F1}
    - Dyżury zwykłe: {stats.RegularDuties}
    - Dyżury ostre: {stats.EmergencyDuties}
    - Dyżury pod nadzorem: {stats.SupervisedDuties}
    - Dyżury samodzielne: {stats.IndependentDuties}
    - Przypadki nagłe: {stats.TotalEmergencyCases}
    - Liczba wykonanych procedur: {stats.ProceduresPerformed.Count}";
            }
        }
    }
}