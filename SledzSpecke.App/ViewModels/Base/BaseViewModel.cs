using CommunityToolkit.Mvvm.ComponentModel;

namespace SledzSpecke.App.ViewModels.Base
{
    public class BaseViewModel : ObservableObject
    {
        public bool isBusy;
        private string title;

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
    }
}