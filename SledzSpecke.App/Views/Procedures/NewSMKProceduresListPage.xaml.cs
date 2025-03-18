using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures
{
    public partial class NewSMKProceduresListPage : ContentPage
    {
        private readonly NewSMKProceduresListViewModel viewModel;
        private readonly IProcedureService procedureService;

        public NewSMKProceduresListPage(NewSMKProceduresListViewModel viewModel, IProcedureService procedureService)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.procedureService = procedureService;
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
            this.viewModel.RefreshCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (this.viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}