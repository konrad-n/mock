namespace SledzSpecke.App.Services.SmkStrategy
{
    public class NewSmkStrategy: ISmkVersionStrategy
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

                // Inne przypadki...
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

                // Inne przypadki...
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

                // Inne przypadki...
                default:
                    return new List<string>();
            }
        }

        public Dictionary<string, object> GetDefaultValues(string viewName)
        {
            // Implementacja
            return new Dictionary<string, object>();
        }

        public string FormatAdditionalFields(Dictionary<string, object> fields)
        {
            // Serializacja do JSON
            return System.Text.Json.JsonSerializer.Serialize(fields);
        }

        public Dictionary<string, object> ParseAdditionalFields(string json)
        {
            // Deserializacja z JSON
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>();
            }

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}
