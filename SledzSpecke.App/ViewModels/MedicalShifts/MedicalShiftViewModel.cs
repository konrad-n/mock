using System;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    /// <summary>
    /// ViewModel reprezentujący pojedynczy dyżur medyczny w interfejsie użytkownika.
    /// </summary>
    public class MedicalShiftViewModel : BaseViewModel
    {
        private int shiftId;
        private DateTime date;
        private int hours;
        private int minutes;
        private string location = string.Empty;
        private int year;
        private string internshipName = string.Empty;
        private SyncStatus syncStatus;
        private string syncStatusText = string.Empty;

        /// <summary>
        /// Pobiera lub ustawia identyfikator dyżuru.
        /// </summary>
        public int ShiftId
        {
            get => this.shiftId;
            set => this.SetProperty(ref this.shiftId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę dyżuru.
        /// </summary>
        public DateTime Date
        {
            get => this.date;
            set => this.SetProperty(ref this.date, value);
        }

        /// <summary>
        /// Pobiera lub ustawia liczbę godzin dyżuru.
        /// </summary>
        public int Hours
        {
            get => this.hours;
            set => this.SetProperty(ref this.hours, value);
        }

        /// <summary>
        /// Pobiera lub ustawia liczbę minut dyżuru.
        /// </summary>
        public int Minutes
        {
            get => this.minutes;
            set => this.SetProperty(ref this.minutes, value);
        }

        /// <summary>
        /// Pobiera lub ustawia miejsce dyżuru.
        /// </summary>
        public string Location
        {
            get => this.location;
            set => this.SetProperty(ref this.location, value);
        }

        /// <summary>
        /// Pobiera lub ustawia rok szkolenia.
        /// </summary>
        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę stażu, podczas którego odbył się dyżur.
        /// </summary>
        public string InternshipName
        {
            get => this.internshipName;
            set => this.SetProperty(ref this.internshipName, value);
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
        /// Pobiera tekstową reprezentację czasu trwania dyżuru.
        /// </summary>
        public string DurationText
        {
            get
            {
                if (this.Minutes > 0)
                {
                    return $"{this.Hours}h {this.Minutes}min";
                }
                else
                {
                    return $"{this.Hours}h";
                }
            }
        }

        /// <summary>
        /// Pobiera łączny czas dyżuru w godzinach (z uwzględnieniem minut).
        /// </summary>
        public double TotalHours
        {
            get => this.Hours + (this.Minutes / 60.0);
        }

        /// <summary>
        /// Konwertuje obiekt MedicalShift do MedicalShiftViewModel.
        /// </summary>
        /// <param name="shift">Obiekt dyżuru z modelu danych.</param>
        /// <param name="internshipName">Opcjonalna nazwa stażu.</param>
        /// <returns>Obiekt ViewModel dyżuru.</returns>
        public static MedicalShiftViewModel? FromModel(MedicalShift? shift, string? internshipName = null)
        {
            if (shift == null)
            {
                return null;
            }

            return new MedicalShiftViewModel
            {
                ShiftId = shift.ShiftId,
                Date = shift.Date,
                Hours = shift.Hours,
                Minutes = shift.Minutes,
                Location = shift.Location,
                Year = shift.Year,
                InternshipName = internshipName ?? string.Empty,
                SyncStatus = shift.SyncStatus,
            };
        }

        /// <summary>
        /// Konwertuje ViewModel na model danych MedicalShift.
        /// </summary>
        /// <param name="internshipId">Identyfikator stażu, do którego należy dyżur.</param>
        /// <returns>Obiekt modelu dyżuru.</returns>
        public MedicalShift ToModel(int internshipId)
        {
            return new MedicalShift
            {
                ShiftId = this.ShiftId,
                InternshipId = internshipId,
                Date = this.Date,
                Hours = this.Hours,
                Minutes = this.Minutes,
                Location = this.Location,
                Year = this.Year,
                SyncStatus = this.SyncStatus,
            };
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