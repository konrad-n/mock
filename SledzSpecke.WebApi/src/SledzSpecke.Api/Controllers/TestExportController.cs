using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Export.DTO;
using SledzSpecke.Infrastructure.Export;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/test-export")]
public class TestExportController : ControllerBase
{
    [HttpGet("preview")]
    public IActionResult GetTestPreview([FromQuery] string version = "old")
    {
        var testData = new SpecializationExportDto
        {
            BasicInfo = new BasicInfoExportDto
            {
                Pesel = "91010112345",
                PwzNumber = "1234567",
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                PhoneNumber = "+48123456789",
                SpecializationName = "Chirurgia Ogólna",
                SmkVersion = version,
                ProgramVariant = "normal",
                PlannedPesYear = "2027",
                SpecializationStartDate = "01.03.2022",
                SpecializationEndDate = "01.03.2027",
                CurrentModuleName = "Podstawowy",
                CurrentModuleStartDate = "01.03.2022",
                CorrespondenceAddress = "ul. Testowa 123, 00-001 Warszawa"
            },
            
            Internships = new List<InternshipExportDto>
            {
                new InternshipExportDto
                {
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii",
                    InstitutionName = "Szpital Uniwersytecki",
                    DepartmentName = "Oddział Chirurgii",
                    StartDate = "01.03.2022",
                    EndDate = "31.08.2022",
                    DurationDays = 183,
                    SupervisorName = "prof. dr hab. Jan Nowak",
                    SupervisorPwz = "7654321",
                    ModuleName = "Podstawowy",
                    Status = "Zakończony"
                }
            },
            
            Courses = new List<CourseExportDto>
            {
                new CourseExportDto
                {
                    CourseName = "Kurs Podstaw Chirurgii",
                    CourseNumber = "CMKP/2022/CHR/001",
                    Provider = "CMKP",
                    StartDate = "10.05.2022",
                    EndDate = "14.05.2022",
                    CreditHours = 40,
                    ModuleName = "Podstawowy",
                    CourseType = "Obowiązkowy",
                    CertificateNumber = "CERT/2022/001",
                    Status = "Zakończony"
                }
            },
            
            MedicalShifts = new List<MedicalShiftExportDto>
            {
                new MedicalShiftExportDto
                {
                    Date = "15.05.2022",
                    StartTime = "08:00",
                    EndTime = "20:00",
                    Duration = "12:00",
                    Location = "Oddział Chirurgii",
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii",
                    ModuleName = "Podstawowy",
                    SupervisorName = "dr Anna Kowalska",
                    Notes = ""
                }
            },
            
            Procedures = GetTestProcedures(version),
            
            AdditionalSelfEducationDays = new List<AdditionalSelfEducationExportDto>
            {
                new AdditionalSelfEducationExportDto
                {
                    StartDate = "15.07.2022",
                    EndDate = "15.07.2022",
                    NumberOfDays = 1,
                    Purpose = "Konferencja naukowa",
                    EventName = "XIII Konferencja Chirurgii Ogólnej",
                    ModuleName = "Podstawowy",
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii"
                }
            }
        };
        
        return Ok(testData);
    }
    
    [HttpGet("validate")]
    public IActionResult ValidateTestData()
    {
        return Ok(new { valid = true, errors = new List<string>() });
    }
    
    [HttpGet("generate-excel")]
    public async Task<IActionResult> GenerateTestExcel([FromServices] ISmkExcelGenerator generator, [FromQuery] string version = "old")
    {
        try
        {
            var testData = GetTestData(version);
            
            byte[] excelBytes = await generator.GenerateAsync(testData);
            
            return File(excelBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"test_export_{version}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
    
    private SpecializationExportDto GetTestData(string version = "old")
    {
        return new SpecializationExportDto
        {
            BasicInfo = new BasicInfoExportDto
            {
                Pesel = "91010112345",
                PwzNumber = "1234567",
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                PhoneNumber = "+48123456789",
                SpecializationName = "Chirurgia Ogólna",
                SmkVersion = version,
                ProgramVariant = "normal",
                PlannedPesYear = "2027",
                SpecializationStartDate = "01.03.2022",
                SpecializationEndDate = "01.03.2027",
                CurrentModuleName = "Podstawowy",
                CurrentModuleStartDate = "01.03.2022",
                CorrespondenceAddress = "ul. Testowa 123, 00-001 Warszawa"
            },
            
            Internships = new List<InternshipExportDto>
            {
                new InternshipExportDto
                {
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii",
                    InstitutionName = "Szpital Uniwersytecki",
                    DepartmentName = "Oddział Chirurgii",
                    StartDate = "01.03.2022",
                    EndDate = "31.08.2022",
                    DurationDays = 183,
                    SupervisorName = "prof. dr hab. Jan Nowak",
                    SupervisorPwz = "7654321",
                    ModuleName = "Podstawowy",
                    Status = "Zakończony"
                }
            },
            
            Courses = new List<CourseExportDto>
            {
                new CourseExportDto
                {
                    CourseName = "Kurs Podstaw Chirurgii",
                    CourseNumber = "CMKP/2022/CHR/001",
                    Provider = "CMKP",
                    StartDate = "10.05.2022",
                    EndDate = "14.05.2022",
                    CreditHours = 40,
                    ModuleName = "Podstawowy",
                    CourseType = "Obowiązkowy",
                    CertificateNumber = "CERT/2022/001",
                    Status = "Zakończony"
                }
            },
            
            MedicalShifts = new List<MedicalShiftExportDto>
            {
                new MedicalShiftExportDto
                {
                    Date = "15.05.2022",
                    StartTime = "08:00",
                    EndTime = "20:00",
                    Duration = "12:00",
                    Location = "Oddział Chirurgii",
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii",
                    ModuleName = "Podstawowy",
                    SupervisorName = "dr Anna Kowalska",
                    Notes = ""
                }
            },
            
            Procedures = GetTestProcedures(version),
            
            AdditionalSelfEducationDays = new List<AdditionalSelfEducationExportDto>
            {
                new AdditionalSelfEducationExportDto
                {
                    StartDate = "15.07.2022",
                    EndDate = "15.07.2022",
                    NumberOfDays = 1,
                    Purpose = "Konferencja naukowa",
                    EventName = "XIII Konferencja Chirurgii Ogólnej",
                    ModuleName = "Podstawowy",
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii"
                }
            }
        };
    }

    private List<ProcedureExportDto> GetTestProcedures(string version)
    {
        if (version == "old")
        {
            return new List<ProcedureExportDto>
            {
                new ProcedureExportDto
                {
                    Date = "10.06.2022",
                    ProcedureCode = "JGP.F03",
                    ProcedureName = "Appendektomia",
                    Location = "Blok operacyjny",
                    ModuleName = "Podstawowy",
                    PatientInitials = "JK",
                    PatientGender = "M",
                    YearOfTraining = "1",
                    InternshipName = "Szpital Uniwersytecki - Oddział Chirurgii",
                    FirstAssistantData = "dr Anna Nowak, PWZ: 8765432",
                    SecondAssistantData = "",
                    Role = "A"
                }
            };
        }
        else
        {
            return new List<ProcedureExportDto>
            {
                new ProcedureExportDto
                {
                    Date = "10.06.2022",
                    ProcedureCode = "JGP.F03",
                    ProcedureName = "Appendektomia",
                    Location = "Blok operacyjny",
                    ModuleName = "Podstawowy",
                    CountA = 1,
                    CountB = 0,
                    Supervisor = "dr Piotr Wiśniewski, PWZ: 9876543"
                }
            };
        }
    }
}