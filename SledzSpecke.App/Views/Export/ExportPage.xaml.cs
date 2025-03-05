using SledzSpecke.App.ViewModels.Export;

namespace SledzSpecke.App.Views.Export
{
    public partial class ExportPage : ContentPage
    {
        private readonly ExportViewModel viewModel;

        public ExportPage(ExportViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }
    }
}