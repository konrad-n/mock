using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Courses.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.Views
{
    public partial class CourseDetailsPage : BaseContentPage
    {
        private readonly Course course;
        private readonly ModuleType currentModule;
        private readonly Func<Course, Task> onSaveCallback;
        private CourseDetailsViewModel viewModel;

        public CourseDetailsPage(Course course, ModuleType currentModule, Func<Course, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this.course = course;
            this.currentModule = currentModule;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<CourseDetailsViewModel>();
                this.viewModel.Initialize(this.course, this.currentModule, this.onSaveCallback);
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad", "Nie udalo sie zainicjalizowac strony szczególów kursu.", "OK");
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
