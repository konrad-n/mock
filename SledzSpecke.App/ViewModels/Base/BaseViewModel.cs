using CommunityToolkit.Mvvm.ComponentModel;
using SledzSpecke.App.Services.Exceptions;

namespace SledzSpecke.App.ViewModels.Base
{
    public class BaseViewModel : ObservableObject
    {
        private bool isBusy;
        private string title;

        protected readonly IExceptionHandlerService ExceptionHandler;

        public BaseViewModel(IExceptionHandlerService exceptionHandler = null)
        {
            ExceptionHandler = exceptionHandler;
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set => this.SetProperty(ref this.isBusy, value);
        }

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację z automatyczną obsługą błędów
        /// </summary>
        protected async Task SafeExecuteAsync(Func<Task> operation, string userFriendlyMessage = null)
        {
            if (ExceptionHandler != null)
            {
                await ExceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację zwracającą wartość z automatyczną obsługą błędów
        /// </summary>
        protected async Task<T> SafeExecuteAsync<T>(Func<Task<T>> operation, string userFriendlyMessage = null)
        {
            if (ExceptionHandler != null)
            {
                return await ExceptionHandler.ExecuteAsync(operation, null, userFriendlyMessage);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteAsync)}: {ex.Message}");
                    return default;
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację z ponawianiem i automatyczną obsługą błędów
        /// </summary>
        protected async Task SafeExecuteWithRetryAsync(Func<Task> operation, string userFriendlyMessage = null,
                                                    int retryCount = 3, int delayMilliseconds = 500)
        {
            if (ExceptionHandler != null)
            {
                await ExceptionHandler.ExecuteWithRetryAsync(operation, null, userFriendlyMessage,
                                                           retryCount, delayMilliseconds);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteWithRetryAsync)}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Bezpiecznie wykonuje operację zwracającą wartość z ponawianiem i automatyczną obsługą błędów
        /// </summary>
        protected async Task<T> SafeExecuteWithRetryAsync<T>(Func<Task<T>> operation, string userFriendlyMessage = null,
                                                          int retryCount = 3, int delayMilliseconds = 500)
        {
            if (ExceptionHandler != null)
            {
                return await ExceptionHandler.ExecuteWithRetryAsync(operation, null, userFriendlyMessage,
                                                                  retryCount, delayMilliseconds);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception in {nameof(SafeExecuteWithRetryAsync)}: {ex.Message}");
                    return default;
                }
            }
        }
    }
}