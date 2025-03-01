using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Internships.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Internships.Views
{
    public partial class InternshipDetailsPage : BaseContentPage
    {
        private InternshipDetailsViewModel _viewModel;
        private Internship _internship;
        private ModuleType _currentModule;
        private Func<Internship, Task> _onSaveCallback;

        public InternshipDetailsPage(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            InitializeComponent();
            _internship = internship;
            _currentModule = currentModule;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<InternshipDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                _viewModel.Initialize(_internship, _currentModule, _onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów stażu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in InternshipDetailsPage: {ex}");
            }
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Internship != null)
            {
                _viewModel.UpdateModuleTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.Internship != null)
            {
                _viewModel.UpdateStatusCommand.Execute(picker.SelectedIndex);
            }
        }
    }
}