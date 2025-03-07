using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class MedicalShiftsSelectorViewModel : BaseViewModel
    {
        public MedicalShiftsSelectorViewModel()
        {
            this.Title = "Dyżury medyczne";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await Shell.Current.GoToAsync("OldSMKMedicalShifts");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await Shell.Current.GoToAsync("NewSMKMedicalShifts");
        }
    }
}