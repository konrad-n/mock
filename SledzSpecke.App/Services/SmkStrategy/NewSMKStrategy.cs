using System.Text.Json;

namespace SledzSpecke.App.Services.SmkStrategy
{
    public class NewSmkStrategy : ISmkVersionStrategy
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
                        { "OldSMKField1", false },
                        { "OldSMKField2", false },
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
                        { "AssistantData", true },
                        { "ProcedureGroup", true },
                        { "Status", true },
                        { "PerformingPerson", false },
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
                    };

                case "AddEditProcedure":
                    return new Dictionary<string, string>
                    {
                        { "Date", "Data wykonania" },
                        { "Code", "Kod zabiegu" },
                        { "OperatorCode", "Rola" },
                        { "PatientInitials", "Inicjały pacjenta" },
                        { "PatientGender", "Płeć pacjenta" },
                        { "Location", "Miejsce wykonania" },
                        { "AssistantData", "Dane asysty" },
                        { "ProcedureGroup", "Grupa procedur" },
                        { "Status", "Status" },
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
                    return new List<string> { "Date", "Hours", "Location" };

                case "AddEditProcedure":
                    return new List<string> { "Date", "Code", "OperatorCode", "Location" };

                case "AddEditInternship":
                    return new List<string> { "InstitutionName", "InternshipName", "StartDate", "EndDate" };

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
                    };

                case "AddEditProcedure":
                    return new Dictionary<string, object>
                    {
                        { "Date", DateTime.Today },
                        { "OperatorCode", "A" },
                        { "Status", "Completed" },
                    };

                case "AddEditInternship":
                    return new Dictionary<string, object>
                    {
                        { "StartDate", DateTime.Today },
                        { "EndDate", DateTime.Today.AddMonths(3) },
                        { "Year", 1 },
                        { "IsCompleted", false },
                        { "IsApproved", false },
                    };

                default:
                    return new Dictionary<string, object>();
            }
        }

        public string FormatAdditionalFields(Dictionary<string, object> fields)
        {
            // Serializacja do JSON
            return JsonSerializer.Serialize(fields);
        }

        public Dictionary<string, object> ParseAdditionalFields(string json)
        {
            // Deserializacja z JSON
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
                            return "Rola (operator/asysta) jest wymagana";
                        case "Location":
                            return "Miejsce wykonania jest wymagane";
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
                    return "Dodaj/Edytuj dyżur medyczny";
                case "AddEditProcedure":
                    return "Dodaj/Edytuj procedurę";
                case "AddEditInternship":
                    return "Dodaj/Edytuj staż";
                case "AddEditCourse":
                    return "Dodaj/Edytuj kurs";
                case "AddEditSelfEducation":
                    return "Dodaj/Edytuj samokształcenie";
                case "AddEditPublication":
                    return "Dodaj/Edytuj publikację";
                case "AddEditAbsence":
                    return "Dodaj/Edytuj nieobecność";
                default:
                    return viewName;
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
                                { "Completed", "Ukończona" },
                                { "PartiallyCompleted", "Częściowo ukończona" },
                                { "Approved", "Zatwierdzona" },
                                { "NotApproved", "Niezatwierdzona" },
                                { "Pending", "Oczekująca" },
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