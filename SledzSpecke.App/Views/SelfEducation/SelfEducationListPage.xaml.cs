using SledzSpecke.App.ViewModels.SelfEducation;

namespace SledzSpecke.App.Views.SelfEducation
{
    public partial class SelfEducationListPage : ContentPage
    {
        private readonly SelfEducationListViewModel viewModel;

        public SelfEducationListPage(SelfEducationListViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Dla ICommand używamy Execute, nie ExecuteAsync
                this.viewModel.RefreshCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas odświeżania listy: {ex.Message}");
            }
        }
    }
}