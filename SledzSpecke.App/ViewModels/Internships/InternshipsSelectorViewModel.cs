using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipsSelectorViewModel : BaseViewModel
    {
        public InternshipsSelectorViewModel(IExceptionHandlerService exceptionHandler = null) : base(exceptionHandler)
        {
            this.Title = "Staże";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKInternships");
            }, "Nie udało się przejść do okna staży (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKInternships");
            }, "Nie udało się przejść do okna staży (Nowy SMK).");
        }
    }
}
