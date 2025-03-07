using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.Views.MedicalShifts
{
    [QueryProperty(nameof(ShiftId), nameof(ShiftId))]
    public partial class AddEditMedicalShiftPage : ContentPage
    {
        private readonly AddEditMedicalShiftViewModel viewModel;
        private int? shiftId;

        public string ShiftId
        {
            set
            {
                if (int.TryParse(value, out int id))
                {
                    this.shiftId = id;
                }
            }
        }

        public AddEditMedicalShiftPage(AddEditMedicalShiftViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Inicjalizacja ViewModel z przekazanym ID dyżuru
            await this.viewModel.InitializeAsync(this.shiftId);
        }
    }
}