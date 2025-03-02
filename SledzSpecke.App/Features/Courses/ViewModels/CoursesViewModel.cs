using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Courses.ViewModels
{
    public partial class CoursesViewModel : ViewModelBase
    {
        private readonly ISpecializationService _specializationService;

        [ObservableProperty]
        private ModuleType _currentModule = ModuleType.Basic;

        [ObservableProperty]
        private Specialization _specialization;

        [ObservableProperty]
        private bool _isModuleBasicSelected = true;

        [ObservableProperty]
        private bool _isModuleSpecialisticSelected = false;

        [ObservableProperty]
        private bool _noCoursesVisible = false;

        public CoursesViewModel(
            ISpecializationService specializationService,
            ILogger<CoursesViewModel> logger) : base(logger)
        {
            _specializationService = specializationService;
            Title = "Kursy";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                await LoadSpecializationDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses data");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadSpecializationDataAsync()
        {
            Specialization = await _specializationService.GetSpecializationAsync();
        }

        public void SelectBasicModule()
        {
            IsModuleBasicSelected = true;
            IsModuleSpecialisticSelected = false;
            CurrentModule = ModuleType.Basic;
        }

        public void SelectSpecialisticModule()
        {
            IsModuleBasicSelected = false;
            IsModuleSpecialisticSelected = true;
            CurrentModule = ModuleType.Specialistic;
        }

        public List<Course> GetFilteredCourses()
        {
            if (Specialization == null) return new List<Course>();

            return Specialization.RequiredCourses
                .Where(c => c.Module == CurrentModule)
                .OrderBy(c => c.IsCompleted)
                .ToList();
        }
    }
}