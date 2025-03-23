using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class NewSMKInternshipsListPage : ContentPage
    {
        private readonly NewSMKInternshipsListViewModel viewModel;

        public NewSMKInternshipsListPage(NewSMKInternshipsListViewModel viewModel)
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