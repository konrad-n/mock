namespace SledzSpecke.App.Services.SmkStrategy
{
    public interface ISmkVersionStrategy
    {
        Dictionary<string, bool> GetVisibleFields(string viewName);

        Dictionary<string, string> GetFieldLabels(string viewName);

        List<string> GetRequiredFields(string viewName);

        Dictionary<string, object> GetDefaultValues(string viewName);

        string FormatAdditionalFields(Dictionary<string, object> fields);

        Dictionary<string, object> ParseAdditionalFields(string json);
    }
}
