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
        /// Safely executes an operation with automatic error handling
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
        /// Safely executes an operation that returns a value with automatic error handling
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
    }
}
