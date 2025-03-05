using System;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.SelfEducation
{
    /// <summary>
    /// ViewModel reprezentujący pojedynczy element samokształcenia w interfejsie użytkownika.
    /// </summary>
    public class SelfEducationViewModel : BaseViewModel
    {
        private int selfEducationId;
        private string title = string.Empty;
        private string type = string.Empty;
        private string publisher = string.Empty;
        private int year;
        private SyncStatus syncStatus;
        private string syncStatusText = string.Empty;
        private string moduleName = string.Empty;
        private bool requiresAcceptance;

        /// <summary>
        /// Pobiera lub ustawia identyfikator elementu samokształcenia.
        /// </summary>
        public int SelfEducationId
        {
            get => this.selfEducationId;
            set => this.SetProperty(ref this.selfEducationId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia tytuł elementu samokształcenia.
        /// </summary>
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        /// <summary>
        /// Pobiera lub ustawia typ elementu samokształcenia.
        /// </summary>
        public string Type
        {
            get => this.type;
            set => this.SetProperty(ref this.type, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wydawcę/organizatora elementu samokształcenia.
        /// </summary>
        public string Publisher
        {
            get => this.publisher;
            set => this.SetProperty(ref this.publisher, value);
        }

        /// <summary>
        /// Pobiera lub ustawia rok elementu samokształcenia.
        /// </summary>
        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy samokształcenie wymaga akceptacji (tylko stary SMK).
        /// </summary>
        public bool RequiresAcceptance
        {
            get => this.requiresAcceptance;
            set => this.SetProperty(ref this.requiresAcceptance, value);
        }

        /// <summary>
        /// Pobiera lub ustawia status synchronizacji z systemem SMK.
        /// </summary>
        public SyncStatus SyncStatus
        {
            get => this.syncStatus;
            set
            {
                if (this.SetProperty(ref this.syncStatus, value))
                {
                    this.SyncStatusText = this.GetSyncStatusText(value);
                }
            }
        }

        /// <summary>
        /// Pobiera lub ustawia tekstowy opis statusu synchronizacji.
        /// </summary>
        public string SyncStatusText
        {
            get => this.syncStatusText;
            set => this.SetProperty(ref this.syncStatusText, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę modułu, do którego należy element samokształcenia.
        /// </summary>
        public string ModuleName
        {
            get => this.moduleName;
            set => this.SetProperty(ref this.moduleName, value);
        }

        /// <summary>
        /// Konwertuje obiekt SelfEducation do SelfEducationViewModel.
        /// </summary>
        /// <param name="selfEducation">Obiekt samokształcenia z modelu danych.</param>
        /// <param name="moduleName">Opcjonalna nazwa modułu.</param>
        /// <returns>Obiekt ViewModel samokształcenia.</returns>
        public static SelfEducationViewModel FromModel(Models.SelfEducation selfEducation, string moduleName = null)
        {
            if (selfEducation == null)
            {
                return null;
            }

            var viewModel = new SelfEducationViewModel
            {
                SelfEducationId = selfEducation.SelfEducationId,
                Title = selfEducation.Title,
                Type = selfEducation.Type,
                Publisher = selfEducation.Publisher,
                Year = selfEducation.Year,
                SyncStatus = selfEducation.SyncStatus,
                ModuleName = moduleName ?? string.Empty,
            };

            // Parsowanie dodatkowych pól
            if (!string.IsNullOrEmpty(selfEducation.AdditionalFields))
            {
                try
                {
                    var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(selfEducation.AdditionalFields);
                    if (additionalFields != null && additionalFields.TryGetValue("RequiresAcceptance", out object acceptanceValue))
                    {
                        viewModel.RequiresAcceptance = Convert.ToBoolean(acceptanceValue);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas parsowania pól dodatkowych: {ex.Message}");
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Konwertuje ViewModel na model danych SelfEducation.
        /// </summary>
        /// <param name="specializationId">Identyfikator specjalizacji.</param>
        /// <param name="moduleId">Opcjonalny identyfikator modułu.</param>
        /// <returns>Obiekt modelu samokształcenia.</returns>
        public Models.SelfEducation ToModel(int specializationId, int? moduleId = null)
        {
            var selfEducation = new Models.SelfEducation
            {
                SelfEducationId = this.SelfEducationId,
                SpecializationId = specializationId,
                ModuleId = moduleId,
                Title = this.Title,
                Type = this.Type,
                Publisher = this.Publisher,
                Year = this.Year,
                SyncStatus = this.SyncStatus,
            };

            // Dodanie pól dodatkowych
            var additionalFields = new Dictionary<string, object>
            {
                { "RequiresAcceptance", this.RequiresAcceptance },
            };

            selfEducation.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);

            return selfEducation;
        }

        /// <summary>
        /// Zwraca tekstowy opis dla danego statusu synchronizacji.
        /// </summary>
        /// <param name="status">Status synchronizacji.</param>
        /// <returns>Opis tekstowy statusu.</returns>
        private string GetSyncStatusText(SyncStatus status)
        {
            return status switch
            {
                SyncStatus.NotSynced => "Nie zsynchronizowano z SMK",
                SyncStatus.Synced => "Zsynchronizowano z SMK",
                SyncStatus.SyncFailed => "Synchronizacja nie powiodła się",
                SyncStatus.Modified => "Zsynchronizowano, ale potem zmodyfikowano",
                _ => "Nieznany status",
            };
        }
    }
}