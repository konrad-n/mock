using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Export.DTO;

namespace SledzSpecke.Infrastructure.Export;

public sealed class SmkExcelGenerator : ISmkExcelGenerator
{
    private readonly ILogger<SmkExcelGenerator> _logger;

    public SmkExcelGenerator(ILogger<SmkExcelGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GenerateAsync(SpecializationExportDto data)
    {
        _logger.LogInformation("Generating SMK Excel export");

        using var workbook = new XLWorkbook();
        
        // Sheet 1: Basic Information
        GenerateBasicInfoSheet(workbook, data.BasicInfo);
        
        // Sheet 2: Internships
        GenerateInternshipsSheet(workbook, data.Internships);
        
        // Sheet 3: Courses
        GenerateCoursesSheet(workbook, data.Courses);
        
        // Sheet 4: Medical Shifts
        GenerateMedicalShiftsSheet(workbook, data.MedicalShifts);
        
        // Sheet 5: Procedures
        GenerateProceduresSheet(workbook, data.Procedures, data.BasicInfo.SmkVersion);
        
        // Sheet 6: Additional Self-Education Days
        GenerateSelfEducationSheet(workbook, data.AdditionalSelfEducationDays);

        // Save to byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var bytes = stream.ToArray();
        
        _logger.LogInformation("Successfully generated Excel export with {ByteCount} bytes", bytes.Length);
        return await Task.FromResult(bytes);
    }

    private void GenerateBasicInfoSheet(IXLWorkbook workbook, BasicInfoExportDto basicInfo)
    {
        var sheet = workbook.Worksheets.Add("Informacje podstawowe");
        
        // Set headers and values
        sheet.Cell(1, 1).Value = "Imię";
        sheet.Cell(1, 2).Value = basicInfo.FirstName;
        
        sheet.Cell(2, 1).Value = "Nazwisko";
        sheet.Cell(2, 2).Value = basicInfo.LastName;
        
        sheet.Cell(3, 1).Value = "Email";
        sheet.Cell(3, 2).Value = basicInfo.Email;
        
        sheet.Cell(4, 1).Value = "Telefon";
        sheet.Cell(4, 2).Value = basicInfo.PhoneNumber;
        
        sheet.Cell(5, 1).Value = "Nazwa specjalizacji";
        sheet.Cell(5, 2).Value = basicInfo.SpecializationName;
        
        sheet.Cell(6, 1).Value = "Wersja SMK";
        sheet.Cell(6, 2).Value = basicInfo.SmkVersion;
        
        sheet.Cell(7, 1).Value = "Wariant programu";
        sheet.Cell(7, 2).Value = basicInfo.ProgramVariant;
        
        sheet.Cell(8, 1).Value = "Planowany rok PES";
        sheet.Cell(8, 2).Value = basicInfo.PlannedPesYear;
        
        sheet.Cell(9, 1).Value = "Data rozpoczęcia specjalizacji";
        sheet.Cell(9, 2).Value = basicInfo.SpecializationStartDate;
        
        sheet.Cell(10, 1).Value = "Data zakończenia specjalizacji";
        sheet.Cell(10, 2).Value = basicInfo.SpecializationEndDate;
        
        sheet.Cell(11, 1).Value = "Aktualny moduł";
        sheet.Cell(11, 2).Value = basicInfo.CurrentModuleName;
        
        sheet.Cell(12, 1).Value = "Data rozpoczęcia modułu";
        sheet.Cell(12, 2).Value = basicInfo.CurrentModuleStartDate;
        
        sheet.Cell(13, 1).Value = "Adres korespondencyjny";
        sheet.Cell(13, 2).Value = basicInfo.CorrespondenceAddress;
        
        // Format the sheet
        sheet.Column(1).Width = 30;
        sheet.Column(2).Width = 50;
        sheet.Range("A1:A15").Style.Font.Bold = true;
        sheet.Range("A1:B15").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    }

    private void GenerateInternshipsSheet(IXLWorkbook workbook, List<InternshipExportDto> internships)
    {
        var sheet = workbook.Worksheets.Add("Staże");
        
        // Headers
        sheet.Cell(1, 1).Value = "Nazwa stażu";
        sheet.Cell(1, 2).Value = "Nazwa podmiotu";
        sheet.Cell(1, 3).Value = "Komórka organizacyjna";
        sheet.Cell(1, 4).Value = "Data rozpoczęcia";
        sheet.Cell(1, 5).Value = "Data zakończenia";
        sheet.Cell(1, 6).Value = "Czas trwania (dni)";
        sheet.Cell(1, 7).Value = "Kierownik stażu";
        sheet.Cell(1, 8).Value = "Moduł";
        sheet.Cell(1, 9).Value = "Status";
        
        // Data
        int row = 2;
        foreach (var internship in internships.OrderBy(i => i.StartDate))
        {
            sheet.Cell(row, 1).Value = internship.InternshipName;
            sheet.Cell(row, 2).Value = internship.InstitutionName;
            sheet.Cell(row, 3).Value = internship.DepartmentName;
            sheet.Cell(row, 4).Value = internship.StartDate;
            sheet.Cell(row, 5).Value = internship.EndDate;
            sheet.Cell(row, 6).Value = internship.DurationDays;
            sheet.Cell(row, 7).Value = internship.SupervisorName;
            sheet.Cell(row, 8).Value = internship.ModuleName;
            sheet.Cell(row, 9).Value = internship.Status;
            row++;
        }
        
        // Format
        FormatDataSheet(sheet, row - 1);
    }

    private void GenerateCoursesSheet(IXLWorkbook workbook, List<CourseExportDto> courses)
    {
        var sheet = workbook.Worksheets.Add("Kursy");
        
        // Headers
        sheet.Cell(1, 1).Value = "Nazwa kursu";
        sheet.Cell(1, 2).Value = "Numer kursu";
        sheet.Cell(1, 3).Value = "Organizator";
        sheet.Cell(1, 4).Value = "Data rozpoczęcia";
        sheet.Cell(1, 5).Value = "Data zakończenia";
        sheet.Cell(1, 6).Value = "Liczba godzin";
        sheet.Cell(1, 7).Value = "Typ kursu";
        sheet.Cell(1, 8).Value = "Moduł";
        sheet.Cell(1, 9).Value = "Numer certyfikatu";
        sheet.Cell(1, 10).Value = "Status";
        
        // Data
        int row = 2;
        foreach (var course in courses.OrderBy(c => c.StartDate))
        {
            sheet.Cell(row, 1).Value = course.CourseName;
            sheet.Cell(row, 2).Value = course.CourseNumber;
            sheet.Cell(row, 3).Value = course.Provider;
            sheet.Cell(row, 4).Value = course.StartDate;
            sheet.Cell(row, 5).Value = course.EndDate;
            sheet.Cell(row, 6).Value = course.CreditHours;
            sheet.Cell(row, 7).Value = course.CourseType;
            sheet.Cell(row, 8).Value = course.ModuleName;
            sheet.Cell(row, 9).Value = course.CertificateNumber;
            sheet.Cell(row, 10).Value = course.Status;
            row++;
        }
        
        // Format
        FormatDataSheet(sheet, row - 1);
    }

    private void GenerateMedicalShiftsSheet(IXLWorkbook workbook, List<MedicalShiftExportDto> shifts)
    {
        var sheet = workbook.Worksheets.Add("Dyżury");
        
        // Headers
        sheet.Cell(1, 1).Value = "Data";
        sheet.Cell(1, 2).Value = "Godzina rozpoczęcia";
        sheet.Cell(1, 3).Value = "Godzina zakończenia";
        sheet.Cell(1, 4).Value = "Czas trwania";
        sheet.Cell(1, 5).Value = "Miejsce";
        sheet.Cell(1, 6).Value = "Staż";
        sheet.Cell(1, 7).Value = "Moduł";
        sheet.Cell(1, 8).Value = "Nadzorujący";
        sheet.Cell(1, 9).Value = "Uwagi";
        
        // Data
        int row = 2;
        foreach (var shift in shifts.OrderBy(s => s.Date))
        {
            sheet.Cell(row, 1).Value = shift.Date;
            sheet.Cell(row, 2).Value = shift.StartTime;
            sheet.Cell(row, 3).Value = shift.EndTime;
            sheet.Cell(row, 4).Value = shift.Duration;
            sheet.Cell(row, 5).Value = shift.Location;
            sheet.Cell(row, 6).Value = shift.InternshipName;
            sheet.Cell(row, 7).Value = shift.ModuleName;
            sheet.Cell(row, 8).Value = shift.SupervisorName;
            sheet.Cell(row, 9).Value = shift.Notes;
            row++;
        }
        
        // Format
        FormatDataSheet(sheet, row - 1);
    }

    private void GenerateProceduresSheet(IXLWorkbook workbook, List<ProcedureExportDto> procedures, string smkVersion)
    {
        var sheet = workbook.Worksheets.Add("Procedury");
        
        if (smkVersion == "old")
        {
            // Headers for old SMK
            sheet.Cell(1, 1).Value = "Kod procedury";
            sheet.Cell(1, 2).Value = "Nazwa procedury";
            sheet.Cell(1, 3).Value = "Data";
            sheet.Cell(1, 4).Value = "Miejsce";
            sheet.Cell(1, 5).Value = "Inicjały pacjenta";
            sheet.Cell(1, 6).Value = "Płeć pacjenta";
            sheet.Cell(1, 7).Value = "Rok szkolenia";
            sheet.Cell(1, 8).Value = "Staż";
            sheet.Cell(1, 9).Value = "Pierwszy asystent";
            sheet.Cell(1, 10).Value = "Drugi asystent";
            sheet.Cell(1, 11).Value = "Rola";
            sheet.Cell(1, 12).Value = "Moduł";
            
            // Data
            int row = 2;
            foreach (var procedure in procedures.OrderBy(p => p.Date))
            {
                sheet.Cell(row, 1).Value = procedure.ProcedureCode;
                sheet.Cell(row, 2).Value = procedure.ProcedureName;
                sheet.Cell(row, 3).Value = procedure.Date;
                sheet.Cell(row, 4).Value = procedure.Location;
                sheet.Cell(row, 5).Value = procedure.PatientInitials;
                sheet.Cell(row, 6).Value = procedure.PatientGender;
                sheet.Cell(row, 7).Value = procedure.YearOfTraining;
                sheet.Cell(row, 8).Value = procedure.InternshipName;
                sheet.Cell(row, 9).Value = procedure.FirstAssistantData;
                sheet.Cell(row, 10).Value = procedure.SecondAssistantData;
                sheet.Cell(row, 11).Value = procedure.Role;
                sheet.Cell(row, 12).Value = procedure.ModuleName;
                row++;
            }
        }
        else // new SMK
        {
            // Headers for new SMK
            sheet.Cell(1, 1).Value = "ID wymagania";
            sheet.Cell(1, 2).Value = "Kod procedury";
            sheet.Cell(1, 3).Value = "Nazwa procedury";
            sheet.Cell(1, 4).Value = "Data";
            sheet.Cell(1, 5).Value = "Miejsce";
            sheet.Cell(1, 6).Value = "Liczba wykonanych (A)";
            sheet.Cell(1, 7).Value = "Liczba asystowanych (B)";
            sheet.Cell(1, 8).Value = "Nadzorujący";
            sheet.Cell(1, 9).Value = "Moduł";
            
            // Data
            int row = 2;
            foreach (var procedure in procedures.OrderBy(p => p.Date))
            {
                sheet.Cell(row, 1).Value = procedure.ProcedureRequirementId ?? 0;
                sheet.Cell(row, 2).Value = procedure.ProcedureCode;
                sheet.Cell(row, 3).Value = procedure.ProcedureName;
                sheet.Cell(row, 4).Value = procedure.Date;
                sheet.Cell(row, 5).Value = procedure.Location;
                sheet.Cell(row, 6).Value = procedure.CountA ?? 0;
                sheet.Cell(row, 7).Value = procedure.CountB ?? 0;
                sheet.Cell(row, 8).Value = procedure.Supervisor;
                sheet.Cell(row, 9).Value = procedure.ModuleName;
                row++;
            }
        }
        
        // Format
        FormatDataSheet(sheet, procedures.Count + 1);
    }

    private void GenerateSelfEducationSheet(IXLWorkbook workbook, List<AdditionalSelfEducationExportDto> selfEducationDays)
    {
        var sheet = workbook.Worksheets.Add("Samokształcenie dodatkowe");
        
        // Headers
        sheet.Cell(1, 1).Value = "Data rozpoczęcia";
        sheet.Cell(1, 2).Value = "Data zakończenia";
        sheet.Cell(1, 3).Value = "Liczba dni";
        sheet.Cell(1, 4).Value = "Cel";
        sheet.Cell(1, 5).Value = "Nazwa wydarzenia";
        sheet.Cell(1, 6).Value = "Moduł";
        sheet.Cell(1, 7).Value = "Staż";
        
        // Data
        int row = 2;
        foreach (var selfEdu in selfEducationDays.OrderBy(s => s.StartDate))
        {
            sheet.Cell(row, 1).Value = selfEdu.StartDate;
            sheet.Cell(row, 2).Value = selfEdu.EndDate;
            sheet.Cell(row, 3).Value = selfEdu.NumberOfDays;
            sheet.Cell(row, 4).Value = selfEdu.Purpose;
            sheet.Cell(row, 5).Value = selfEdu.EventName;
            sheet.Cell(row, 6).Value = selfEdu.ModuleName;
            sheet.Cell(row, 7).Value = selfEdu.InternshipName;
            row++;
        }
        
        // Format
        FormatDataSheet(sheet, row - 1);
    }

    private void FormatDataSheet(IXLWorksheet sheet, int lastRow)
    {
        if (lastRow < 1) return;
        
        // Header formatting
        var headerRange = sheet.Range(1, 1, 1, sheet.LastCellUsed().Address.ColumnNumber);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        
        // Data formatting
        if (lastRow > 1)
        {
            var dataRange = sheet.Range(2, 1, lastRow, sheet.LastCellUsed().Address.ColumnNumber);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }
        
        // Auto-fit columns
        sheet.Columns().AdjustToContents();
        
        // Set minimum column width
        foreach (var column in sheet.ColumnsUsed())
        {
            if (column.Width < 12)
                column.Width = 12;
        }
    }
}