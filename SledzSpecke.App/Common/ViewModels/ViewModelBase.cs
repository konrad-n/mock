﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

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
            this._logger = logger;
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}