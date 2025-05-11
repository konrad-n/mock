using System.Runtime.CompilerServices;

namespace SledzSpecke.App.Services.Logging
{
    public class LoggingService : ILoggingService
    {
        public void LogInformation(string message, Dictionary<string, object> properties = null,
                                 [CallerMemberName] string callerMemberName = "")
        {
            // Simple debug logging - can be replaced with a more sophisticated logging system
            System.Diagnostics.Debug.WriteLine($"INFO [{callerMemberName}]: {message}");
            LogProperties(properties);
        }

        public void LogWarning(string message, Dictionary<string, object> properties = null,
                              [CallerMemberName] string callerMemberName = "")
        {
            System.Diagnostics.Debug.WriteLine($"WARNING [{callerMemberName}]: {message}");
            LogProperties(properties);
        }

        public void LogError(System.Exception exception, string message, Dictionary<string, object> properties = null,
                           [CallerMemberName] string callerMemberName = "")
        {
            System.Diagnostics.Debug.WriteLine($"ERROR [{callerMemberName}]: {message}");
            System.Diagnostics.Debug.WriteLine($"Exception: {exception.GetType().Name}: {exception.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");

            if (exception.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}");
            }

            LogProperties(properties);
        }

        private void LogProperties(Dictionary<string, object> properties)
        {
            if (properties != null && properties.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Context Properties:");
                foreach (var property in properties)
                {
                    System.Diagnostics.Debug.WriteLine($"  {property.Key}: {property.Value}");
                }
            }
        }
    }
}