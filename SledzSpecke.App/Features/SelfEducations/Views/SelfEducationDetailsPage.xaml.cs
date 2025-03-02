using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SelfEducations.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.SelfEducations.Views
{
    public partial class SelfEducationDetailsPage : BaseContentPage
    {
        private readonly SelfEducation selfEducation;
        private readonly Action<SelfEducation> onSaveCallback;
        private SelfEducationDetailsViewModel viewModel;

        public SelfEducationDetailsPage(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            this.InitializeComponent();
            this.selfEducation = selfEducation;
            this.onSaveCallback = onSaveCallback;
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<SelfEducationDetailsViewModel>();

                // Najpierw ViewModel
                this.viewModel.Initialize(this.selfEducation, this.onSaveCallback);

                // Dopiero potem BindingContext
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów wydarzenia.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SelfEducationDetailsPage: {ex}");
            }
        }

        private void OnTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel != null && this.viewModel.SelfEducation != null)
            {
                this.viewModel.UpdateTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (this.viewModel != null && this.viewModel.SelfEducation != null)
            {
                this.viewModel.UpdateDateCommand.Execute(null);
            }
        }
    }
}