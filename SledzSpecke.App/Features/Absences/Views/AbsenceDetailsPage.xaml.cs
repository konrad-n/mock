// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceDetailsPage.xaml.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Strona szczegółów nieobecności.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Absences.ViewModels;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Absences.Views
{
    /// <summary>
    /// Strona szczegółów nieobecności.
    /// </summary>
    public partial class AbsenceDetailsPage : BaseContentPage
    {
        private readonly Absence absence;
        private readonly Action<Absence> onSaveCallback;
        private AbsenceDetailsViewModel viewModel = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsenceDetailsPage"/> class.
        /// </summary>
        /// <param name="absence">Nieobecność do edycji lub null dla nowej nieobecności.</param>
        /// <param name="onSaveCallback">Wywołanie zwrotne po zapisaniu nieobecności.</param>
        public AbsenceDetailsPage(
            Absence absence,
            Action<Absence> onSaveCallback)
        {
            this.InitializeComponent();
            this.absence = absence;
            this.onSaveCallback = onSaveCallback;
        }

        /// <summary>
        /// Inicjalizuje stronę asynchronicznie.
        /// </summary>
        /// <returns>Task reprezentujący operację asynchroniczną.</returns>
        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<AbsenceDetailsViewModel>();
                this.viewModel.Initialize(this.absence, this.onSaveCallback);
                this.BindingContext = this.viewModel;
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować strony szczegółów nieobecności.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in AbsenceDetailsPage: {ex}");
            }
        }

        /// <summary>
        /// Obsługuje zmianę typu nieobecności.
        /// </summary>
        /// <param name="sender">Obiekt źródłowy zdarzenia.</param>
        /// <param name="e">Argumenty zdarzenia.</param>
        private void OnAbsenceTypeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && this.viewModel is not null)
            {
                this.viewModel.UpdateAbsenceTypeCommand.Execute(picker.SelectedIndex);
            }
        }

        /// <summary>
        /// Obsługuje zmianę daty.
        /// </summary>
        /// <param name="sender">Obiekt źródłowy zdarzenia.</param>
        /// <param name="e">Argumenty zdarzenia.</param>
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (this.viewModel is not null)
            {
                this.viewModel.CalculateDuration();
            }
        }
    }
}