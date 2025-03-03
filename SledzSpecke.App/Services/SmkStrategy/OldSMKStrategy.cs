using System.Text.Json;

namespace SledzSpecke.App.Services.SmkStrategy
{
    public class OldSmkStrategy : ISmkVersionStrategy
    {
        public Dictionary<string, bool> GetVisibleFields(string viewName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    return new Dictionary<string, bool>
                    {
                        { "Date", true },
                        { "Hours", true },
                        { "Minutes", true },
                        { "Location", true },
                        { "Year", true },
                        { "OldSMKField1", true },
                        { "OldSMKField2", true },
                    };

                case "AddEditProcedure":
                    return new Dictionary<string, bool>
                    {
                        { "Date", true },
                        { "Code", true },
                        { "OperatorCode", true },
                        { "PatientInitials", true },
                        { "PatientGender", true },
                        { "Location", true },
                        { "AssistantData", false },
                        { "ProcedureGroup", true },
                        { "Status", true },
                        { "PerformingPerson", true },
                    };

                case "AddEditInternship":
                    return new Dictionary<string, bool>
                    {
                        { "InstitutionName", true },
                        { "DepartmentName", true },
                        { "InternshipName", true },
                        { "StartDate", true },
                        { "EndDate", true },
                        { "Year", true },
                        { "IsCompleted", true },
                        { "IsApproved", true },
                        { "OldSMKField1", true },
                    };

                default:
                    return new Dictionary<string, bool>();
            }
        }

