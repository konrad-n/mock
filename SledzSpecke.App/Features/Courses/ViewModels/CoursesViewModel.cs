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
        private readonly ISpecializationService specializationService;

        [ObservableProperty]
        private ModuleType currentModule = ModuleType.Basic;

        [ObservableProperty]
        private Specialization specialization;

        [ObservableProperty]
        private bool isModuleBasicSelected = true;

        [ObservableProperty]
        private bool isModuleSpecialisticSelected = false;

        [ObservableProperty]
        private bool noCoursesVisible = false;

        public CoursesViewModel(
            ISpecializationService specializationService,
            ILogger<CoursesViewModel> logger)
            : base(logger)
        {
            this.specializationService = specializationService;
            this.Title = "Kursy";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadSpecializationDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading courses data");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task LoadSpecializationDataAsync()
        {
            this.Specialization = await this.specializationService.GetSpecializationAsync();
        }

        public void SelectBasicModule()
        {
            this.IsModuleBasicSelected = true;
            this.IsModuleSpecialisticSelected = false;
            this.CurrentModule = ModuleType.Basic;
        }

        public void SelectSpecialisticModule()
        {
            this.IsModuleBasicSelected = false;
            this.IsModuleSpecialisticSelected = true;
            this.CurrentModule = ModuleType.Specialistic;
        }

        public List<Course> GetFilteredCourses()
        {
            if (this.Specialization == null)
            {
                return new List<Course>();
            }

            return this.Specialization.RequiredCourses
                .Where(c => c.Module == this.CurrentModule)
                .OrderBy(c => c.IsCompleted)
                .ToList();
        }
    }
}
