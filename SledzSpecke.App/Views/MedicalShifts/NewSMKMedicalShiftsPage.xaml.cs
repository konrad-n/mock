using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    public partial class NewSMKMedicalShiftsPage : ContentPage
    {
        private readonly NewSMKMedicalShiftsListViewModel viewModel;

        public NewSMKMedicalShiftsPage(NewSMKMedicalShiftsListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    await Shell.Current.GoToAsync("///dashboard");
                })
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!this.viewModel.IsBusy)
            {
                this.viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}