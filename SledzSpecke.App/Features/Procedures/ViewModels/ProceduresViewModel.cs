using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProceduresViewModel : ViewModelBase
    {
        private readonly ISpecializationService specializationService;

        [ObservableProperty]
        private ModuleType _currentModule = ModuleType.Basic;

        [ObservableProperty]
        private ProcedureType _currentProcedureType = ProcedureType.TypeA;

        [ObservableProperty]
        private Specialization _specialization;

        [ObservableProperty]
        private bool _isProceduresEmpty = false;

        [ObservableProperty]
        private Color _basicModuleButtonBackgroundColor = new Color(8, 32, 68);

        [ObservableProperty]
        private Color _basicModuleButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color _specialisticModuleButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color _specialisticModuleButtonTextColor = Colors.Black;

        [ObservableProperty]
        private Color _typeAButtonBackgroundColor = new Color(13, 117, 156);

        [ObservableProperty]
        private Color _typeAButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color _typeBButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color _typeBButtonTextColor = Colors.Black;

        public ProceduresViewModel(
            ISpecializationService specializationService,
            ILogger<ProceduresViewModel> logger) : base(logger)
        {
            this.specializationService = specializationService;
            this.Title = "Procedury";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadSpecializationDataAsync();
                this.UpdateButtonStyles();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading procedures data");
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

        public void UpdateButtonStyles()
        {
            // Module buttons
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

            // Type buttons
            if (this.CurrentProcedureType == ProcedureType.TypeA)
            {
                this.TypeAButtonBackgroundColor = new Color(13, 117, 156);
                this.TypeAButtonTextColor = Colors.White;
                this.TypeBButtonBackgroundColor = new Color(228, 240, 245);
                this.TypeBButtonTextColor = Colors.Black;
            }
            else
            {
                this.TypeAButtonBackgroundColor = new Color(228, 240, 245);
                this.TypeAButtonTextColor = Colors.Black;
                this.TypeBButtonBackgroundColor = new Color(13, 117, 156);
                this.TypeBButtonTextColor = Colors.White;
            }
        }

        public async Task SaveProcedureAsync(MedicalProcedure procedure)
        {
            await this.specializationService.SaveProcedureAsync(procedure);
            await this.LoadSpecializationDataAsync();
        }

        public async Task AddProcedureEntryAsync(MedicalProcedure procedure, ProcedureEntry entry)
        {
            await this.specializationService.AddProcedureEntryAsync(procedure, entry);
            await this.LoadSpecializationDataAsync();
        }
    }
}