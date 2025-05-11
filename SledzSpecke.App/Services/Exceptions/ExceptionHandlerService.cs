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
        private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);
        private readonly List<string> _recentErrors = new List<string>();
        private DateTime _lastErrorTime = DateTime.MinValue;

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

            // Sprawdź czy pokazywać dialogi czy nie
            await ShowErrorToUserAsync(title, message);
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

        // Nowa metoda do wykonywania operacji z ponawianiem
        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation,
                                                Dictionary<string, object> contextInfo = null,
                                                string userFriendlyMessage = null,
                                                int retryCount = 3,
                                                int delayMilliseconds = 500,
                                                [CallerMemberName] string operationName = "")
        {
            contextInfo ??= new Dictionary<string, object>();
            contextInfo["OperationName"] = operationName;
            contextInfo["RetryCount"] = retryCount;

            Exception lastException = null;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        _loggingService.LogInformation($"Retry attempt {i + 1} for operation: {operationName}", contextInfo, operationName);
                    }

                    return await operation();
                }
                catch (System.Exception ex)
                {
                    lastException = ex;

                    // Sprawdź czy wyjątek kwalifikuje się do ponowienia
                    if (!IsTransientException(ex) || i >= retryCount - 1)
                    {
                        // Ostatnia próba lub nie-tymczasowy wyjątek - nie próbuj ponownie
                        break;
                    }

                    // Loguj informację o ponowieniu
                    _loggingService.LogWarning($"Transient error occurred, retrying (attempt {i + 1}/{retryCount}): {ex.Message}",
                        contextInfo, operationName);

                    // Odczekaj przed ponowieniem (ze zwiększającym się opóźnieniem)
                    await Task.Delay(delayMilliseconds * (i + 1));
                }
            }

            // Jeśli dotarliśmy tutaj, to wszystkie próby się nie powiodły
            var appException = TransformException(lastException, userFriendlyMessage, contextInfo);
            await HandleExceptionAsync(appException, operationName);
            return default;
        }

        // Bez zwracanego wyniku
        public async Task ExecuteWithRetryAsync(Func<Task> operation,
                                          Dictionary<string, object> contextInfo = null,
                                          string userFriendlyMessage = null,
                                          int retryCount = 3,
                                          int delayMilliseconds = 500,
                                          [CallerMemberName] string operationName = "")
        {
            await ExecuteWithRetryAsync<object>(async () => {
                await operation();
                return null;
            }, contextInfo, userFriendlyMessage, retryCount, delayMilliseconds, operationName);
        }

        private bool IsTransientException(System.Exception ex)
        {
            // Check if it's a SQLite exception without referencing SQLite.Result enum
            return ex is SQLite.SQLiteException sqlEx &&
                   (sqlEx.Result == (SQLite.SQLite3.Result)5 ||   // Result.Busy
                    sqlEx.Result == (SQLite.SQLite3.Result)6 ||   // Result.Locked
                    sqlEx.Result == (SQLite.SQLite3.Result)19 ||  // Result.Constraint
                    sqlEx.Result == (SQLite.SQLite3.Result)10)    // Result.IOError
                || ex is System.Net.Http.HttpRequestException
                || ex is System.Net.WebException
                || ex is System.IO.IOException
                || ex is System.TimeoutException
                || (ex.Message?.Contains("network", StringComparison.OrdinalIgnoreCase) == true)
                || (ex.Message?.Contains("connection", StringComparison.OrdinalIgnoreCase) == true);
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

            if (exception is System.Net.WebException || exception is System.Net.Http.HttpRequestException)
            {
                return new NetworkException(
                    $"Network error: {exception.Message}",
                    userFriendlyMessage ?? "Wystąpił problem z połączeniem sieciowym. Sprawdź swoje połączenie i spróbuj ponownie.",
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

        // Metoda do pokazywania błędów użytkownikowi z ograniczeniem liczby dialogów
        private async Task ShowErrorToUserAsync(string title, string message)
        {
            try
            {
                // Próbujemy zdobyć semafor
                bool semaphoreAcquired = await _dialogSemaphore.WaitAsync(100);

                if (!semaphoreAcquired)
                {
                    // Nie udało się zdobyć semafora - możliwy wyścig lub inny dialog jest już pokazywany
                    _loggingService.LogWarning($"Dialog semaphore acquisition failed for error: {message}",
                        new Dictionary<string, object> { { "Title", title } });
                    return;
                }

                try
                {
                    // Oczyść starą listę błędów (starsze niż 5 sekund)
                    if ((DateTime.Now - _lastErrorTime).TotalSeconds > 5)
                    {
                        _recentErrors.Clear();
                    }

                    // Sprawdź czy ten sam błąd nie został już wyświetlony ostatnio
                    if (_recentErrors.Contains(message))
                    {
                        return;
                    }

                    // Dodaj do listy ostatnich błędów
                    _recentErrors.Add(message);
                    _lastErrorTime = DateTime.Now;

                    // Pokaż dialog
                    await _dialogService.DisplayAlertAsync(title, message, "OK");
                }
                finally
                {
                    // Zwolnij semafor
                    _dialogSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                // Coś poszło nie tak przy wyświetlaniu błędu, logujemy to, ale nie pokazujemy 
                // użytkownikowi (aby uniknąć pętli)
                _loggingService.LogError(ex, $"Error displaying error dialog: {ex.Message}",
                    new Dictionary<string, object> { { "OriginalMessage", message } });
            }
        }
    }
}