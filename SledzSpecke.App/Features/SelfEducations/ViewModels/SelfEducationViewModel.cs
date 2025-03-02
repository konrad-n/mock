using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.SelfEducations.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.SelfEducations.ViewModels
{
    public partial class SelfEducationViewModel : ViewModelBase
    {
        private readonly ISelfEducationService selfEducationService;

        [ObservableProperty]
        private ObservableCollection<SelfEducation> selfEducationList;

        [ObservableProperty]
        private ObservableCollection<YearlyEducationGroup> educationByYear;

        [ObservableProperty]
        private string usedDaysLabel;

        [ObservableProperty]
        private string yearlyDaysLabel;

        [ObservableProperty]
        private bool noEventsVisible;

        public SelfEducationViewModel(
            ISelfEducationService selfEducationService,
            ILogger<SelfEducationViewModel> logger)
            : base(logger)
        {
            this.selfEducationService = selfEducationService;
            this.SelfEducationList = new ObservableCollection<SelfEducation>();
            this.EducationByYear = new ObservableCollection<YearlyEducationGroup>();
            this.Title = "Samokształcenie";
        }

        public static string GetSelfEducationTypeName(SelfEducationType type)
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

        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading self-education data");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Get all self-education events
                var events = await this.selfEducationService.GetAllSelfEducationAsync();
                this.SelfEducationList = new ObservableCollection<SelfEducation>(events);

                // Update used days label
                var totalUsedDays = await this.selfEducationService.GetTotalUsedDaysAsync();
                var yearlyAllowance = await this.selfEducationService.GetYearlyAllowanceAsync();
                var totalAllowedDays = yearlyAllowance * 3; // 3 years typical
                this.UsedDaysLabel = $"{totalUsedDays}/{totalAllowedDays}";

                // Get yearly used days and update label
                var currentYear = DateTime.Now.Year;
                var yearlyUsedDays = await this.selfEducationService.GetYearlyUsedDaysAsync();
                var usedDaysThisYear = yearlyUsedDays.ContainsKey(currentYear) ? yearlyUsedDays[currentYear] : 0;
                this.YearlyDaysLabel = $"{usedDaysThisYear} dni";

                // Group education events by year
                this.GroupEducationEventsByYear();

                // Show/hide "no events" message
                this.NoEventsVisible = this.SelfEducationList.Count == 0;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading self-education data");
                throw;
            }
        }

        private void GroupEducationEventsByYear()
        {
            try
            {
                this.EducationByYear.Clear();

                var groupedEvents = this.SelfEducationList
                    .OrderByDescending(s => s.StartDate)
                    .GroupBy(s => s.StartDate.Year)
                    .ToList();

                foreach (var yearGroup in groupedEvents)
                {
                    var totalYearDays = yearGroup.Sum(s => s.DurationDays);
                    var yearEvents = new ObservableCollection<SelfEducation>(yearGroup.OrderByDescending(s => s.StartDate));
                    var yearInfo = new YearlyEducationGroup(yearGroup.Key, totalYearDays, yearEvents);
                    this.EducationByYear.Add(yearInfo);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error grouping education events by year");
            }
        }

        [RelayCommand]
        private async Task AddSelfEducationAsync()
        {
            await Shell.Current.Navigation.PushAsync(new SelfEducationDetailsPage(null, this.OnSelfEducationAdded));
        }

        [RelayCommand]
        private async Task EditSelfEducationAsync(int educationId)
        {
            var selfEducation = this.SelfEducationList.FirstOrDefault(s => s.Id == educationId);
            if (selfEducation != null)
            {
                await Shell.Current.Navigation.PushAsync(new SelfEducationDetailsPage(selfEducation, this.OnSelfEducationUpdated));
            }
        }

        private void OnSelfEducationAdded(SelfEducation selfEducation)
        {
            try
            {
                // Generowanie nowego ID
                selfEducation.Id = this.SelfEducationList.Count > 0 ? this.SelfEducationList.Max(s => s.Id) + 1 : 1;
                this.SelfEducationList.Add(selfEducation);

                // Save to database
                this.selfEducationService.SaveSelfEducationAsync(selfEducation);

                // Refresh view
                this.LoadDataAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error adding self-education event");
            }
        }

        private void OnSelfEducationUpdated(SelfEducation selfEducation)
        {
            try
            {
                var existingSelfEducation = this.SelfEducationList.FirstOrDefault(s => s.Id == selfEducation.Id);
                if (existingSelfEducation != null)
                {
                    var index = this.SelfEducationList.IndexOf(existingSelfEducation);
                    this.SelfEducationList[index] = selfEducation;
                }

                // Save to database
                this.selfEducationService.SaveSelfEducationAsync(selfEducation);

                // Refresh view
                this.LoadDataAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating self-education event");
            }
        }
    }
}