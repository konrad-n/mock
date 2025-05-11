using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Logging;
using System.Runtime.CompilerServices;

namespace SledzSpecke.App.Services.Exceptions
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _loggingService;

        public ExceptionHandlerService(
            IDialogService dialogService,
            ILoggingService loggingService)
        {
            _dialogService = dialogService;
            _loggingService = loggingService;
        }

        public async Task HandleExceptionAsync(System.Exception exception,
                                              [CallerMemberName] string callerMemberName = "")
        {
            string title = "Błąd";
            string message;
            Dictionary<string, object> logProperties = new Dictionary<string, object>();

            // Add caller info
            if (!string.IsNullOrEmpty(callerMemberName))
            {
                logProperties["CallerMethod"] = callerMemberName;
            }

            // Handling specific exception types
            if (exception is AppBaseException appException)
            {
                message = appException.UserFriendlyMessage;

                // Add exception details to log
                foreach (var detail in appException.ErrorDetails)
                {
                    logProperties[detail.Key] = detail.Value;
                }

                // Log with the original technical message
                _loggingService.LogError(exception, appException.Message, logProperties, callerMemberName);
            }
            else
            {
                // Default handling for non-app exceptions
                message = "Wystąpił nieoczekiwany błąd w aplikacji.";
                _loggingService.LogError(exception, exception.Message, logProperties, callerMemberName);
            }

            // Show user-friendly message
            await _dialogService.DisplayAlertAsync(title, message, "OK");
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation,
                                           Dictionary<string, object> contextInfo = null,
                                           string userFriendlyMessage = null,
                                           [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;

            try
            {
                // Log operation start if needed
                _loggingService.LogInformation($"Starting operation: {operationName}", contextInfo, operationName);

                // Execute operation
                var result = await operation();

                // Log operation end if needed
                _loggingService.LogInformation($"Completed operation: {operationName}", contextInfo, operationName);

                return result;
            }
            catch (System.Exception ex)
            {
                // Transform standard exceptions to app exceptions
                var appException = TransformException(ex, userFriendlyMessage, contextInfo);

                // Handle the exception
                await HandleExceptionAsync(appException, operationName);

                // Default value for the return type
                return default;
            }
        }

        public async Task ExecuteAsync(Func<Task> operation,
                                     Dictionary<string, object> contextInfo = null,
                                     string userFriendlyMessage = null,
                                     [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;

            try
            {
                // Log operation start if needed
                _loggingService.LogInformation($"Starting operation: {operationName}", contextInfo, operationName);

                // Execute operation
                await operation();

                // Log operation end if needed
                _loggingService.LogInformation($"Completed operation: {operationName}", contextInfo, operationName);
            }
            catch (System.Exception ex)
            {
                // Transform standard exceptions to app exceptions
                var appException = TransformException(ex, userFriendlyMessage, contextInfo);

                // Handle the exception
                await HandleExceptionAsync(appException, operationName);
            }
        }

        private AppBaseException TransformException(System.Exception exception,
                                                  string userFriendlyMessage,
                                                  Dictionary<string, object> contextInfo)
        {
            // Return if it's already an app exception
            if (exception is AppBaseException appEx)
            {
                return appEx;
            }

            // Transform common .NET exceptions to app exceptions
            if (exception is SQLite.SQLiteException)
            {
                return new DatabaseConnectionException(
                    $"SQLite error: {exception.Message}",
                    userFriendlyMessage ?? "Wystąpił problem z bazą danych.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            if (exception is KeyNotFoundException)
            {
                return new ResourceNotFoundException(
                    $"Resource not found: {exception.Message}",
                    userFriendlyMessage ?? "Nie znaleziono żądanego zasobu.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            if (exception is ArgumentException || exception is FormatException)
            {
                return new InvalidInputException(
                    $"Invalid input: {exception.Message}",
                    userFriendlyMessage ?? "Nieprawidłowe dane wejściowe.",
                    exception,
                    new Dictionary<string, object>(contextInfo));
            }

            // Default transformation
            return new DomainLogicException(
                exception.Message,
                userFriendlyMessage ?? "Wystąpił błąd w aplikacji.",
                exception,
                new Dictionary<string, object>(contextInfo));
        }
    }
}