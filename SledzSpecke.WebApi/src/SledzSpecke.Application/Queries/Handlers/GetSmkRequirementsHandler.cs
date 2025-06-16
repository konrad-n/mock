using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetSmkRequirementsHandler : IQueryHandler<GetSmkRequirements, SmkRequirementsDto>
{
    // This would typically come from a database or configuration
    // For now, using hardcoded SMK requirements based on Polish medical specialization rules
    private static readonly Dictionary<string, Dictionary<string, SmkRequirementsDto>> Requirements = new()
    {
        ["kardiologia"] = new Dictionary<string, SmkRequirementsDto>
        {
            ["old"] = new SmkRequirementsDto
            {
                SpecializationName = "Kardiologia",
                SmkVersion = "old",
                DurationYears = 5,
                MonthlyHoursMinimum = 160,
                WeeklyHours = new WeeklyHoursRequirement
                {
                    MinimumHours = 40,
                    MaximumHours = 48,
                    AverageHours = 44,
                    AverageMinutes = 0
                },
                Modules = new List<ModuleRequirement>
                {
                    new ModuleRequirement
                    {
                        ModuleType = "Podstawowy",
                        ModuleName = "Moduł podstawowy kardiologii",
                        DurationMonths = 24,
                        RequiredInternships = 4,
                        RequiredCourses = 3
                    },
                    new ModuleRequirement
                    {
                        ModuleType = "Specjalistyczny",
                        ModuleName = "Kardiologia interwencyjna",
                        DurationMonths = 12,
                        RequiredInternships = 2,
                        RequiredCourses = 2
                    }
                },
                RequiredProcedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement
                    {
                        Code = "P001",
                        Name = "Echokardiografia",
                        RequiredCountCodeA = 100,
                        RequiredCountCodeB = 50
                    },
                    new ProcedureRequirement
                    {
                        Code = "P002",
                        Name = "Koronarografia",
                        RequiredCountCodeA = 50,
                        RequiredCountCodeB = 100
                    }
                },
                RequiredCourses = new List<CourseRequirement>
                {
                    new CourseRequirement
                    {
                        CourseType = "Obowiązkowy",
                        CourseName = "Kurs podstawowy kardiologii",
                        RequiresCmkpCertificate = true,
                        MinimumHours = 40
                    }
                }
            },
            ["new"] = new SmkRequirementsDto
            {
                SpecializationName = "Kardiologia",
                SmkVersion = "new",
                DurationYears = 6,
                MonthlyHoursMinimum = 160,
                WeeklyHours = new WeeklyHoursRequirement
                {
                    MinimumHours = 40,
                    MaximumHours = 48,
                    AverageHours = 44,
                    AverageMinutes = 0
                },
                Modules = new List<ModuleRequirement>
                {
                    new ModuleRequirement
                    {
                        ModuleType = "Podstawowy",
                        ModuleName = "Moduł podstawowy kardiologii",
                        DurationMonths = 36,
                        RequiredInternships = 6,
                        RequiredCourses = 4
                    },
                    new ModuleRequirement
                    {
                        ModuleType = "Specjalistyczny",
                        ModuleName = "Kardiologia zaawansowana",
                        DurationMonths = 24,
                        RequiredInternships = 4,
                        RequiredCourses = 3
                    }
                },
                RequiredProcedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement
                    {
                        Code = "N001",
                        Name = "Echokardiografia przezklatkowa",
                        RequiredCountCodeA = 150,
                        RequiredCountCodeB = 75
                    },
                    new ProcedureRequirement
                    {
                        Code = "N002",
                        Name = "Angioplastyka wieńcowa",
                        RequiredCountCodeA = 75,
                        RequiredCountCodeB = 150
                    }
                },
                RequiredCourses = new List<CourseRequirement>
                {
                    new CourseRequirement
                    {
                        CourseType = "Obowiązkowy",
                        CourseName = "Kurs specjalistyczny kardiologii",
                        RequiresCmkpCertificate = true,
                        MinimumHours = 60
                    }
                }
            }
        },
        ["chirurgia-ogolna"] = new Dictionary<string, SmkRequirementsDto>
        {
            ["old"] = new SmkRequirementsDto
            {
                SpecializationName = "Chirurgia ogólna",
                SmkVersion = "old",
                DurationYears = 6,
                MonthlyHoursMinimum = 160,
                WeeklyHours = new WeeklyHoursRequirement
                {
                    MinimumHours = 40,
                    MaximumHours = 48,
                    AverageHours = 45,
                    AverageMinutes = 30
                },
                Modules = new List<ModuleRequirement>
                {
                    new ModuleRequirement
                    {
                        ModuleType = "Podstawowy",
                        ModuleName = "Chirurgia podstawowa",
                        DurationMonths = 36,
                        RequiredInternships = 6,
                        RequiredCourses = 4
                    }
                },
                RequiredProcedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement
                    {
                        Code = "CH001",
                        Name = "Appendektomia",
                        RequiredCountCodeA = 20,
                        RequiredCountCodeB = 40
                    }
                },
                RequiredCourses = new List<CourseRequirement>
                {
                    new CourseRequirement
                    {
                        CourseType = "Obowiązkowy",
                        CourseName = "Kurs chirurgii małoinwazyjnej",
                        RequiresCmkpCertificate = true,
                        MinimumHours = 40
                    }
                }
            },
            ["new"] = new SmkRequirementsDto
            {
                SpecializationName = "Chirurgia ogólna",
                SmkVersion = "new",
                DurationYears = 6,
                MonthlyHoursMinimum = 160,
                WeeklyHours = new WeeklyHoursRequirement
                {
                    MinimumHours = 40,
                    MaximumHours = 48,
                    AverageHours = 45,
                    AverageMinutes = 30
                },
                Modules = new List<ModuleRequirement>
                {
                    new ModuleRequirement
                    {
                        ModuleType = "Podstawowy",
                        ModuleName = "Chirurgia podstawowa",
                        DurationMonths = 48,
                        RequiredInternships = 8,
                        RequiredCourses = 5
                    }
                },
                RequiredProcedures = new List<ProcedureRequirement>
                {
                    new ProcedureRequirement
                    {
                        Code = "NCH001",
                        Name = "Operacje laparoskopowe",
                        RequiredCountCodeA = 50,
                        RequiredCountCodeB = 100
                    }
                },
                RequiredCourses = new List<CourseRequirement>
                {
                    new CourseRequirement
                    {
                        CourseType = "Obowiązkowy",
                        CourseName = "Kurs zaawansowanych technik chirurgicznych",
                        RequiresCmkpCertificate = true,
                        MinimumHours = 80
                    }
                }
            }
        }
    };

    public Task<SmkRequirementsDto> HandleAsync(GetSmkRequirements query)
    {
        var specializationKey = query.Specialization.ToLower().Replace(" ", "-");
        var smkVersionKey = query.SmkVersion.ToLower();
        
        if (Requirements.TryGetValue(specializationKey, out var specializationRequirements))
        {
            if (specializationRequirements.TryGetValue(smkVersionKey, out var requirements))
            {
                return Task.FromResult(requirements);
            }
        }
        
        // Return default requirements if specific ones not found
        return Task.FromResult(new SmkRequirementsDto
        {
            SpecializationName = query.Specialization,
            SmkVersion = query.SmkVersion,
            DurationYears = 5,
            MonthlyHoursMinimum = 160,
            WeeklyHours = new WeeklyHoursRequirement
            {
                MinimumHours = 40,
                MaximumHours = 48,
                AverageHours = 44,
                AverageMinutes = 0
            },
            Modules = new List<ModuleRequirement>
            {
                new ModuleRequirement
                {
                    ModuleType = "Podstawowy",
                    ModuleName = "Moduł podstawowy",
                    DurationMonths = 24,
                    RequiredInternships = 4,
                    RequiredCourses = 2
                },
                new ModuleRequirement
                {
                    ModuleType = "Specjalistyczny",
                    ModuleName = "Moduł specjalistyczny",
                    DurationMonths = 24,
                    RequiredInternships = 4,
                    RequiredCourses = 2
                }
            },
            RequiredProcedures = new List<ProcedureRequirement>(),
            RequiredCourses = new List<CourseRequirement>
            {
                new CourseRequirement
                {
                    CourseType = "Obowiązkowy",
                    CourseName = "Kurs podstawowy",
                    RequiresCmkpCertificate = true,
                    MinimumHours = 40
                }
            }
        });
    }
}