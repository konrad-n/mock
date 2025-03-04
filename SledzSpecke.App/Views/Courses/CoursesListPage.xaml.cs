using SledzSpecke.App.ViewModels.Courses;

namespace SledzSpecke.App.Views.Courses
{
    public partial class CoursesListPage : ContentPage
    {
        private readonly CoursesListViewModel viewModel;

        public CoursesListPage(CoursesListViewModel viewModel)
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
                // Odśwież listę kursów przy każdym wejściu na stronę
                this.viewModel.RefreshCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas odświeżania listy kursów: {ex.Message}");
            }
        }
    }
}