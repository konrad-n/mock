using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Duties
{
    public partial class DutiesViewModel : BaseViewModel
    {
        private readonly IDutyService _dutyService;

        public DutiesViewModel(IDutyService dutyService)
        {
            _dutyService = dutyService;
            Title = "Dyżury";
            
            Duties = new ObservableCollection<DutyViewModel>();
            DutyTypes = new ObservableCollection<string> { "Wszystkie", "Regular", "Emergency", "Weekend", "Holiday", "Supervised" };
            SelectedDutyType = "Wszystkie";
        }

        [ObservableProperty]
        private ObservableCollection<DutyViewModel> duties;

        [ObservableProperty]
        private DutyStatistics statistics;

        [ObservableProperty]
        private ObservableCollection<string> dutyTypes;

        [ObservableProperty]
        private string selectedDutyType;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime toDate = DateTime.Today.AddMonths(1);

        [ObservableProperty]
        private decimal totalHours;

        [ObservableProperty]
        private decimal monthlyHours;

        [ObservableProperty]
        private decimal remainingHours;

        [ObservableProperty]
        private double progress;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Pobierz dyżury
                var userDuties = await _dutyService.GetUserDutiesAsync(FromDate);
                
                // Pobierz statystyki
                Statistics = await _dutyService.GetDutyStatisticsAsync();
                
                TotalHours = Statistics.TotalHours;
                MonthlyHours = Statistics.MonthlyHours;
                RemainingHours = Statistics.RemainingHours > 0 ? Statistics.RemainingHours : 1;
                
                // Oblicz postęp
                Progress = (double)(TotalHours / (TotalHours + RemainingHours));

                UpdateDutiesList(userDuties);
                
                await ApplyFiltersAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się załadować dyżurów: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateDutiesList(List<Duty> userDuties)
        {
            Duties.Clear();
            foreach (var duty in userDuties)
            {
                Duties.Add(new DutyViewModel
                {
                    Id = duty.Id,
                    Location = duty.Location,
                    Date = duty.StartTime.Date,
                    Hours = (decimal)(duty.EndTime - duty.StartTime).TotalHours,
                    Type = duty.Type.ToString(),
                    StartTime = duty.StartTime,
                    EndTime = duty.EndTime,
                    Notes = duty.Notes
                });
            }
        }

        partial void OnSelectedDutyTypeChanged(string value)
        {
            _ = ApplyFiltersAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var userDuties = await _dutyService.GetUserDutiesAsync(FromDate);
                
                // Filtrowanie wg typu
                if (SelectedDutyType != "Wszystkie")
                {
                    if (Enum.TryParse<DutyType>(SelectedDutyType, out var dutyType))
                    {
                        userDuties = userDuties.Where(d => d.Type == dutyType).ToList();
                    }
                }
                
                // Filtrowanie wg daty
                userDuties = userDuties.Where(d => 
                    d.StartTime.Date >= FromDate.Date && 
                    d.StartTime.Date <= ToDate.Date).ToList();

                UpdateDutiesList(userDuties);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd", 
                    $"Nie udało się zafiltrować dyżurów: {ex.Message}", 
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddDutyAsync()
        {
            await Shell.Current.GoToAsync("duty/add");
        }

        [RelayCommand]
        private async Task ViewDutyAsync(int id)
        {
            await Shell.Current.GoToAsync($"duty/edit?id={id}");
        }

        [RelayCommand]
        private async Task ExportDutiesAsync()
        {
            await Shell.Current.DisplayAlert(
                "Eksport dyżurów", 
                "Funkcja eksportu dyżurów zostanie zaimplementowana wkrótce.", 
                "OK");
        }
    }

    public class DutyViewModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
    }
}
