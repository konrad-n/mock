using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationDetailsPage : BaseContentPage
    {
        private SelfEducationDetailsViewModel _viewModel;
        private readonly SelfEducation _selfEducation;
        private readonly Action<SelfEducation> _onSaveCallback;

        public SelfEducationDetailsPage(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            this.InitializeComponent();
            this._selfEducation = selfEducation;
            this._onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<SelfEducationDetailsViewModel>();
                // Najpierw inicjalizujemy ViewModel
                this._viewModel.Initialize(this._selfEducation, this._onSaveCallback);
                // Dopiero potem ustawiamy BindingContext
                this.BindingContext = this._viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów wydarzenia.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SelfEducationDetailsPage: {ex}");
            }
        }

        private void OnTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this._viewModel != null && this._viewModel.SelfEducation != null)
            {
                this._viewModel.UpdateTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (this._viewModel != null && this._viewModel.SelfEducation != null)
            {
                this._viewModel.UpdateDateCommand.Execute(null);
            }
        }
    }
}