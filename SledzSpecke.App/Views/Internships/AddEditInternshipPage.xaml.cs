using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.Views.Internships
{
    public partial class AddEditInternshipPage : ContentPage
    {
        private readonly AddEditInternshipViewModel viewModel;

        public AddEditInternshipPage(AddEditInternshipViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Tutaj można dodać dodatkową logikę inicjalizacyjną, jeśli jest potrzebna
        }
    }
}