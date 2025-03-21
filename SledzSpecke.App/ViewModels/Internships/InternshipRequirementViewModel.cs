﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke;
using SledzSpecke.App;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipRequirementViewModel : ObservableObject
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IDialogService dialogService;

        private InternshipRequirement requirement;
        private MedicalShiftsSummary summary;
        private ObservableCollection<RealizedMedicalShiftNewSMK> shifts;
        private bool isExpanded;
        private RealizedMedicalShiftNewSMK currentShift;
        private bool isEditing;

        public InternshipRequirementViewModel(
            InternshipRequirement requirement,
            MedicalShiftsSummary summary,
            List<RealizedMedicalShiftNewSMK> shifts,
            IMedicalShiftsService medicalShiftsService,
            IDialogService dialogService,
            int? currentModuleId)
        {
            this.requirement = requirement;
            this.summary = summary;
            this.medicalShiftsService = medicalShiftsService;
            this.dialogService = dialogService;

            Shifts = new ObservableCollection<RealizedMedicalShiftNewSMK>(shifts);
            currentShift = new RealizedMedicalShiftNewSMK
            {
                InternshipRequirementId = requirement.Id,
                ModuleId = currentModuleId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Hours = summary.TotalHours,
                Minutes = summary.TotalMinutes
            };
            ToggleExpandCommand = new RelayCommand(ToggleExpand);
            SaveCommand = new AsyncRelayCommand(SaveShiftAsync);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public string Name => requirement.Name;
        public int Id => requirement.Id;
        public string FormattedTime => $"{summary.TotalHours} godz. {summary.TotalMinutes} min.";
        public string Title => $"Dyżury do stażu\n{Name}";
        public string Summary => $"Czas wprowadzony:\n{FormattedTime}";

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public RealizedMedicalShiftNewSMK CurrentShift
        {
            get => currentShift;
            set => SetProperty(ref currentShift, value);
        }

        public ObservableCollection<RealizedMedicalShiftNewSMK> Shifts
        {
            get => shifts;
            set => SetProperty(ref shifts, value);
        }

        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            IsEditing = IsExpanded;
        }

        private async Task SaveShiftAsync()
        {
            bool success = await medicalShiftsService.SaveNewSMKShiftAsync(CurrentShift);

            if (success)
            {
                var shifts = await medicalShiftsService.GetNewSMKShiftsAsync(Id);
                Shifts.Clear();
                foreach (var shift in shifts)
                {
                    Shifts.Add(shift);
                }

                summary = await medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: Id);
                OnPropertyChanged(nameof(FormattedTime));
                OnPropertyChanged(nameof(Summary));
                IsEditing = false;
                IsExpanded = false;
            }
            else
            {
                await dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać dyżuru.",
                    "OK");
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            IsExpanded = false;
        }
    }
}