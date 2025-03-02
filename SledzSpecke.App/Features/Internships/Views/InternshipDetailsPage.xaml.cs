using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Internships.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Internships.Views
{
    public partial class InternshipDetailsPage : BaseContentPage
    {
        private InternshipDetailsViewModel viewModel;
        private readonly Internship _internship;
        private readonly ModuleType _currentModule;
        private readonly Func<Internship, Task> _onSaveCallback;

        public InternshipDetailsPage(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            this.InitializeComponent();
            this._internship = internship;
            this._currentModule = currentModule;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<InternshipDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                this.viewModel.Initialize(this._internship, this._currentModule, this._onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
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