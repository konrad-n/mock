using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProceduresViewModel : ViewModelBase
    {
        private readonly ISpecializationService specializationService;

        [ObservableProperty]
        private ModuleType currentModule = ModuleType.Basic;

        [ObservableProperty]
        private ProcedureType currentProcedureType = ProcedureType.TypeA;

        [ObservableProperty]
        private Specialization specialization;

        [ObservableProperty]
        private bool isProceduresEmpty = false;

        [ObservableProperty]
        private Color basicModuleButtonBackgroundColor = new Color(8, 32, 68);

        [ObservableProperty]
        private Color basicModuleButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color specialisticModuleButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color specialisticModuleButtonTextColor = Colors.Black;

        [ObservableProperty]
        private Color typeAButtonBackgroundColor = new Color(13, 117, 156);

        [ObservableProperty]
        private Color typeAButtonTextColor = Colors.White;

        [ObservableProperty]
        private Color typeBButtonBackgroundColor = new Color(228, 240, 245);

        [ObservableProperty]
        private Color typeBButtonTextColor = Colors.Black;

        public ProceduresViewModel(
            ISpecializationService specializationService,
            ILogger<ProceduresViewModel> logger)
            : base(logger)
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