using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.Views
{
    public partial class CourseDetailsPage : BaseContentPage
    {
        private CourseDetailsViewModel _viewModel;
        private Course _course;
        private ModuleType _currentModule;
        private Func<Course, Task> _onSaveCallback;

        public CourseDetailsPage(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            InitializeComponent();
            _course = course;
            _currentModule = currentModule;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<CourseDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                _viewModel.Initialize(_course, _currentModule, _onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów kursu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in CourseDetailsPage: {ex}");
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Course != null)
            {
                _viewModel.UpdateModuleTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Course != null)
            {
                _viewModel.UpdateStatusCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}