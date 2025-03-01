﻿using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Internships.ViewModels;

namespace SledzSpecke.App.Features.Internships.Views
{
    public partial class InternshipsPage : BaseContentPage
    {
        private InternshipsViewModel viewModel;

        public InternshipsPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<InternshipsViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować danych staży.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in InternshipsPage: {ex}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh data when page appears
            if (this.viewModel != null)
            {
                this.viewModel.LoadDataAsync().ConfigureAwait(false);
            }
        }
    }
}