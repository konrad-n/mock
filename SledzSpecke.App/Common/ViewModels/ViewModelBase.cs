using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace SledzSpecke.App.Common.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        protected readonly ILogger _logger;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title;

        protected ViewModelBase(ILogger logger)
        {
            _logger = logger;
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}