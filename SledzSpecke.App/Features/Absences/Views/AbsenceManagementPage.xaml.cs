// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceManagementPage.xaml.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Strona zarządzania nieobecnościami.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;

namespace SledzSpecke.App.Features.Absences.Views
{
    /// <summary>
    /// Strona zarządzania nieobecnościami.
    /// </summary>
    public partial class AbsenceManagementPage : BaseContentPage
    {
        private AbsenceManagementViewModel viewModel = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsenceManagementPage"/> class.
        /// </summary>
        public AbsenceManagementPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Inicjalizuje stronę asynchronicznie.
        /// </summary>
        /// <returns>Task reprezentujący operację asynchroniczną.</returns>
        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<AbsenceManagementViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceManagementPage: {ex}");
            }
        }

        /// <summary>
        /// Wywoływane przy pojawieniu się strony.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh data when page appears
            this.viewModel?.LoadDataAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Obsługuje zmianę filtra typu nieobecności.
        /// </summary>
        /// <param name="sender">Obiekt źródłowy zdarzenia.</param>
        /// <param name="e">Argumenty zdarzenia.</param>
        private void OnFilterTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this.viewModel.FilterByAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        /// <summary>
        /// Obsługuje zmianę filtra roku.
        /// </summary>
        /// <param name="sender">Obiekt źródłowy zdarzenia.</param>
        /// <param name="e">Argumenty zdarzenia.</param>
        private void OnFilterYearChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                this.viewModel.FilterByYearCommand.Execute(picker.SelectedIndex);
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && int.TryParse(button.ClassId, out int absenceId))
            {
                this.viewModel.EditAbsenceCommand.Execute(absenceId);
            }
        }
    }
}