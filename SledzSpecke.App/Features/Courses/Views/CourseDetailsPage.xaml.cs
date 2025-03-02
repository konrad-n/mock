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
            this.InitializeComponent();
            this._course = course;
            this._currentModule = currentModule;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<CourseDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                this._viewModel.Initialize(this._course, this._currentModule, this._onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                this.BindingContext = this._viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów kursu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in CourseDetailsPage: {ex}");
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.Course != null)
            {
                this._viewModel.UpdateModuleTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.Course != null)
            {
                this._viewModel.UpdateStatusCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}