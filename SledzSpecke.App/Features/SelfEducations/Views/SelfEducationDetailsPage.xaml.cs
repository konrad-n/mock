using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationDetailsPage : BaseContentPage
    {
        private SelfEducationDetailsViewModel _viewModel;
        private SelfEducation _selfEducation;
        private Action<SelfEducation> _onSaveCallback;

        public SelfEducationDetailsPage(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            InitializeComponent();
            _selfEducation = selfEducation;
            _onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<SelfEducationDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                _viewModel.Initialize(_selfEducation, _onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów wydarzenia.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SelfEducationDetailsPage: {ex}");
            }
        }

        private void OnTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && _viewModel != null && _viewModel.SelfEducation != null)
            {
                _viewModel.UpdateTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (_viewModel != null && _viewModel.SelfEducation != null)
            {
                _viewModel.UpdateDateCommand.Execute(null);
            }
        }
    }
}