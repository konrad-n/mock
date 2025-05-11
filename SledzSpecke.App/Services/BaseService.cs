using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services
{
    public abstract class BaseService
    {
        protected readonly IExceptionHandlerService ExceptionHandler;
        protected readonly ILoggingService Logger;

        protected BaseService(IExceptionHandlerService exceptionHandler, ILoggingService logger)
        {
            ExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<T> SafeExecuteAsync<T>(
            Func<Task<T>> operation,
            string userFriendlyMessage = null,
            Dictionary<string, object> context = null,
            bool withRetry = false,
            int retryCount = 3,
            int delayMilliseconds = 500)
        {
            if (withRetry)
            {
                return await ExceptionHandler.ExecuteWithRetryAsync(
                    operation, context, userFriendlyMessage, retryCount, delayMilliseconds);
            }
            else
            {
                return await ExceptionHandler.ExecuteAsync(
                    operation, context, userFriendlyMessage);
            }
        }

        protected async Task SafeExecuteAsync(
            Func<Task> operation,
            string userFriendlyMessage = null,
            Dictionary<string, object> context = null,
            bool withRetry = false,
            int retryCount = 3,
            int delayMilliseconds = 500)
        {
            if (withRetry)
            {
                await ExceptionHandler.ExecuteWithRetryAsync(
                    operation, context, userFriendlyMessage, retryCount, delayMilliseconds);
            }
            else
            {
                await ExceptionHandler.ExecuteAsync(
                    operation, context, userFriendlyMessage);
            }
        }
    }
}
