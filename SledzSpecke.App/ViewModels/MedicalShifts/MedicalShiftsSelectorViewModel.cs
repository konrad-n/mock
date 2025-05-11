using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class MedicalShiftsSelectorViewModel : BaseViewModel
    {
        public MedicalShiftsSelectorViewModel(IExceptionHandlerService exceptionHandler = null) : base(exceptionHandler)
        {
            this.Title = "Dyżury medyczne";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("OldSMKMedicalShifts");
            }, "Wystąpił problem podczas nawigacji do dyżurów (Stary SMK).");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("NewSMKMedicalShifts");
            }, "Wystąpił problem podczas nawigacji do dyżurów (Nowy SMK).");
        }
    }
}
