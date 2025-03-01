using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProceduresViewModel : ViewModelBase
    {
        private readonly ISpecializationService _specializationService;
        private readonly IDatabaseService _databaseService;

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
            IDatabaseService databaseService,
            ILogger<ProceduresViewModel> logger) : base(logger)
        {
            _specializationService = specializationService;
            _databaseService = databaseService;
            Title = "Procedury";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                await LoadSpecializationDataAsync();
                UpdateButtonStyles();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading procedures data");
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

        public void UpdateButtonStyles()
        {
            // Module buttons
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

            // Type buttons
            if (CurrentProcedureType == ProcedureType.TypeA)
            {
                TypeAButtonBackgroundColor = new Color(13, 117, 156);
                TypeAButtonTextColor = Colors.White;
                TypeBButtonBackgroundColor = new Color(228, 240, 245);
                TypeBButtonTextColor = Colors.Black;
            }
            else
            {
                TypeAButtonBackgroundColor = new Color(228, 240, 245);
                TypeAButtonTextColor = Colors.Black;
                TypeBButtonBackgroundColor = new Color(13, 117, 156);
                TypeBButtonTextColor = Colors.White;
            }
        }

        public async Task SaveProcedureAsync(MedicalProcedure procedure)
        {
            await _specializationService.SaveProcedureAsync(procedure);
            await LoadSpecializationDataAsync();
        }

        public async Task AddProcedureEntryAsync(MedicalProcedure procedure, ProcedureEntry entry)
        {
            await _specializationService.AddProcedureEntryAsync(procedure, entry);
            await LoadSpecializationDataAsync();
        }
    }
}