using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Internships.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Internships.Views
{
    public partial class InternshipDetailsPage : BaseContentPage
    {
        private readonly Internship internship;
        private readonly ModuleType currentModule;
        private readonly Func<Internship, Task> onSaveCallback;
        private InternshipDetailsViewModel viewModel;

        public InternshipDetailsPage(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this.internship = internship;
            this.currentModule = currentModule;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<InternshipDetailsViewModel>();
                this.viewModel.Initialize(this.internship, this.currentModule, this.onSaveCallback);
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów stażu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in InternshipDetailsPage: {ex}");
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Internship != null)
            {
                this.viewModel.UpdateModuleTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.Internship != null)
            {
                this.viewModel.UpdateStatusCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}