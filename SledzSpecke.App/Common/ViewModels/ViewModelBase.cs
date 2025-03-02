using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace SledzSpecke.App.Common.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        protected readonly ILogger logger;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title;

        protected ViewModelBase(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
