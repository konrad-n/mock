using SledzSpecke.App.ViewModels.Duties;

namespace SledzSpecke.App.Views.Duties;

[QueryProperty(nameof(DutyId), "id")]
public partial class DutyEditPage : ContentPage
{
    private readonly DutyEditViewModel _viewModel;

    public DutyEditPage(DutyEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public int DutyId
    {
        set
        {
            _viewModel.LoadDuty(value);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}