using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.SelfEducations.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.Features.SelfEducations.ViewModels
{
    public partial class SelfEducationViewModel : ViewModelBase
    {
        private readonly ISelfEducationService _selfEducationService;

        [ObservableProperty]
        private ObservableCollection<SelfEducation> _selfEducationList;

        [ObservableProperty]
        private ObservableCollection<YearlyEducationGroup> _educationByYear;

        [ObservableProperty]
        private string _usedDaysLabel;

        [ObservableProperty]
        private string _yearlyDaysLabel;

        [ObservableProperty]
        private bool _noEventsVisible;

        public SelfEducationViewModel(
            ISelfEducationService selfEducationService,
            ILogger<SelfEducationViewModel> logger) : base(logger)
        {
            _selfEducationService = selfEducationService;
            SelfEducationList = new ObservableCollection<SelfEducation>();
            EducationByYear = new ObservableCollection<YearlyEducationGroup>();
            Title = "Samokształcenie";
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
                _logger.LogError(ex, "Error loading self-education data");
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
                // Get all self-education events
                var events = await _selfEducationService.GetAllSelfEducationAsync();
                SelfEducationList = new ObservableCollection<SelfEducation>(events);

                // Update used days label
                var totalUsedDays = await _selfEducationService.GetTotalUsedDaysAsync();
                var yearlyAllowance = await _selfEducationService.GetYearlyAllowanceAsync();
                var totalAllowedDays = yearlyAllowance * 3; // 3 years typical
                UsedDaysLabel = $"{totalUsedDays}/{totalAllowedDays}";

                // Get yearly used days and update label
                var currentYear = DateTime.Now.Year;
                var yearlyUsedDays = await _selfEducationService.GetYearlyUsedDaysAsync();
                var usedDaysThisYear = yearlyUsedDays.ContainsKey(currentYear) ? yearlyUsedDays[currentYear] : 0;
                YearlyDaysLabel = $"{usedDaysThisYear} dni";

                // Group education events by year
                GroupEducationEventsByYear();

                // Show/hide "no events" message
                NoEventsVisible = SelfEducationList.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading self-education data");
                throw;
            }
        }

        private void GroupEducationEventsByYear()
        {
            try
            {
                EducationByYear.Clear();

                var groupedEvents = SelfEducationList
                    .OrderByDescending(s => s.StartDate)
                    .GroupBy(s => s.StartDate.Year)
                    .ToList();

                foreach (var yearGroup in groupedEvents)
                {
                    var totalYearDays = yearGroup.Sum(s => s.DurationDays);
                    var yearEvents = new ObservableCollection<SelfEducation>(yearGroup.OrderByDescending(s => s.StartDate));
                    var yearInfo = new YearlyEducationGroup(yearGroup.Key, totalYearDays, yearEvents);
                    EducationByYear.Add(yearInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grouping education events by year");
            }
        }

        [RelayCommand]
        private async Task AddSelfEducationAsync()
        {
            await Shell.Current.Navigation.PushAsync(new SelfEducationDetailsPage(null, OnSelfEducationAdded));
        }

        [RelayCommand]
        private async Task EditSelfEducationAsync(int educationId)
        {
            var selfEducation = SelfEducationList.FirstOrDefault(s => s.Id == educationId);
            if (selfEducation != null)
            {
                await Shell.Current.Navigation.PushAsync(new SelfEducationDetailsPage(selfEducation, OnSelfEducationUpdated));
            }
        }

        private void OnSelfEducationAdded(SelfEducation selfEducation)
        {
            try
            {
                // Generowanie nowego ID
                selfEducation.Id = SelfEducationList.Count > 0 ? SelfEducationList.Max(s => s.Id) + 1 : 1;
                SelfEducationList.Add(selfEducation);

                // Save to database
                _selfEducationService.SaveSelfEducationAsync(selfEducation);

                // Refresh view
                LoadDataAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding self-education event");
            }
        }

        private void OnSelfEducationUpdated(SelfEducation selfEducation)
        {
            try
            {
                var existingSelfEducation = SelfEducationList.FirstOrDefault(s => s.Id == selfEducation.Id);
                if (existingSelfEducation != null)
                {
                    var index = SelfEducationList.IndexOf(existingSelfEducation);
                    SelfEducationList[index] = selfEducation;
                }

                // Save to database
                _selfEducationService.SaveSelfEducationAsync(selfEducation);

                // Refresh view
                LoadDataAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating self-education event");
            }
        }

        public string GetSelfEducationTypeName(SelfEducationType type)
        {
            return type switch
            {
                SelfEducationType.Conference => "Konferencja",
                SelfEducationType.Workshop => "Warsztaty",
                SelfEducationType.Course => "Kurs",
                SelfEducationType.ScientificMeeting => "Spotkanie naukowe",
                SelfEducationType.Publication => "Publikacja",
                SelfEducationType.Other => "Inne",
                _ => "Nieznany"
            };
        }
    }

    public class YearlyEducationGroup : ObservableCollection<SelfEducation>
    {
        public int Year { get; private set; }
        public int TotalDays { get; private set; }
        public string Header { get; private set; }
        public string Summary { get; private set; }

        public YearlyEducationGroup(int year, int totalDays, ObservableCollection<SelfEducation> items) : base(items)
        {
            Year = year;
            TotalDays = totalDays;
            Header = $"Rok {year}";
            Summary = $"Wykorzystano: {totalDays}/6 dni";
        }
    }
}