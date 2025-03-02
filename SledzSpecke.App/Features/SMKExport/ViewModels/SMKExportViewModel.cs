using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.SMKExport.ViewModels
{
    public partial class SMKExportViewModel : ViewModelBase
    {
        private readonly IExportService _exportService;
        private SMKExportOptions _exportOptions;

        [ObservableProperty]
        private bool _isGeneralExportSelected = true;

        [ObservableProperty]
        private bool _isProcedureExportSelected;

        [ObservableProperty]
        private bool _isDutyShiftExportSelected;

        [ObservableProperty]
        private bool _useSmkExactFormat = true;

        [ObservableProperty]
        private bool _includeCoursesChecked = true;

        [ObservableProperty]
        private bool _includeInternshipsChecked = true;

        [ObservableProperty]
        private bool _includeProceduresChecked = true;

        [ObservableProperty]
        private int _formatSelectedIndex;

        [ObservableProperty]
        private bool _isAllModulesSelected = true;

        [ObservableProperty]
        private bool _isBasicModuleSelected;

        [ObservableProperty]
        private bool _isSpecialisticModuleSelected;

        [ObservableProperty]
        private bool _isAllDatesSelected = true;

        [ObservableProperty]
        private bool _isCustomDatesSelected;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddMonths(-3);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        [ObservableProperty]
        private bool _isDateRangeVisible;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isResultVisible;

        [ObservableProperty]
        private string _filePath;

        public SMKExportViewModel(
            IExportService exportService,
            ILogger<SMKExportViewModel> logger) : base(logger)
        {
            _exportService = exportService;
            _exportOptions = new SMKExportOptions();
            Title = "Eksport do SMK";
        }

        public override Task InitializeAsync()
        {
            SetupInitialState();
            return Task.CompletedTask;
        }

        private void SetupInitialState()
        {
            _exportOptions = new SMKExportOptions();
            IsGeneralExportSelected = true;
            IsProcedureExportSelected = false;
            IsDutyShiftExportSelected = false;
            UseSmkExactFormat = true;
            IncludeCoursesChecked = true;
            IncludeInternshipsChecked = true;
            IncludeProceduresChecked = true;
            FormatSelectedIndex = 0;
            IsAllModulesSelected = true;
            IsBasicModuleSelected = false;
            IsSpecialisticModuleSelected = false;
            IsAllDatesSelected = true;
            IsCustomDatesSelected = false;
            StartDate = DateTime.Now.AddMonths(-3);
            EndDate = DateTime.Now;
            IsDateRangeVisible = false;
            IsLoading = false;
            IsResultVisible = false;
        }

        [RelayCommand]
        private void ChangeExportType(int exportTypeIndex)
        {
            switch (exportTypeIndex)
            {
                case 0: // General
                    IsGeneralExportSelected = true;
                    IsProcedureExportSelected = false;
                    IsDutyShiftExportSelected = false;
                    break;
                case 1: // Procedures
                    IsGeneralExportSelected = false;
                    IsProcedureExportSelected = true;
                    IsDutyShiftExportSelected = false;
                    UseSmkExactFormat = true;
                    break;
                case 2: // Duty Shifts
                    IsGeneralExportSelected = false;
                    IsProcedureExportSelected = false;
                    IsDutyShiftExportSelected = true;
                    UseSmkExactFormat = true;
                    break;
            }
        }

        [RelayCommand]
        private void ToggleCustomDates(bool isChecked)
        {
            IsDateRangeVisible = isChecked;
            if (isChecked)
            {
                IsAllDatesSelected = false;
                IsCustomDatesSelected = true;
            }
            else
            {
                IsAllDatesSelected = true;
                IsCustomDatesSelected = false;
            }
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            try
            {
                // Validate selections
                if (IsCustomDatesSelected && StartDate > EndDate)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd zakresu dat", "Data początkowa nie może być późniejsza niż data końcowa.", "OK");
                    return;
                }

                // If general export selected, ensure at least one category is selected
                if (IsGeneralExportSelected &&
                    !IncludeCoursesChecked && !IncludeInternshipsChecked && !IncludeProceduresChecked)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd wyboru zakresu", "Wybierz przynajmniej jedną kategorię danych do eksportu.", "OK");
                    return;
                }

                // Set export options
                _exportOptions = new SMKExportOptions
                {
                    ExportType = IsGeneralExportSelected ? SMKExportType.General :
                                  IsProcedureExportSelected ? SMKExportType.Procedures :
                                  SMKExportType.DutyShifts,

                    IncludeCourses = IncludeCoursesChecked,
                    IncludeInternships = IncludeInternshipsChecked,
                    IncludeProcedures = IncludeProceduresChecked,

                    Format = FormatSelectedIndex == 0 ? ExportFormat.Excel : ExportFormat.CSV,

                    ModuleFilter = IsAllModulesSelected ? SMKExportModuleFilter.All :
                                   IsBasicModuleSelected ? SMKExportModuleFilter.BasicOnly :
                                   SMKExportModuleFilter.SpecialisticOnly,

                    UseCustomDateRange = IsCustomDatesSelected,
                    StartDate = IsCustomDatesSelected ? StartDate : null,
                    EndDate = IsCustomDatesSelected ? EndDate : null,

                    UseSmkExactFormat = UseSmkExactFormat,
                    SplitDutyHoursAndMinutes = true // Always true for SMK format
                };

                // Show loading indicator
                IsLoading = true;

                // Generate report
                FilePath = await _exportService.ExportToSMKAsync(_exportOptions);

                // Show result
                IsResultVisible = true;

                await Application.Current.MainPage.DisplayAlert(
                    "Sukces",
                    $"Raport został wygenerowany pomyślnie w formacie zgodnym z SMK.\n\nFormat: {(_exportOptions.Format == ExportFormat.Excel ? "Excel" : "CSV")}",
                    "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SMK export");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem podczas generowania raportu: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task OpenFileAsync()
        {
            try
            {
                string filePath = FilePath;

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Plik nie istnieje lub został usunięty.", "OK");
                    return;
                }

                // Try to open the file in the default application
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening file");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się otworzyć pliku: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task OpenFolderAsync()
        {
            try
            {
                // Try to open the folder containing the file
                string folderPath = Path.GetDirectoryName(FilePath);

                if (!Directory.Exists(folderPath))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Folder nie istnieje lub został usunięty.", "OK");
                    return;
                }

                await Launcher.OpenAsync(folderPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening folder");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się otworzyć folderu: {ex.Message}", "OK");
            }
        }
    }
}