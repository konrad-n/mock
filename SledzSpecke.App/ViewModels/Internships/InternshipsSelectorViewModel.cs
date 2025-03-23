using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipsSelectorViewModel : BaseViewModel
    {
        public InternshipsSelectorViewModel()
        {
            this.Title = "Staże";

            this.NavigateToOldSMKCommand = new AsyncRelayCommand(this.NavigateToOldSMKAsync);
            this.NavigateToNewSMKCommand = new AsyncRelayCommand(this.NavigateToNewSMKAsync);
        }

        public ICommand NavigateToOldSMKCommand { get; }
        public ICommand NavigateToNewSMKCommand { get; }

        private async Task NavigateToOldSMKAsync()
        {
            await Shell.Current.GoToAsync("OldSMKInternships");
        }

        private async Task NavigateToNewSMKAsync()
        {
            await Shell.Current.GoToAsync("NewSMKInternships");
        }
    }
}