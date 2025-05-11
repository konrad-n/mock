using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureSelectorViewModel : BaseViewModel
    {
        private readonly IAuthService authService;

        public ProcedureSelectorViewModel(
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.authService = authService;
            this.Title = "Procedury";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        public async Task InitializeAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();

                if (user != null)
                {
                    if (user.SmkVersion == SmkVersion.Old)
                    {
                        await Shell.Current.GoToAsync("OldSMKProcedures");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("NewSMKProcedures");
                    }
                }
            }, "Wystąpił problem podczas inicjalizacji widoku procedur.");
        }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKProcedures");
            }, "Wystąpił problem podczas nawigacji do widoku procedur (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKProcedures");
            }, "Wystąpił problem podczas nawigacji do widoku procedur (Nowy SMK).");
        }
    }
}
