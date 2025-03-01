﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Duties.ViewModels
{
    public partial class DutyShiftDetailsViewModel : ViewModelBase
    {
        private Func<DutyShift, Task> onSaveCallback;
        private DutyShift dutyShift;
        private bool isNewDutyShift = false;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan startTime = new TimeSpan(8, 0, 0); // 8:00 AM

        [ObservableProperty]
        private DateTime endDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private TimeSpan endTime = new TimeSpan(8, 0, 0); // 8:00 AM next day

        [ObservableProperty]
        private string durationText = "24 godziny";

        [ObservableProperty]
        private string location = string.Empty;

        [ObservableProperty]
        private string departmentName = string.Empty; // For SMK

        [ObservableProperty]
        private string supervisorName = string.Empty;

        [ObservableProperty]
        private string notes = string.Empty;

        [ObservableProperty]
        private bool isSupervisorVisible;

        [ObservableProperty]
        private int dutyTypeSelectedIndex;

        public DutyShiftDetailsViewModel(ILogger<DutyShiftDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły dyżuru";
        }

        public void Initialize(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            this.onSaveCallback = onSaveCallback;

            if (dutyShift == null)
            {
                // New duty shift
                this.isNewDutyShift = true;
                this.dutyShift = new DutyShift
                {
                    StartDate = DateTime.Now.Date.Add(this.StartTime),
                    EndDate = DateTime.Now.Date.AddDays(1).Add(this.EndTime),
                    DurationHours = 24,
                    Type = DutyType.Independent,
                };
                this.PageTitle = "Dodaj dyżur";
                this.DutyTypeSelectedIndex = 0; // Independent
                this.IsSupervisorVisible = false;
            }
            else
            {
                // Edit existing duty shift
                this.isNewDutyShift = false;
                this.dutyShift = dutyShift;
                this.PageTitle = "Edytuj dyżur";

                this.StartDate = dutyShift.StartDate.Date;
                this.StartTime = dutyShift.StartDate.TimeOfDay;
                this.EndDate = dutyShift.EndDate.Date;
                this.EndTime = dutyShift.EndDate.TimeOfDay;

                this.Location = dutyShift.Location ?? string.Empty;
                this.DepartmentName = dutyShift.DepartmentName ?? string.Empty;
                this.SupervisorName = dutyShift.SupervisorName ?? string.Empty;
                this.Notes = dutyShift.Notes ?? string.Empty;

                this.DutyTypeSelectedIndex = dutyShift.Type == DutyType.Independent ? 0 : 1;
                this.IsSupervisorVisible = dutyShift.Type == DutyType.Accompanied;

                this.UpdateDurationText();
            }
        }

        public void UpdateDurationText()
        {
            try
            {
                // Sprawdź czy dutyShift nie jest null
                if (this.dutyShift == null)
                {
                    this.DurationText = "Wczytywanie...";
                    return;
                }

                var startDateTime = this.StartDate.Date.Add(this.StartTime);
                var endDateTime = this.EndDate.Date.Add(this.EndTime);

                if (endDateTime <= startDateTime)
                {
                    this.DurationText = "Nieprawidłowy zakres czasu";
                    return;
                }

                var duration = endDateTime - startDateTime;

                // Warn about long duty shifts
                if (duration.TotalHours > 24)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        bool continueAnyway = await Application.Current.MainPage.DisplayAlert(
                            "Uwaga",
                            "Dyżur przekracza 24 godziny. Dyżury powinny zazwyczaj trwać maksymalnie 24 godziny. Czy na pewno chcesz kontynuować?",
                            "Tak",
                            "Nie");

                        if (!continueAnyway)
                        {
                            // Reset to 24 hours duty
                            this.EndDate = this.StartDate.Date.AddDays(1);
                            this.EndTime = this.StartTime;
                            this.OnPropertyChanged(nameof(this.EndDate));
                            this.OnPropertyChanged(nameof(this.EndTime));
                            this.UpdateDurationText();
                            return;
                        }
                    });
                }

                this.dutyShift.DurationHours = duration.TotalHours;

                // Format according to SMK requirements - split into hours and minutes
                int hours = (int)Math.Floor(duration.TotalHours);
                int minutes = (int)Math.Round((duration.TotalHours - hours) * 60);

                if (duration.TotalHours < 24)
                {
                    this.DurationText = $"{hours} godz. {minutes} min.";
                }
                else
                {
                    var days = Math.Floor(duration.TotalDays);
                    var remainingHours = hours - (days * 24);
                    this.DurationText = days > 0
                        ? $"{days} dni {remainingHours} godz. {minutes} min."
                        : $"{hours} godz. {minutes} min.";
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating duration text");
                this.DurationText = "Błąd obliczania czasu";
            }
        }

        [RelayCommand]
        private void UpdateDutyType(int index)
        {
            // Sprawdź czy dutyShift nie jest null
            if (this.dutyShift == null)
            {
                this.logger?.LogWarning("Próba aktualizacji typu dyżuru gdy dutyShift jest null");
                return;
            }

            this.dutyShift.Type = index == 0 ? DutyType.Independent : DutyType.Accompanied;
            this.IsSupervisorVisible = this.dutyShift.Type == DutyType.Accompanied;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(this.Location))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce dyżuru jest wymagane.", "OK");
                    return;
                }

                var startDateTime = this.StartDate.Date.Add(this.StartTime);
                var endDateTime = this.EndDate.Date.Add(this.EndTime);

                if (endDateTime <= startDateTime)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Data i godzina zakończenia musi być późniejsza niż data i godzina rozpoczęcia.", "OK");
                    return;
                }

                // For accompanied type, supervisor is required
                if (this.dutyShift.Type == DutyType.Accompanied && string.IsNullOrWhiteSpace(this.SupervisorName))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla dyżuru towarzyszącego.", "OK");
                    return;
                }

                // Always create a new duty shift object to avoid reference issues
                var dutyShiftToSave = new DutyShift
                {
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    DurationHours = (endDateTime - startDateTime).TotalHours,
                    Location = this.Location,
                    DepartmentName = this.DepartmentName,
                    SupervisorName = this.SupervisorName,
                    Notes = this.Notes,
                    Type = this.dutyShift.Type,
                };

                // Only set the ID if we're editing an existing duty shift
                if (!this.isNewDutyShift)
                {
                    dutyShiftToSave.Id = this.dutyShift.Id;
                }

                if (this.onSaveCallback != null)
                {
                    await this.onSaveCallback(dutyShiftToSave);
                }
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving duty shift");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem podczas zapisywania dyżuru: {ex.Message}", "OK");
            }
        }
    }
}