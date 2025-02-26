using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Text;
using System;

public static class DataSeeder
{
    public static List<Specialization> GetBasicSpecializations()
    {
        return new List<Specialization>
        {
            new Specialization
            {
                Id = 5, // Kolejny numer po wcześniejszych specjalizacjach
                Name = "Hematologia",
                DurationInWeeks = 156, // 3 lata (52 tygodnie × 3)
                ProgramVersion = "2023",
                ApprovalDate = new DateTime(2023, 1, 1),
                MinimumDutyHours = 480, // 10 godzin 5 minut na tydzień
                Requirements = "Program specjalizacji w dziedzinie hematologii dla lekarzy, którzy zrealizowali i zaliczyli moduł podstawowy w zakresie chorób wewnętrznych",
                Description = "Celem szkolenia specjalizacyjnego jest opanowanie wiedzy teoretycznej i umiejętności praktycznych umożliwiających diagnozowanie, profilaktykę i leczenie chorób na poziomie zapewniającym samodzielne udzielanie świadczeń zdrowotnych według najwyższych standardów."
            }
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
            }
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
                SpecializationId = 5,
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
            }
        };
    }

    public static Dictionary<string, List<RequiredProcedure>> GetRequiredProcedures()
    {
        return new Dictionary<string, List<RequiredProcedure>>
        {
            {
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
            {
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
            {
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
            }
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
        public List<ProcedureExecution> Executions { get; set; } = new();
    }

    public class ProcedureExecution
    {
        public DateTime ExecutionDate { get; set; }
        public string Type { get; set; } // "Wykonanie", "Asysta"
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
            var requirements = GetAllRequiredProcedures();
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
                }
            }

            return (deficiencies.Count == 0, deficiencies);
        }

        private Dictionary<string, List<RequiredProcedure>> GetAllRequiredProcedures()
        {
            // W rzeczywistej implementacji ta metoda zwracałaby procedury z DataSeeder
            return DataSeeder.GetRequiredProcedures();
        }
    }

    public class ProgressSummary
    {
        public List<CategorySummary> Categories { get; set; } = new();
        public double OverallCompletionPercentage { get; set; }
        public int TotalProceduresRequired { get; set; }
        public int TotalProceduresCompleted { get; set; }

        public void CalculateOverallProgress()
        {
            TotalProceduresRequired = Categories.Sum(c => c.TotalRequired + c.TotalAssistanceRequired);
            TotalProceduresCompleted = Categories.Sum(c => c.CompletedCount + c.AssistanceCount);
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
            report.AppendLine();

            foreach (var category in Categories)
            {
                report.AppendLine($"Kategoria: {category.CategoryName}");
                report.AppendLine($"- Postęp: {category.CompletionPercentage:F1}%");
                report.AppendLine($"- Wykonane: {category.CompletedCount}/{category.TotalRequired}");
                report.AppendLine($"- Asysty: {category.AssistanceCount}/{category.TotalAssistanceRequired}");
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
            new DutySpecification
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
            new DutySpecification
            {
                Year = 2,
                MinimumHoursPerMonth = 40,
                MinimumDutiesPerMonth = 4,
                RequiresSupervision = false,
                Type = "Independent",
                RequiredCompetencies = new List<string>
                {
                    "Samodzielne prowadzenie dyżuru",
                    "Postępowanie w hiperleukocytozie",
                    "Postępowanie w zespole nadlepkości",
                    "Postępowanie w zaburzeniach krzepnięcia"
                }
            },
            new DutySpecification
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
        public List<string> PerformedProcedures { get; set; } = new();
        public bool WasSupervised { get; set; }
        public List<string> KeyCompetencies { get; set; } = new();
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
        public List<string> ProceduresPerformed { get; set; } = new();
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
- Liczba wykonanych procedur: {stats.ProceduresPerformed.Count}";
        }
    }
}
