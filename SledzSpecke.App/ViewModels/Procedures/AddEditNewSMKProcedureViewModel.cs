using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), nameof(ProcedureId))]
    [QueryProperty(nameof(RequirementId), nameof(RequirementId))]
    public class AddEditNewSMKProcedureViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;

        private string procedureId;
        private string requirementId;
        private string procedureName;
        private int countA;
        private int countB;
        private DateTime startDate;
        private DateTime endDate;
        private RealizedProcedureNewSMK procedure;
        private bool isEdit;

        public AddEditNewSMKProcedureViewModel(
            IProcedureService procedureService,
            IDialogService dialogService)
        {
            this.procedureService = procedureService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Inicjalizacja dat
            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;

            this.Title = "Dodaj realizację procedury";
        }

        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
                this.LoadProcedureAsync().ConfigureAwait(false);
            }
        }

        public string RequirementId
        {
            set
            {
                this.requirementId = value;
                this.LoadRequirementAsync().ConfigureAwait(false);
            }
        }

        public string ProcedureName
        {
            get => this.procedureName;
            set => this.SetProperty(ref this.procedureName, value);
        }

        public int CountA
        {
            get => this.countA;
            set
            {
                if (this.SetProperty(ref this.countA, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public int CountB
        {
            get => this.countB;
            set
            {
                if (this.SetProperty(ref this.countB, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (this.SetProperty(ref this.startDate, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (this.SetProperty(ref this.endDate, value))
                {
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadProcedureAsync()
        {
            if (this.IsBusy || string.IsNullOrEmpty(this.procedureId))
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                if (int.TryParse(this.procedureId, out int id))
                {
                    this.procedure = await this.procedureService.GetNewSMKProcedureAsync(id);
                    if (this.procedure != null)
                    {
                        this.isEdit = true;
                        this.Title = "Edytuj realizację procedury";

                        this.CountA = this.procedure.CountA;
                        this.CountB = this.procedure.CountB;
                        this.StartDate = this.procedure.StartDate;
                        this.EndDate = this.procedure.EndDate;
                        this.ProcedureName = this.procedure.ProcedureName;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania procedury.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadRequirementAsync()
        {
            if (this.IsBusy || string.IsNullOrEmpty(this.requirementId))
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                if (int.TryParse(this.requirementId, out int id))
                {
                    var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync();
                    var requirement = requirements.FirstOrDefault(r => r.Id == id);
                    if (requirement != null)
                    {
                        this.ProcedureName = requirement.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania wymagania: {ex.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private bool CanSave()
        {
            return (this.CountA > 0 || this.CountB > 0) &&
                   this.StartDate <= this.EndDate;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                

                // Przygotuj obiekt procedury
                var procedureToSave = this.procedure ?? new RealizedProcedureNewSMK();

                procedureToSave.CountA = this.CountA;
                procedureToSave.CountB = this.CountB;
                procedureToSave.StartDate = this.StartDate;
                procedureToSave.EndDate = this.EndDate;
                procedureToSave.ProcedureName = this.ProcedureName;

                if (!this.isEdit && !string.IsNullOrEmpty(this.requirementId))
                {
                    procedureToSave.ProcedureRequirementId = int.Parse(this.requirementId);
                }

                procedureToSave.ModuleId = // tutaj dodaj id modulu do zapisania.

                // Zapisz procedurę
                bool success = await this.procedureService.SaveNewSMKProcedureAsync(procedureToSave);

                if (success)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        this.isEdit ? "Realizacja procedury została zaktualizowana." : "Realizacja procedury została dodana.",
                        "OK");

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać realizacji procedury.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zapisywania realizacji procedury.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}