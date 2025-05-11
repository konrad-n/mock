using SledzSpecke.App.Exceptions;

namespace SledzSpecke.App.Services.Exceptions
{
    public interface IExceptionHandlerService
    {
        Task HandleExceptionAsync(System.Exception exception, string callerMemberName = "");
        Task<T> ExecuteAsync<T>(Func<Task<T>> operation, Dictionary<string, object> contextInfo = null,
                               string userFriendlyMessage = null, string operationName = "");
        Task ExecuteAsync(Func<Task> operation, Dictionary<string, object> contextInfo = null,
                         string userFriendlyMessage = null, string operationName = "");
    }
}