using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.SMKExport.ViewModels;

namespace SledzSpecke.App.Features.SMKExport.Views
{
    public partial class SMKExportPage : BaseContentPage
    {
        private SMKExportViewModel viewModel;

        public SMKExportPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<SMKExportViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony eksportu SMK.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SMKExportPage: {ex}");
            }
        }

        private void OnExportTypeChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked)
            {
                int exportTypeIndex = 0;

                if (rb.BindingContext is SMKExportViewModel vm)
                {
                    if (vm.IsProcedureExportSelected)
                        exportTypeIndex = 1;
                    else if (vm.IsDutyShiftExportSelected)
                        exportTypeIndex = 2;
                }
                else
                {
                    // Fallback for binding issues
                    if (rb.Parent is Grid grid && grid.Children.IndexOf(rb) == 0)
                    {
                        VerticalStackLayout vsl = grid.Children.FirstOrDefault(c => c is VerticalStackLayout) as VerticalStackLayout;
                        if (vsl != null)
                        {
                            Label label = vsl.Children.FirstOrDefault(c => c is Label) as Label;
                            if (label != null)
                            {
                                string text = label.Text?.ToLowerInvariant() ?? string.Empty;
                                if (text.Contains("procedur"))
                                    exportTypeIndex = 1;
                                else if (text.Contains("dyżur"))
                                    exportTypeIndex = 2;
                            }
                        }
                    }
                }

                this.viewModel.ChangeExportTypeCommand.Execute(exportTypeIndex);
            }
        }

        private void OnCustomDatesCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                this.viewModel.ToggleCustomDatesCommand.Execute(rb.IsChecked);
            }
        }
    }
}