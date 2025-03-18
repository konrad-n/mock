using OfficeOpenXml;
using OfficeOpenXml.Style;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Helpers
{
    public static class ExcelHelper
    {
        public static void Initialize()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static ExcelWorksheet AddSummaryWorksheet(ExcelPackage package, Specialization specialization, ExportOptions options)
        {
            var worksheet = package.Workbook.Worksheets.Add("Podsumowanie");

            worksheet.Cells[1, 1].Value = "PODSUMOWANIE DANYCH SPECJALIZACJI";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;

            worksheet.Cells[3, 1].Value = "Nazwa specjalizacji:";
            worksheet.Cells[3, 2].Value = specialization.Name;

            worksheet.Cells[4, 1].Value = "Data rozpoczęcia:";
            worksheet.Cells[4, 2].Value = specialization.StartDate;
            worksheet.Cells[4, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[5, 1].Value = "Planowana data zakończenia:";
            worksheet.Cells[5, 2].Value = specialization.PlannedEndDate;
            worksheet.Cells[5, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[6, 1].Value = "Obliczona data zakończenia:";
            worksheet.Cells[6, 2].Value = specialization.CalculatedEndDate;
            worksheet.Cells[6, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[8, 1].Value = "Zakres eksportowanych danych:";
            worksheet.Cells[8, 1].Style.Font.Bold = true;

            worksheet.Cells[9, 1].Value = "Od:";
            worksheet.Cells[9, 2].Value = options.StartDate;
            worksheet.Cells[9, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[10, 1].Value = "Do:";
            worksheet.Cells[10, 2].Value = options.EndDate;
            worksheet.Cells[10, 2].Style.Numberformat.Format = "yyyy-MM-dd";

            worksheet.Cells[12, 1].Value = "Eksportowane kategorie:";
            worksheet.Cells[12, 1].Style.Font.Bold = true;

            int row = 13;

            if (options.IncludeShifts)
            {
                worksheet.Cells[row++, 1].Value = "- Dyżury medyczne";
            }

            if (options.IncludeProcedures)
            {
                worksheet.Cells[row++, 1].Value = "- Zabiegi i procedury";
            }

            if (options.IncludeInternships)
            {
                worksheet.Cells[row++, 1].Value = "- Staże kierunkowe";
            }

            if (options.IncludeCourses)
            {
                worksheet.Cells[row++, 1].Value = "- Kursy specjalizacyjne";
            }

            if (options.IncludeSelfEducation)
            {
                worksheet.Cells[row++, 1].Value = "- Samokształcenie";
            }

            if (options.IncludePublications)
            {
                worksheet.Cells[row++, 1].Value = "- Publikacje";
            }

            if (options.IncludeAbsences)
            {
                worksheet.Cells[row++, 1].Value = "- Nieobecności";
            }

            if (options.IncludeEducationalActivities)
            {
                worksheet.Cells[row++, 1].Value = "- Działalność edukacyjna";
            }

            if (options.IncludeRecognitions)
            {
                worksheet.Cells[row++, 1].Value = "- Uznania/skrócenia";
            }

            worksheet.Cells[row + 2, 1].Value = "Data wygenerowania:";
            worksheet.Cells[row + 2, 2].Value = DateTime.Now;
            worksheet.Cells[row + 2, 2].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

            worksheet.Cells[row + 3, 1].Value = "Format:";
            worksheet.Cells[row + 3, 2].Value = options.FormatForOldSMK ? "Stara wersja SMK" : "Nowa wersja SMK";

            worksheet.Columns[1, 2].AutoFit();

            return worksheet;
        }

        public static ExcelWorksheet AddMedicalShiftsWorksheet(ExcelPackage package, List<MedicalShift> shifts, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Dyżury medyczne");

            int col = 1;
            worksheet.Cells[1, col++].Value = "Data";
            worksheet.Cells[1, col++].Value = "Godziny";
            worksheet.Cells[1, col++].Value = "Minuty";
            worksheet.Cells[1, col++].Value = "Miejsce";
            worksheet.Cells[1, col++].Value = "Rok szkolenia";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Pole dodatkowe 1";
                worksheet.Cells[1, col++].Value = "Pole dodatkowe 2";
            }

            for (int i = 1; i <= col - 1; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            for (int i = 0; i < shifts.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var shift = shifts[i];

                worksheet.Cells[row, col++].Value = shift.Date;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = shift.Hours;
                worksheet.Cells[row, col++].Value = shift.Minutes;
                worksheet.Cells[row, col++].Value = shift.Location;
                worksheet.Cells[row, col++].Value = shift.Year;

                if (oldSmkFormat && !string.IsNullOrEmpty(shift.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);

                        if (additionalFields != null)
                        {
                            if (additionalFields.TryGetValue("Field1", out object field1))
                            {
                                worksheet.Cells[row, col++].Value = field1?.ToString();
                            }
                            else
                            {
                                col++;
                            }

                            if (additionalFields.TryGetValue("Field2", out object field2))
                            {
                                worksheet.Cells[row, col++].Value = field2?.ToString();
                            }
                            else
                            {
                                col++;
                            }
                        }
                    }
                    catch
                    {
                        col += 2;
                    }
                }
            }

            worksheet.Columns[1, col - 1].AutoFit();

            return worksheet;
        }

        public static ExcelWorksheet AddProceduresWorksheet(ExcelPackage package, List<Procedure> procedures, bool oldSmkFormat)
        {
            var worksheet = package.Workbook.Worksheets.Add("Zabiegi i procedury");

            int col = 1;
            worksheet.Cells[1, col++].Value = "Data";
            worksheet.Cells[1, col++].Value = "Kod zabiegu";
            worksheet.Cells[1, col++].Value = "Operator/Asysta";
            worksheet.Cells[1, col++].Value = "Miejsce wykonania";
            worksheet.Cells[1, col++].Value = "Nazwa stażu";
            worksheet.Cells[1, col++].Value = "Inicjały pacjenta";
            worksheet.Cells[1, col++].Value = "Płeć pacjenta";
            worksheet.Cells[1, col++].Value = "Dane asysty";
            worksheet.Cells[1, col++].Value = "Procedura z grupy";
            worksheet.Cells[1, col++].Value = "Status";

            if (oldSmkFormat)
            {
                worksheet.Cells[1, col++].Value = "Osoba wykonująca";
                worksheet.Cells[1, col++].Value = "Pole dodatkowe";
            }

            for (int i = 1; i <= col - 1; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            for (int i = 0; i < procedures.Count; i++)
            {
                int row = i + 2;
                col = 1;

                var procedure = procedures[i];

                worksheet.Cells[row, col++].Value = procedure.Date;
                worksheet.Cells[row, col - 1].Style.Numberformat.Format = "yyyy-MM-dd";

                worksheet.Cells[row, col++].Value = procedure.Code;
                worksheet.Cells[row, col++].Value = procedure.OperatorCode;
                worksheet.Cells[row, col++].Value = procedure.Location;

                string internshipName = string.Empty;
                if (!string.IsNullOrEmpty(procedure.AdditionalFields))
                {
                    var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(procedure.AdditionalFields);
                    if (additionalFields != null && additionalFields.TryGetValue("InternshipName", out object name))
                    {
                        internshipName = name?.ToString() ?? string.Empty;
                    }
                }

                worksheet.Cells[row, col++].Value = internshipName;
                worksheet.Cells[row, col++].Value = procedure.PatientInitials;
                worksheet.Cells[row, col++].Value = procedure.PatientGender;
                worksheet.Cells[row, col++].Value = procedure.AssistantData;
                worksheet.Cells[row, col++].Value = procedure.ProcedureGroup;
                worksheet.Cells[row, col++].Value = procedure.Status;

                if (oldSmkFormat)
                {
                    worksheet.Cells[row, col++].Value = procedure.PerformingPerson;

                    if (!string.IsNullOrEmpty(procedure.AdditionalFields))
                    {
                        try
                        {
                            var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(procedure.AdditionalFields);

                            if (additionalFields != null && additionalFields.TryGetValue("ExtraField", out object extraField))
                            {
                                worksheet.Cells[row, col++].Value = extraField?.ToString();
                            }
                            else
                            {
                                col++;
                            }
                        }
                        catch
                        {
                            col++;
                        }
                    }
                    else
                    {
                        col++;
                    }
                }
            }

            worksheet.Columns[1, col - 1].AutoFit();

            return worksheet;
        }
    }
}