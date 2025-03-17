using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class AddEditNewSMKProcedurePage : ContentPage
    {
        private readonly AddEditNewSMKProcedureViewModel viewModel;

        public AddEditNewSMKProcedurePage(AddEditNewSMKProcedureViewModel viewModel)
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
                    await Shell.Current.GoToAsync("..");
                })
            });
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}