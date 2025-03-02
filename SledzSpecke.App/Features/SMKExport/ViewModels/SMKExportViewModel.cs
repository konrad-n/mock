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
        private readonly IExportService exportService;
        private SmkExportOptions exportOptions;

        [ObservableProperty]
        private bool isGeneralExportSelected = true;

        [ObservableProperty]
        private bool isProcedureExportSelected;

        [ObservableProperty]
        private bool isDutyShiftExportSelected;

        [ObservableProperty]
        private bool useSmkExactFormat = true;

        [ObservableProperty]
        private bool includeCoursesChecked = true;

        [ObservableProperty]
        private bool includeInternshipsChecked = true;

        [ObservableProperty]
        private bool includeProceduresChecked = true;

        [ObservableProperty]
        private int formatSelectedIndex;

        [ObservableProperty]
        private bool isAllModulesSelected = true;

        [ObservableProperty]
        private bool isBasicModuleSelected;

        [ObservableProperty]
        private bool isSpecialisticModuleSelected;

        [ObservableProperty]
        private bool isAllDatesSelected = true;

        [ObservableProperty]
        private bool isCustomDatesSelected;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now.AddMonths(-3);

        [ObservableProperty]
        private DateTime endDate = DateTime.Now;

        [ObservableProperty]
        private bool isDateRangeVisible;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isResultVisible;

        [ObservableProperty]
        private string filePath;

        public SMKExportViewModel(
            IExportService exportService,
            ILogger<SMKExportViewModel> logger) : base(logger)
        {
            this.exportService = exportService;
            this.exportOptions = new SmkExportOptions();
            this.Title = "Eksport do SMK";
        }

        public override Task InitializeAsync()
        {
            this.SetupInitialState();
            return Task.CompletedTask;
        }

        private void SetupInitialState()
        {
            this.exportOptions = new SmkExportOptions();
            this.IsGeneralExportSelected = true;
            this.IsProcedureExportSelected = false;
            this.IsDutyShiftExportSelected = false;
            this.UseSmkExactFormat = true;
            this.IncludeCoursesChecked = true;
            this.IncludeInternshipsChecked = true;
            this.IncludeProceduresChecked = true;
            this.FormatSelectedIndex = 0;
            this.IsAllModulesSelected = true;
            this.IsBasicModuleSelected = false;
            this.IsSpecialisticModuleSelected = false;
            this.IsAllDatesSelected = true;
            this.IsCustomDatesSelected = false;
            this.StartDate = DateTime.Now.AddMonths(-3);
            this.EndDate = DateTime.Now;
            this.IsDateRangeVisible = false;
            this.IsLoading = false;
            this.IsResultVisible = false;
        }

        [RelayCommand]
        private void ChangeExportType(int exportTypeIndex)
        {
            switch (exportTypeIndex)
            {
                case 0: // General
                    this.IsGeneralExportSelected = true;
                    this.IsProcedureExportSelected = false;
                    this.IsDutyShiftExportSelected = false;
                    break;
                case 1: // Procedures
                    this.IsGeneralExportSelected = false;
                    this.IsProcedureExportSelected = true;
                    this.IsDutyShiftExportSelected = false;
                    this.UseSmkExactFormat = true;
                    break;
                case 2: // Duty Shifts
                    this.IsGeneralExportSelected = false;
                    this.IsProcedureExportSelected = false;
                    this.IsDutyShiftExportSelected = true;
                    this.UseSmkExactFormat = true;
                    break;
            }
        }

        [RelayCommand]
        private void ToggleCustomDates(bool isChecked)
        {
            this.IsDateRangeVisible = isChecked;
            if (isChecked)
            {
                this.IsAllDatesSelected = false;
                this.IsCustomDatesSelected = true;
            }
            else
            {
                this.IsAllDatesSelected = true;
                this.IsCustomDatesSelected = false;
            }
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            try
            {
                // Validate selections
                if (this.IsCustomDatesSelected && this.StartDate > this.EndDate)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd zakresu dat", "Data początkowa nie może być późniejsza niż data końcowa.", "OK");
                    return;
                }

                // If general export selected, ensure at least one category is selected
                if (this.IsGeneralExportSelected &&
                    !this.IncludeCoursesChecked && !this.IncludeInternshipsChecked && !this.IncludeProceduresChecked)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd wyboru zakresu", "Wybierz przynajmniej jedną kategorię danych do eksportu.", "OK");
                    return;
                }

                // Set export options
                this.exportOptions = new SmkExportOptions
                {
                    ExportType = this.IsGeneralExportSelected ? SmkExportType.General :
                                  this.IsProcedureExportSelected ? SmkExportType.Procedures :
                                  SmkExportType.DutyShifts,

                    IncludeCourses = this.IncludeCoursesChecked,
                    IncludeInternships = this.IncludeInternshipsChecked,
                    IncludeProcedures = this.IncludeProceduresChecked,

                    Format = this.FormatSelectedIndex == 0 ? ExportFormat.Excel : ExportFormat.CSV,

                    ModuleFilter = this.IsAllModulesSelected ? SmkExportModuleFilter.All :
                                   this.IsBasicModuleSelected ? SmkExportModuleFilter.BasicOnly :
                                   SmkExportModuleFilter.SpecialisticOnly,

                    UseCustomDateRange = this.IsCustomDatesSelected,
                    StartDate = this.IsCustomDatesSelected ? this.StartDate : null,
                    EndDate = this.IsCustomDatesSelected ? this.EndDate : null,

                    UseSmkExactFormat = this.UseSmkExactFormat,
                    SplitDutyHoursAndMinutes = true // Always true for SMK format
                };

                // Show loading indicator
                this.IsLoading = true;

                // Generate report
                this.FilePath = await this.exportService.ExportToSMKAsync(this.exportOptions);

                // Show result
                this.IsResultVisible = true;

                await Application.Current.MainPage.DisplayAlert(
                    "Sukces",
                    $"Raport został wygenerowany pomyślnie w formacie zgodnym z SMK.\n\nFormat: {(this.exportOptions.Format == ExportFormat.Excel ? "Excel" : "CSV")}",
                    "OK");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error generating SMK export");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem podczas generowania raportu: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                this.IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task OpenFileAsync()
        {
            try
            {
                string filePath = this.FilePath;

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
                this.logger.LogError(ex, "Error opening file");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się otworzyć pliku: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task OpenFolderAsync()
        {
            try
            {
                // Try to open the folder containing the file
                string folderPath = Path.GetDirectoryName(this.FilePath);

                if (!Directory.Exists(folderPath))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Folder nie istnieje lub został usunięty.", "OK");
                    return;
                }

                await Launcher.OpenAsync(folderPath);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error opening folder");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się otworzyć folderu: {ex.Message}", "OK");
            }
        }
    }
}