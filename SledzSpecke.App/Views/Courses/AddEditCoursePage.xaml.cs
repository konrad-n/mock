using SledzSpecke.App.ViewModels.Courses;

namespace SledzSpecke.App.Views.Courses
{
    public partial class AddEditCoursePage : ContentPage
    {
        private readonly AddEditCourseViewModel viewModel;

        public AddEditCoursePage(AddEditCourseViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Można tu dodać dodatkową logikę inicjalizacyjną jeśli potrzeba
        }
    }
}