using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class CourseRequirementService : ICourseRequirementService
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    // Real SMK course requirements based on CMKP 2023 guidelines
    private readonly Dictionary<string, List<CourseRequirement>> _courseRequirementsBySpecialization = new()
    {
        // Cardiology (Kardiologia) - New SMK
        ["KARD"] = new List<CourseRequirement>
        {
            // Basic Module - Internal Medicine courses (2 years)
            new() { CourseId = new CourseId(1), CourseName = "Diagnostyka obrazowa", CourseCode = "KARD-001", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(2), CourseName = "Alergologia", CourseCode = "KARD-002", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(3), CourseName = "Onkologia kliniczna", CourseCode = "KARD-003", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(4), CourseName = "Medycyna paliatywna", CourseCode = "KARD-004", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(5), CourseName = "Toksykologia", CourseCode = "KARD-005", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(6), CourseName = "Geriatria", CourseCode = "KARD-006", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(7), CourseName = "Diabetologia", CourseCode = "KARD-007", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(8), CourseName = "Przetaczanie krwi i jej składników", CourseCode = "KARD-008", RequiredHours = 16, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(9), CourseName = "Orzecznictwo lekarskie", CourseCode = "KARD-009", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(10), CourseName = "Profilaktyka i promocja zdrowia", CourseCode = "KARD-010", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            
            // Specialistic Module - Cardiology courses (3 years)
            new() { CourseId = new CourseId(11), CourseName = "Wprowadzenie do specjalizacji w dziedzinie kardiologii", CourseCode = "KARD-S01", RequiredHours = 8, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(12), CourseName = "Patofizjologia chorób sercowo-naczyniowych", CourseCode = "KARD-S02", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(13), CourseName = "Farmakoterapia chorób sercowo-naczyniowych", CourseCode = "KARD-S03", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(14), CourseName = "Nieinwazyjna diagnostyka elektrokardiograficzna", CourseCode = "KARD-S04", RequiredHours = 16, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(15), CourseName = "Diagnostyka obrazowa – echokardiografia", CourseCode = "KARD-S05", RequiredHours = 8, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(16), CourseName = "Diagnostyka obrazowa – nowe techniki obrazowania", CourseCode = "KARD-S06", RequiredHours = 16, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(17), CourseName = "Intensywna terapia kardiologiczna", CourseCode = "KARD-S07", RequiredHours = 16, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(18), CourseName = "Elektrofizjologia i elektroterapia", CourseCode = "KARD-S08", RequiredHours = 24, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(19), CourseName = "Diagnostyka inwazyjna i leczenie interwencyjne", CourseCode = "KARD-S09", RequiredHours = 24, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(20), CourseName = "Wrodzone i nabyte wady serca", CourseCode = "KARD-S10", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(21), CourseName = "Onkologia w kardiologii", CourseCode = "KARD-S11", RequiredHours = 8, IsMandatory = false, Type = CourseType.Seminar, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(22), CourseName = "Przewlekły zespół wieńcowy", CourseCode = "KARD-S12", RequiredHours = 8, IsMandatory = false, Type = CourseType.Seminar, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(23), CourseName = "Nadciśnienie tętnicze", CourseCode = "KARD-S13", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(24), CourseName = "Nadciśnienie płucne i niewydolność prawej komory serca", CourseCode = "KARD-S14", RequiredHours = 8, IsMandatory = false, Type = CourseType.Seminar, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(25), CourseName = "Niewydolność serca", CourseCode = "KARD-S15", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(26), CourseName = "Diagnostyka i leczenie chorób naczyń obwodowych", CourseCode = "KARD-S16", RequiredHours = 8, IsMandatory = false, Type = CourseType.Seminar, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(27), CourseName = "Choroby rzadkie w kardiologii", CourseCode = "KARD-S17", RequiredHours = 8, IsMandatory = false, Type = CourseType.Seminar, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(28), CourseName = "Kurs atestacyjny: Kardiologia", CourseCode = "KARD-S18", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // Psychiatry (Psychiatria) - New SMK
        ["PSYCH"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(29), CourseName = "Wprowadzenie do specjalizacji w dziedzinie psychiatrii", CourseCode = "PSYCH-001", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(30), CourseName = "Psychiatria sądowa i opiniowanie sądowo-psychiatryczne", CourseCode = "PSYCH-002", RequiredHours = 80, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(31), CourseName = "Psychiatria środowiskowa i rehabilitacja psychiatryczna", CourseCode = "PSYCH-003", RequiredHours = 40, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(32), CourseName = "Wybrane zagadnienia z zakresu psychiatrii klinicznej", CourseCode = "PSYCH-004", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(33), CourseName = "Wprowadzenie do psychoterapii", CourseCode = "PSYCH-005", RequiredHours = 40, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(34), CourseName = "Orzecznictwo lekarskie", CourseCode = "PSYCH-006", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(35), CourseName = "Profilaktyka i promocja zdrowia", CourseCode = "PSYCH-007", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(36), CourseName = "Kurs atestacyjny: Psychiatria", CourseCode = "PSYCH-008", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // Anesthesiology (Anestezjologia) - New SMK
        ["ANEST"] = new List<CourseRequirement>
        {
            // Basic module courses
            new() { CourseId = new CourseId(37), CourseName = "Podstawy anestezjologii", CourseCode = "ANEST-001", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(38), CourseName = "Intensywna terapia", CourseCode = "ANEST-002", RequiredHours = 60, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(39), CourseName = "Diagnostyka obrazowa", CourseCode = "ANEST-003", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(40), CourseName = "Orzecznictwo lekarskie", CourseCode = "ANEST-004", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            
            // Specialistic module courses
            new() { CourseId = new CourseId(41), CourseName = "Zaawansowane techniki znieczulenia", CourseCode = "ANEST-S01", RequiredHours = 32, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(42), CourseName = "Anestezja w kardiochirurgii", CourseCode = "ANEST-S02", RequiredHours = 24, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(43), CourseName = "Anestezja pediatryczna", CourseCode = "ANEST-S03", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(44), CourseName = "Terapia bólu", CourseCode = "ANEST-S04", RequiredHours = 40, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(45), CourseName = "Kurs atestacyjny: Anestezjologia", CourseCode = "ANEST-S05", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // Internal Medicine (Choroby wewnętrzne) - New SMK
        ["CHOW"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(46), CourseName = "Wprowadzenie do specjalizacji w dziedzinie chorób wewnętrznych", CourseCode = "CHOW-001", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(47), CourseName = "Kardiologia", CourseCode = "CHOW-002", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(48), CourseName = "Pulmonologia", CourseCode = "CHOW-003", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(49), CourseName = "Gastroenterologia", CourseCode = "CHOW-004", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(50), CourseName = "Nefrologia", CourseCode = "CHOW-005", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(51), CourseName = "Endokrynologia", CourseCode = "CHOW-006", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(52), CourseName = "Hematologia", CourseCode = "CHOW-007", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(53), CourseName = "Reumatologia", CourseCode = "CHOW-008", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(54), CourseName = "Diabetologia", CourseCode = "CHOW-009", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(55), CourseName = "Geriatria", CourseCode = "CHOW-010", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(56), CourseName = "Kurs atestacyjny: Choroby wewnętrzne", CourseCode = "CHOW-011", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // Pediatrics (Pediatria) - New SMK
        ["PED"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(57), CourseName = "Wprowadzenie do specjalizacji w dziedzinie pediatrii", CourseCode = "PED-001", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(58), CourseName = "Neonatologia", CourseCode = "PED-002", RequiredHours = 40, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(59), CourseName = "Choroby zakaźne wieku dziecięcego", CourseCode = "PED-003", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(60), CourseName = "Kardiologia dziecięca", CourseCode = "PED-004", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(61), CourseName = "Gastroenterologia dziecięca", CourseCode = "PED-005", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(62), CourseName = "Neurologia dziecięca", CourseCode = "PED-006", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(63), CourseName = "Endokrynologia i diabetologia dziecięca", CourseCode = "PED-007", RequiredHours = 24, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(64), CourseName = "Onkologia i hematologia dziecięca", CourseCode = "PED-008", RequiredHours = 32, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(65), CourseName = "Intensywna terapia dziecięca", CourseCode = "PED-009", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(66), CourseName = "Kurs atestacyjny: Pediatria", CourseCode = "PED-010", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // General Surgery (Chirurgia ogólna) - New SMK
        ["CHOG"] = new List<CourseRequirement>
        {
            // Basic module
            new() { CourseId = new CourseId(67), CourseName = "Podstawy chirurgii", CourseCode = "CHOG-001", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(68), CourseName = "Techniki operacyjne", CourseCode = "CHOG-002", RequiredHours = 48, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Basic },
            new() { CourseId = new CourseId(69), CourseName = "Anestezjologia dla chirurgów", CourseCode = "CHOG-003", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Basic },
            
            // Specialistic module
            new() { CourseId = new CourseId(70), CourseName = "Chirurgia brzuszna", CourseCode = "CHOG-S01", RequiredHours = 40, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(71), CourseName = "Chirurgia naczyniowa", CourseCode = "CHOG-S02", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(72), CourseName = "Chirurgia endokrynologiczna", CourseCode = "CHOG-S03", RequiredHours = 24, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(73), CourseName = "Chirurgia onkologiczna", CourseCode = "CHOG-S04", RequiredHours = 40, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(74), CourseName = "Chirurgia urazowa", CourseCode = "CHOG-S05", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(75), CourseName = "Chirurgia małoinwazyjna", CourseCode = "CHOG-S06", RequiredHours = 40, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(76), CourseName = "Kurs atestacyjny: Chirurgia ogólna", CourseCode = "CHOG-S07", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        },
        
        // Family Medicine (Medycyna rodzinna) - New SMK
        ["MEDR"] = new List<CourseRequirement>
        {
            new() { CourseId = new CourseId(77), CourseName = "Wprowadzenie do medycyny rodzinnej", CourseCode = "MEDR-001", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(78), CourseName = "Promocja zdrowia i profilaktyka", CourseCode = "MEDR-002", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(79), CourseName = "Choroby przewlekłe w praktyce lekarza rodzinnego", CourseCode = "MEDR-003", RequiredHours = 32, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(80), CourseName = "Pediatria w medycynie rodzinnej", CourseCode = "MEDR-004", RequiredHours = 24, IsMandatory = true, Type = CourseType.Practical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(81), CourseName = "Geriatria w medycynie rodzinnej", CourseCode = "MEDR-005", RequiredHours = 24, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(82), CourseName = "Psychiatria w praktyce lekarza rodzinnego", CourseCode = "MEDR-006", RequiredHours = 16, IsMandatory = true, Type = CourseType.Workshop, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(83), CourseName = "Medycyna paliatywna", CourseCode = "MEDR-007", RequiredHours = 16, IsMandatory = true, Type = CourseType.Theoretical, ModuleType = ModuleType.Specialist },
            new() { CourseId = new CourseId(84), CourseName = "Kurs atestacyjny: Medycyna rodzinna", CourseCode = "MEDR-008", RequiredHours = 40, IsMandatory = true, Type = CourseType.Certification, ModuleType = ModuleType.Specialist }
        }
    };
    
    // Mock completed courses data - in real implementation would come from database
    private readonly Dictionary<int, List<CompletedCourse>> _completedCoursesByUser = new();

    public CourseRequirementService(
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<CourseRequirements>> GetRequirementsAsync(
        SpecializationId specializationId,
        SmkVersion smkVersion,
        int? year = null,
        ModuleId? moduleId = null)
    {
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseRequirements>.Failure("Nie znaleziono specjalizacji");
        }

        var requirements = new CourseRequirements
        {
            SpecializationId = specializationId,
            SmkVersion = smkVersion,
            Year = year,
            ModuleId = moduleId
        };

        // Get courses for specialization
        if (_courseRequirementsBySpecialization.TryGetValue(specialization.ProgramCode, out var courses))
        {
            // Filter by year/module if needed
            if (smkVersion == SmkVersion.Old && year.HasValue)
            {
                // For Old SMK, distribute courses across years
                // Note: Most Old SMK specializations don't have module-based courses
                var allCourses = courses.Where(c => c.ModuleType == null || c.ModuleType == ModuleType.Basic).ToList();
                var coursesPerYear = Math.Max(1, allCourses.Count / specialization.DurationYears);
                var startIndex = (year.Value - 1) * coursesPerYear;
                var endIndex = Math.Min(startIndex + coursesPerYear, allCourses.Count);
                requirements.RequiredCourses = allCourses.Skip(startIndex).Take(endIndex - startIndex).ToList();
            }
            else if (smkVersion == SmkVersion.New && moduleId != null)
            {
                // For New SMK, filter courses by module type
                var moduleType = moduleId.Value <= 2 ? ModuleType.Basic : ModuleType.Specialist;
                requirements.RequiredCourses = courses
                    .Where(c => c.ModuleType == moduleType)
                    .ToList();
            }
            else
            {
                // Return all courses if no specific year/module requested
                requirements.RequiredCourses = courses;
            }
        }

        // Calculate totals
        requirements.TotalRequiredHours = requirements.RequiredCourses.Sum(c => c.RequiredHours);
        requirements.MinimumPassingCourses = requirements.RequiredCourses.Count(c => c.IsMandatory);

        return Result<CourseRequirements>.Success(requirements);
    }

    public async Task<Result> RecordCourseParticipationAsync(
        UserId userId,
        CourseId courseId,
        DateTime participationDate,
        int hoursCompleted,
        bool isPassed)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("Nie znaleziono użytkownika");
        }

        // Validate hours
        if (hoursCompleted <= 0)
        {
            return Result.Failure("Liczba godzin musi być większa niż 0");
        }

        // Record the participation
        var completedCourse = new CompletedCourse
        {
            CourseId = courseId,
            CourseName = GetCourseName(courseId), // In real implementation, would fetch from DB
            CompletionDate = participationDate,
            HoursCompleted = hoursCompleted,
            IsPassed = isPassed,
            CertificateNumber = isPassed ? GenerateCertificateNumber() : null
        };

        // Add to user's completed courses
        if (!_completedCoursesByUser.ContainsKey(userId.Value))
        {
            _completedCoursesByUser[userId.Value] = new List<CompletedCourse>();
        }
        _completedCoursesByUser[userId.Value].Add(completedCourse);

        // Raise domain event
        var courseCompletedEvent = new CourseCompletedEvent(
            userId,
            courseId,
            participationDate,
            hoursCompleted,
            isPassed);
        
        await _eventDispatcher.DispatchAsync(courseCompletedEvent);

        return Result.Success();
    }

    public async Task<Result<CourseProgress>> GetUserCourseProgressAsync(
        UserId userId,
        SpecializationId specializationId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<CourseProgress>.Failure("Nie znaleziono użytkownika");
        }

        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseProgress>.Failure("Nie znaleziono specjalizacji");
        }

        var progress = new CourseProgress
        {
            UserId = userId,
            SpecializationId = specializationId
        };

        // Get user's completed courses
        if (_completedCoursesByUser.TryGetValue(userId.Value, out var completedCourses))
        {
            progress.CompletedCourses = completedCourses;
            progress.TotalHoursCompleted = completedCourses.Sum(c => c.HoursCompleted);
            progress.TotalCoursesCompleted = completedCourses.Count;
            progress.TotalCoursesPassed = completedCourses.Count(c => c.IsPassed);
        }

        // Get requirements
        var requirementsResult = await GetRequirementsAsync(
            specializationId, 
            specialization.SmkVersion, 
            null, 
            null);
        
        if (!requirementsResult.IsSuccess)
        {
            return Result<CourseProgress>.Failure(requirementsResult.Error);
        }

        var requirements = requirementsResult.Value;
        
        // Calculate remaining courses
        var completedCourseIds = progress.CompletedCourses.Select(c => c.CourseId.Value).ToHashSet();
        progress.RemainingRequiredCourses = requirements.RequiredCourses
            .Where(r => !completedCourseIds.Contains(r.CourseId.Value))
            .ToList();

        // Calculate completion percentage
        if (requirements.RequiredCourses.Any())
        {
            var mandatoryCompleted = progress.CompletedCourses
                .Count(c => c.IsPassed && requirements.RequiredCourses
                    .Any(r => r.CourseId.Value == c.CourseId.Value && r.IsMandatory));
            
            var totalMandatory = requirements.RequiredCourses.Count(r => r.IsMandatory);
            progress.CompletionPercentage = totalMandatory > 0 
                ? (mandatoryCompleted * 100.0) / totalMandatory 
                : 100;
        }

        return Result<CourseProgress>.Success(progress);
    }

    public async Task<Result<CourseCompletionStatus>> ValidateCourseCompletionAsync(
        UserId userId,
        SpecializationId specializationId,
        int? year = null,
        ModuleId? moduleId = null)
    {
        var progressResult = await GetUserCourseProgressAsync(userId, specializationId);
        if (!progressResult.IsSuccess)
        {
            return Result<CourseCompletionStatus>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<CourseCompletionStatus>.Failure("Nie znaleziono specjalizacji");
        }

        var requirementsResult = await GetRequirementsAsync(
            specializationId,
            specialization.SmkVersion,
            year,
            moduleId);
        
        if (!requirementsResult.IsSuccess)
        {
            return Result<CourseCompletionStatus>.Failure(requirementsResult.Error);
        }

        var requirements = requirementsResult.Value;
        var status = new CourseCompletionStatus
        {
            RequiredHours = requirements.TotalRequiredHours,
            CompletedHours = progress.TotalHoursCompleted,
            RequiredCourses = requirements.MinimumPassingCourses,
            CompletedCourses = progress.TotalCoursesPassed
        };

        // Check hours requirement
        if (status.CompletedHours < status.RequiredHours)
        {
            status.UnmetRequirements.Add($"Brakuje {status.RequiredHours - status.CompletedHours} godzin kursów");
        }

        // Check mandatory courses
        var completedCourseIds = progress.CompletedCourses
            .Where(c => c.IsPassed)
            .Select(c => c.CourseId.Value)
            .ToHashSet();
        
        status.MissingMandatoryCourses = requirements.RequiredCourses
            .Where(r => r.IsMandatory && !completedCourseIds.Contains(r.CourseId.Value))
            .ToList();

        if (status.MissingMandatoryCourses.Any())
        {
            status.UnmetRequirements.Add($"Brakuje {status.MissingMandatoryCourses.Count} obowiązkowych kursów");
        }

        // Check minimum courses
        if (status.CompletedCourses < status.RequiredCourses)
        {
            status.UnmetRequirements.Add($"Brakuje {status.RequiredCourses - status.CompletedCourses} zaliczonych kursów");
        }

        status.MeetsAllRequirements = !status.UnmetRequirements.Any();

        return Result<CourseCompletionStatus>.Success(status);
    }

    public async Task<Result<IEnumerable<RequiredCourse>>> GetUpcomingRequiredCoursesAsync(
        UserId userId,
        SpecializationId specializationId)
    {
        var progressResult = await GetUserCourseProgressAsync(userId, specializationId);
        if (!progressResult.IsSuccess)
        {
            return Result<IEnumerable<RequiredCourse>>.Failure(progressResult.Error);
        }

        var progress = progressResult.Value;
        var upcomingCourses = new List<RequiredCourse>();

        foreach (var requirement in progress.RemainingRequiredCourses)
        {
            var requiredCourse = new RequiredCourse
            {
                Requirement = requirement,
                Priority = requirement.IsMandatory ? 1 : 2,
                RecommendedTiming = GetRecommendedTiming(requirement),
                NextAvailableDate = GetNextCourseDate(requirement.CourseId),
                Location = GetCourseLocation(requirement.CourseId)
            };
            
            upcomingCourses.Add(requiredCourse);
        }

        // Sort by priority and date
        upcomingCourses = upcomingCourses
            .OrderBy(c => c.Priority)
            .ThenBy(c => c.NextAvailableDate ?? DateTime.MaxValue)
            .ToList();

        return Result<IEnumerable<RequiredCourse>>.Success(upcomingCourses);
    }

    // Helper methods
    private string GetCourseName(CourseId courseId)
    {
        // In real implementation, would fetch from database
        return _courseRequirementsBySpecialization
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(c => c.CourseId.Value == courseId.Value)?.CourseName ?? "Unknown Course";
    }

    private string GenerateCertificateNumber()
    {
        // Generate CMKP-style certificate number
        return $"CMKP/{DateTime.UtcNow:yyyy}/{Random.Shared.Next(10000, 99999)}";
    }

    private string GetRecommendedTiming(CourseRequirement requirement)
    {
        if (requirement.IsMandatory)
        {
            return "Jak najszybciej - kurs obowiązkowy";
        }
        return "W dowolnym momencie specjalizacji";
    }

    private DateTime? GetNextCourseDate(CourseId courseId)
    {
        // Mock implementation - in real system would check course schedule
        var daysUntilNext = Random.Shared.Next(7, 60);
        return DateTime.UtcNow.AddDays(daysUntilNext);
    }

    private string GetCourseLocation(CourseId courseId)
    {
        // Real Polish medical training centers
        var locations = new[] 
        { 
            "CMKP Warszawa",
            "Centrum Medyczne Kształcenia Podyplomowego Warszawa",
            "Szpital Uniwersytecki w Krakowie",
            "Uniwersytet Medyczny w Warszawie",
            "Uniwersytet Jagielloński - Collegium Medicum",
            "Gdański Uniwersytet Medyczny", 
            "Uniwersytet Medyczny we Wrocławiu",
            "Centrum Onkologii - Instytut im. M. Skłodowskiej-Curie",
            "Instytut Kardiologii w Warszawie",
            "Online - platforma CMKP"
        };
        return locations[Random.Shared.Next(locations.Length)];
    }
}

// Domain Event
public class CourseCompletedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public UserId UserId { get; }
    public CourseId CourseId { get; }
    public DateTime CompletionDate { get; }
    public int HoursCompleted { get; }
    public bool IsPassed { get; }
    public DateTime OccurredAt { get; }

    public CourseCompletedEvent(
        UserId userId,
        CourseId courseId,
        DateTime completionDate,
        int hoursCompleted,
        bool isPassed)
    {
        EventId = Guid.NewGuid();
        UserId = userId;
        CourseId = courseId;
        CompletionDate = completionDate;
        HoursCompleted = hoursCompleted;
        IsPassed = isPassed;
        OccurredAt = DateTime.UtcNow;
    }
}