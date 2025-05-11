namespace SledzSpecke.App.Services.Logging
{
    public interface ILoggingService
    {
        void LogInformation(string message, Dictionary<string, object> properties = null, string callerMemberName = "");
        void LogWarning(string message, Dictionary<string, object> properties = null, string callerMemberName = "");
        void LogError(System.Exception exception, string message, Dictionary<string, object> properties = null, string callerMemberName = "");
    }
}