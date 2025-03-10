using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class NewSMKProceduresListPage : ContentPage
    {
        private readonly NewSMKProceduresListViewModel viewModel;

        public NewSMKProceduresListPage(NewSMKProceduresListViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.BindingContext = this.viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Ustaw właściwość BackButtonBehavior dla Shell'a
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

            // Odśwież dane przy każdym pokazaniu strony
            this.viewModel.RefreshCommand.Execute(null);
        }
    }
}