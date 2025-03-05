using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Specializations;

namespace SledzSpecke.App.Views.Specialization
{
    public partial class InitializeSpecializationPage : ContentPage
    {
        private readonly InitializeSpecializationViewModel viewModel;

        public InitializeSpecializationPage(InitializeSpecializationViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        private void OnSmkVersionChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked && radioButton.Value != null)
            {
                if (radioButton.Value is SmkVersion version)
                {
                    this.viewModel.SmkVersion = version;
                }
            }
        }
    }
}