        public Dictionary<string, string> GetFieldLabels(string viewName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    return new Dictionary<string, string>
                    {
                        { "Date", "Data dyżuru" },
                        { "Hours", "Godziny" },
                        { "Minutes", "Minuty" },
                        { "Location", "Miejsce dyżuru" },
                        { "Year", "Rok szkolenia" },
                        { "OldSMKField1", "Osoba nadzorująca" },
                        { "OldSMKField2", "Oddział" },
                    };

                case "AddEditProcedure":
                    return new Dictionary<string, string>
                    {
                        { "Date", "Data wykonania" },
                        { "Code", "Kod zabiegu" },
                        { "OperatorCode", "Operator/Asysta" },
                        { "PatientInitials", "Inicjały pacjenta" },
                        { "PatientGender", "Płeć pacjenta" },
                        { "Location", "Miejsce wykonania" },
                        { "ProcedureGroup", "Grupa procedur" },
                        { "Status", "Status" },
                        { "PerformingPerson", "Osoba wykonująca" },
                    };

                case "AddEditInternship":
                    return new Dictionary<string, string>
                    {
                        { "InstitutionName", "Nazwa instytucji" },
                        { "DepartmentName", "Nazwa oddziału" },
                        { "InternshipName", "Nazwa stażu" },
                        { "StartDate", "Data rozpoczęcia" },
                        { "EndDate", "Data zakończenia" },
                        { "Year", "Rok szkolenia" },
                        { "IsCompleted", "Ukończony" },
                        { "IsApproved", "Zatwierdzony" },
                        { "OldSMKField1", "Kierownik stażu" },
                    };

                default:
                    return new Dictionary<string, string>();
            }
        }

        public List<string> GetRequiredFields(string viewName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    return new List<string>
                    {
                        "Date",
                        "Hours",
                        "Location",
                        "OldSMKField1",  // Wymagane w starej wersji SMK
                        "OldSMKField2",  // Wymagane w starej wersji SMK
                    };

                case "AddEditProcedure":
                    return new List<string>
                    {
                        "Date",
                        "Code",
                        "OperatorCode",
                        "Location",
                        "PerformingPerson", // Wymagane w starej wersji SMK
                    };

                case "AddEditInternship":
                    return new List<string>
                    {
                        "InstitutionName",
                        "InternshipName",
                        "StartDate",
                        "EndDate",
                        "OldSMKField1",    // Wymagane w starej wersji SMK
                    };

                default:
                    return new List<string>();
            }
        }

        public Dictionary<string, object> GetDefaultValues(string viewName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    return new Dictionary<string, object>
                    {
                        { "Date", DateTime.Today },
                        { "Hours", 10 },
                        { "Minutes", 0 },
                        { "Year", 1 },
                        { "OldSMKField1", string.Empty },
                        { "OldSMKField2", string.Empty },
                    };

                case "AddEditProcedure":
                    return new Dictionary<string, object>
                    {
                        { "Date", DateTime.Today },
                        { "OperatorCode", "A" },
                        { "Status", "Zatwierdzona" },
                        { "PerformingPerson", string.Empty },
                    };

                case "AddEditInternship":
                    return new Dictionary<string, object>
                    {
                        { "StartDate", DateTime.Today },
                        { "EndDate", DateTime.Today.AddMonths(3) },
                        { "Year", 1 },
                        { "IsCompleted", false },
                        { "IsApproved", false },
                        { "OldSMKField1", string.Empty },
                    };

                default:
                    return new Dictionary<string, object>();
            }
        }

        public string FormatAdditionalFields(Dictionary<string, object> fields)
        {
            // Serializacja do JSON dla starej wersji SMK
            return JsonSerializer.Serialize(fields);
        }

        public Dictionary<string, object> ParseAdditionalFields(string json)
        {
            // Deserializacja z JSON dla starej wersji SMK
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>();
            }

            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }

        public bool IsFieldSupported(string viewName, string fieldName)
        {
            var visibleFields = this.GetVisibleFields(viewName);
            return visibleFields.ContainsKey(fieldName) && visibleFields[fieldName];
        }

        public string GetValidationMessage(string viewName, string fieldName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    switch (fieldName)
                    {
                        case "Date":
                            return "Data dyżuru jest wymagana";
                        case "Hours":
                            return "Liczba godzin musi być większa od 0";
                        case "Location":
                            return "Miejsce dyżuru jest wymagane";
                        case "OldSMKField1":
                            return "Osoba nadzorująca jest wymagana";
                        case "OldSMKField2":
                            return "Oddział jest wymagany";
                        default:
                            return $"Pole {fieldName} jest nieprawidłowe";
                    }

                case "AddEditProcedure":
                    switch (fieldName)
                    {
                        case "Date":
                            return "Data wykonania jest wymagana";
                        case "Code":
                            return "Kod zabiegu jest wymagany";
                        case "OperatorCode":
                            return "Operator/Asysta jest wymagane";
                        case "Location":
                            return "Miejsce wykonania jest wymagane";
                        case "PerformingPerson":
                            return "Osoba wykonująca jest wymagana";
                        default:
                            return $"Pole {fieldName} jest nieprawidłowe";
                    }

                default:
                    return $"Pole {fieldName} jest nieprawidłowe";
            }
        }

        public string GetViewTitle(string viewName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    return "Dodaj/Edytuj dyżur medyczny (Stary SMK)";
                case "AddEditProcedure":
                    return "Dodaj/Edytuj procedurę (Stary SMK)";
                case "AddEditInternship":
                    return "Dodaj/Edytuj staż (Stary SMK)";
                case "AddEditCourse":
                    return "Dodaj/Edytuj kurs (Stary SMK)";
                case "AddEditSelfEducation":
                    return "Dodaj/Edytuj samokształcenie (Stary SMK)";
                case "AddEditPublication":
                    return "Dodaj/Edytuj publikację (Stary SMK)";
                case "AddEditAbsence":
                    return "Dodaj/Edytuj nieobecność (Stary SMK)";
                default:
                    return $"{viewName} (Stary SMK)";
            }
        }

        public Dictionary<string, string> GetPickerOptions(string viewName, string fieldName)
        {
            switch (viewName)
            {
                case "AddEditMedicalShift":
                    switch (fieldName)
                    {
                        case "Year":
                            return new Dictionary<string, string>
                            {
                                { "1", "Rok 1" },
                                { "2", "Rok 2" },
                                { "3", "Rok 3" },
                                { "4", "Rok 4" },
                                { "5", "Rok 5" },
                            };
                        default:
                            return new Dictionary<string, string>();
                    }

                case "AddEditProcedure":
                    switch (fieldName)
                    {
                        case "OperatorCode":
                            return new Dictionary<string, string>
                            {
                                { "A", "Operator (A)" },
                                { "B", "Asysta (B)" },
                            };
                        case "PatientGender":
                            return new Dictionary<string, string>
                            {
                                { "M", "Mężczyzna" },
                                { "K", "Kobieta" },
                            };
                        case "Status":
                            return new Dictionary<string, string>
                            {
                                { "Zatwierdzona", "Zatwierdzona" },
                                { "Niezatwierdzona", "Niezatwierdzona" },
                                { "W trakcie", "W trakcie" },
                                { "Odrzucona", "Odrzucona" },
                            };
                        default:
                            return new Dictionary<string, string>();
                    }

                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}