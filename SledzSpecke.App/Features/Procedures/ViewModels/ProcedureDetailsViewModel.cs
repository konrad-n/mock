﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Procedures.ViewModels
{
    public partial class ProcedureDetailsViewModel : ViewModelBase
    {
        private MedicalProcedure procedure;
        private Func<MedicalProcedure, Task> onSaveCallback;
        private List<Internship> internships;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private string requiredCount;

        [ObservableProperty]
        private string completedCount;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private int procedureTypePickerSelectedIndex;

        [ObservableProperty]
        private int modulePickerSelectedIndex;

        [ObservableProperty]
        private int internshipPickerSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<string> internshipItems = new();

        [ObservableProperty]
        private List<Internship> filteredInternships = new();

        public MedicalProcedure Procedure => this.procedure;

        public ProcedureDetailsViewModel(ILogger<ProcedureDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły procedury";
        }

        public void Initialize(
            MedicalProcedure procedure,
            ModuleType currentModule,
            ProcedureType currentProcedureType,
            Func<MedicalProcedure, Task> onSaveCallback,
            List<Internship> internships)
        {
            this.procedure = procedure ?? new MedicalProcedure();
            this.onSaveCallback = onSaveCallback;
            this.internships = internships;

            if (procedure == null)
            {
                // Nowa procedura
                this.procedure = new MedicalProcedure
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    ProcedureType = currentProcedureType,
                    CompletedCount = 0,
                };
                this.PageTitle = "Dodaj procedurę";
                this.RequiredCount = "1";
                this.CompletedCount = "0";
            }
            else
            {
                // Edycja istniejącej procedury
                this.PageTitle = "Szczegóły procedury";
                this.RequiredCount = procedure.RequiredCount.ToString();
                this.CompletedCount = procedure.CompletedCount.ToString();
                this.Notes = procedure.Description;
            }

            // Ustawienie pickerów
            this.ProcedureTypePickerSelectedIndex = this.procedure.ProcedureType == ProcedureType.TypeA ? 0 : 1;
            this.ModulePickerSelectedIndex = this.procedure.Module == ModuleType.Basic ? 0 : 1;

            // Wypełnienie pickera stażów
            this.LoadInternships(this.procedure.Module);
        }

        [RelayCommand]
        public void UpdateProcedureType(int selectedIndex)
        {
            if (this.procedure != null)
            {
                this.procedure.ProcedureType = selectedIndex == 0 ? ProcedureType.TypeA : ProcedureType.TypeB;
            }
        }

        [RelayCommand]
        public void UpdateModule(int selectedIndex)
        {
            if (this.procedure != null)
            {
                this.procedure.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;

                // Przy zmianie modułu zapamiętujemy aktualny identyfikator stażu
                int? currentInternshipId = this.procedure.InternshipId;

                // Ładujemy nową listę stażów
                this.LoadInternships(this.procedure.Module);

                // Jeśli staż należy do nowego modułu, ustawiamy go ponownie
                if (currentInternshipId.HasValue)
                {
                    int index = this.FilteredInternships.FindIndex(i => i.Id == currentInternshipId.Value);
                    if (index >= 0)
                    {
                        this.InternshipPickerSelectedIndex = index;
                        this.procedure.InternshipId = currentInternshipId;
                    }
                }
            }
        }

        [RelayCommand]
        public void UpdateInternship(int selectedIndex)
        {
            if (this.procedure != null && selectedIndex >= 0 && selectedIndex < this.FilteredInternships.Count)
            {
                this.procedure.InternshipId = this.FilteredInternships[selectedIndex].Id;
            }
            else if (this.procedure != null)
            {
                // Brak wyboru stażu
                this.procedure.InternshipId = null;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(this.procedure.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa procedury jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.RequiredCount, out int requiredCount) || requiredCount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną wymaganą liczbę procedur.", "OK");
                return;
            }

            if (this.InternshipPickerSelectedIndex < 0 ||
                this.InternshipPickerSelectedIndex >= this.FilteredInternships.Count)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wybierz staż, w ramach którego wykonywana jest procedura.", "OK");
                return;
            }

            this.procedure.RequiredCount = requiredCount;
            this.procedure.Description = this.Notes;

            // Upewniamy się, że przypisany jest poprawny staż
            this.procedure.InternshipId = this.FilteredInternships[this.InternshipPickerSelectedIndex].Id;

            if (this.onSaveCallback != null)
            {
                await this.onSaveCallback(this.procedure);
            }

            await Shell.Current.Navigation.PopAsync();
        }

        private void LoadInternships(ModuleType moduleType)
        {
            this.InternshipItems.Clear();
            this.FilteredInternships = this.internships.Where(i => i.Module == moduleType).ToList();

            foreach (var internship in this.FilteredInternships)
            {
                this.InternshipItems.Add(internship.Name);
            }

            // Jeśli edytujemy procedurę, ustawiamy wybrany staż
            if (this.procedure.InternshipId.HasValue)
            {
                int index = this.FilteredInternships.FindIndex(i => i.Id == this.procedure.InternshipId);
                if (index >= 0)
                {
                    this.InternshipPickerSelectedIndex = index;
                }
                else
                {
                    // Staż nie należy do wybranego modułu, więc ustawiamy pusty
                    this.InternshipPickerSelectedIndex = -1;

                    // Czyścimy przypisanie stażu
                    this.procedure.InternshipId = null;
                }
            }
            else
            {
                this.InternshipPickerSelectedIndex = -1;
            }
        }
    }
}