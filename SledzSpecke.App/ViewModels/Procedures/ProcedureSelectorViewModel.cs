using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureSelectorViewModel : BaseViewModel
    {
        private readonly IAuthService authService;

        public ProcedureSelectorViewModel(IAuthService authService)
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
            try
            {
                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();

                if (user != null)
                {
                    // Przekieruj automatycznie w zależności od wersji SMK
                    if (user.SmkVersion == SmkVersion.Old)
                    {
                        await Shell.Current.GoToAsync("OldSMKProcedures");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("NewSMKProcedures");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji: {ex.Message}");
            }
        }

        private async Task NavigateToOldSMKAsync()
        {
            await Shell.Current.GoToAsync("OldSMKProcedures");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await Shell.Current.GoToAsync("NewSMKProcedures");
        }
    }
}