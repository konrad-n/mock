﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Internships.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Internships.ViewModels
{
    public partial class InternshipsViewModel : ViewModelBase
    {
        private readonly ISpecializationService specializationService;

        [ObservableProperty]
        private Specialization specialization;

        [ObservableProperty]
        private ModuleType currentModule = ModuleType.Basic;

        [ObservableProperty]
        private ObservableCollection<Internship> internships;

        [ObservableProperty]
        private bool isNoInternshipsVisible;

        [ObservableProperty]
        private Color basicModuleButtonBackgroundColor = new Color(8, 32, 68);

        [ObservableProperty]
        private Color basicModuleButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color specialisticModuleButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color specialisticModuleButtonTextColor = Colors.Black;

        public InternshipsViewModel(
            ISpecializationService specializationService,
            ILogger<InternshipsViewModel> logger)
            : base(logger)
        {
            this.specializationService = specializationService;
            this.Internships = new ObservableCollection<Internship>();
            this.Title = "Staże";
        }

        public static bool IsCurrentInternship(Internship internship)
        {
            return internship.StartDate.HasValue && !internship.EndDate.HasValue;
        }

        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                base.logger.LogError(ex, "Error loading internships data");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                this.Specialization = await this.specializationService.GetSpecializationAsync();
                this.DisplayInternships(this.CurrentModule);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading specialization data");
                throw;
            }
        }

        public void DisplayInternships(ModuleType moduleType)
        {
            this.CurrentModule = moduleType;
            this.UpdateModuleButtons();

            var filteredInternships = this.Specialization?.RequiredInternships
                .Where(i => i.Module == moduleType)
                .OrderBy(i => i.IsCompleted)
                .ToList() ?? new List<Internship>();

            this.Internships = new ObservableCollection<Internship>(filteredInternships);
            this.IsNoInternshipsVisible = this.Internships.Count == 0;
        }

        public Color GetInternshipStatusColor(Internship internship)
        {
            if (internship.IsCompleted)
            {
                return Colors.Green;
            }

            if (IsCurrentInternship(internship))
            {
                return Colors.Blue;
            }

            if (internship.StartDate.HasValue)
            {
                return Colors.Orange;
            }

            return new Color(84, 126, 158);
        }

        public string GetInternshipStatusText(Internship internship)
        {
            if (internship.IsCompleted)
            {
                return "Ukończony";
            }

            if (IsCurrentInternship(internship))
            {
                return $"W trakcie od: {internship.StartDate?.ToString("dd.MM.yyyy")}";
            }

            if (internship.StartDate.HasValue)
            {
                return $"Zaplanowany na: {internship.StartDate?.ToString("dd.MM.yyyy")}";
            }

            return "Oczekujący";
        }

        private async Task OnInternshipAdded(Internship internship)
        {
            try
            {
                await this.specializationService.SaveInternshipAsync(internship);
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error adding internship");
                throw;
            }
        }

        [RelayCommand]
        private void SelectBasicModule()
        {
            this.DisplayInternships(ModuleType.Basic);
        }

        [RelayCommand]
        private void SelectSpecialisticModule()
        {
            this.DisplayInternships(ModuleType.Specialistic);
        }

        [RelayCommand]
        private async Task AddInternshipAsync()
        {
            await Shell.Current.Navigation.PushAsync(new InternshipDetailsPage(null, this.CurrentModule, this.OnInternshipAdded));
        }

        [RelayCommand]
        private async Task ViewInternshipDetailsAsync(int internshipId)
        {
            var internship = this.Specialization.RequiredInternships.FirstOrDefault(i => i.Id == internshipId);
            if (internship != null)
            {
                await Shell.Current.Navigation.PushAsync(new InternshipDetailsPage(internship, this.CurrentModule, this.OnInternshipUpdated));
            }
        }

        private async Task OnInternshipUpdated(Internship internship)
        {
            try
            {
                await this.specializationService.SaveInternshipAsync(internship);
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating internship");
                throw;
            }
        }

        private void UpdateModuleButtons()
        {
            if (this.CurrentModule == ModuleType.Basic)
            {
                this.BasicModuleButtonBackgroundColor = new Color(8, 32, 68);
                this.BasicModuleButtonTextColor = Colors.White;
                this.SpecialisticModuleButtonBackgroundColor = new Color(228, 240, 245);
                this.SpecialisticModuleButtonTextColor = Colors.Black;
            }
            else
            {
                this.BasicModuleButtonBackgroundColor = new Color(228, 240, 245);
                this.BasicModuleButtonTextColor = Colors.Black;
                this.SpecialisticModuleButtonBackgroundColor = new Color(8, 32, 68);
                this.SpecialisticModuleButtonTextColor = Colors.White;
            }
        }
    }
}