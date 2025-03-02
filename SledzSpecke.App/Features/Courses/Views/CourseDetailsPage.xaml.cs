using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.Views
{
    public partial class CourseDetailsPage : BaseContentPage
    {
        private readonly Course _course;
        private readonly ModuleType _currentModule;
        private readonly Func<Course, Task> _onSaveCallback;
        private CourseDetailsViewModel viewModel;

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
                this.viewModel = this.GetRequiredService<CourseDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                this.viewModel.Initialize(this._course, this._currentModule, this._onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów kursu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in CourseDetailsPage: {ex}");
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Course != null)
            {
                this.viewModel.UpdateModuleTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Course != null)
            {
                this.viewModel.UpdateStatusCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}