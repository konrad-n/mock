using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Internships.Views;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.Features.Internships.ViewModels
{
    public partial class InternshipsViewModel : ViewModelBase
    {
        private readonly ISpecializationService _specializationService;

        [ObservableProperty]
        private Specialization _specialization;

        [ObservableProperty]
        private ModuleType _currentModule = ModuleType.Basic;

        [ObservableProperty]
        private ObservableCollection<Internship> _internships;

        [ObservableProperty]
        private bool _isNoInternshipsVisible;

        [ObservableProperty]
        private Color _basicModuleButtonBackgroundColor = new Color(8, 32, 68);

        [ObservableProperty]
        private Color _basicModuleButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color _specialisticModuleButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color _specialisticModuleButtonTextColor = Colors.Black;

        public InternshipsViewModel(
            ISpecializationService specializationService,
            ILogger<InternshipsViewModel> logger) : base(logger)
        {
            _specializationService = specializationService;
            Internships = new ObservableCollection<Internship>();
            Title = "Staże";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading internships data");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                Specialization = await _specializationService.GetSpecializationAsync();
                DisplayInternships(CurrentModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading specialization data");
                throw;
            }
        }

        public void DisplayInternships(ModuleType moduleType)
        {
            CurrentModule = moduleType;
            UpdateModuleButtons();

            var filteredInternships = Specialization?.RequiredInternships
                .Where(i => i.Module == moduleType)
                .OrderBy(i => i.IsCompleted)
                .ToList() ?? new List<Internship>();

            Internships = new ObservableCollection<Internship>(filteredInternships);
            IsNoInternshipsVisible = Internships.Count == 0;
        }

        private void UpdateModuleButtons()
        {
            if (CurrentModule == ModuleType.Basic)
            {
                BasicModuleButtonBackgroundColor = new Color(8, 32, 68);
                BasicModuleButtonTextColor = Colors.White;
                SpecialisticModuleButtonBackgroundColor = new Color(228, 240, 245);
                SpecialisticModuleButtonTextColor = Colors.Black;
            }
            else
            {
                BasicModuleButtonBackgroundColor = new Color(228, 240, 245);
                BasicModuleButtonTextColor = Colors.Black;
                SpecialisticModuleButtonBackgroundColor = new Color(8, 32, 68);
                SpecialisticModuleButtonTextColor = Colors.White;
            }
        }

        [RelayCommand]
        private void SelectBasicModule()
        {
            DisplayInternships(ModuleType.Basic);
        }

        [RelayCommand]
        private void SelectSpecialisticModule()
        {
            DisplayInternships(ModuleType.Specialistic);
        }

        [RelayCommand]
        private async Task AddInternshipAsync()
        {
            await Shell.Current.Navigation.PushAsync(new InternshipDetailsPage(null, CurrentModule, OnInternshipAdded));
        }

        [RelayCommand]
        private async Task ViewInternshipDetailsAsync(int internshipId)
        {
            var internship = Specialization.RequiredInternships.FirstOrDefault(i => i.Id == internshipId);
            if (internship != null)
            {
                await Shell.Current.Navigation.PushAsync(new InternshipDetailsPage(internship, CurrentModule, OnInternshipUpdated));
            }
        }

        private async Task OnInternshipAdded(Internship internship)
        {
            try
            {
                await _specializationService.SaveInternshipAsync(internship);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving new internship");
                throw;
            }
        }

        private async Task OnInternshipUpdated(Internship internship)
        {
            try
            {
                await _specializationService.SaveInternshipAsync(internship);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating internship");
                throw;
            }
        }

        public bool IsCurrentInternship(Internship internship)
        {
            return internship.StartDate.HasValue && !internship.EndDate.HasValue;
        }

        public Color GetInternshipStatusColor(Internship internship)
        {
            if (internship.IsCompleted)
                return Colors.Green;
            if (IsCurrentInternship(internship))
                return Colors.Blue;
            if (internship.StartDate.HasValue)
                return Colors.Orange;
            return new Color(84, 126, 158);
        }

        public string GetInternshipStatusText(Internship internship)
        {
            if (internship.IsCompleted)
                return "Ukończony";
            if (IsCurrentInternship(internship))
                return $"W trakcie od: {internship.StartDate?.ToString("dd.MM.yyyy")}";
            if (internship.StartDate.HasValue)
                return $"Zaplanowany na: {internship.StartDate?.ToString("dd.MM.yyyy")}";
            return "Oczekujący";
        }
    }
}