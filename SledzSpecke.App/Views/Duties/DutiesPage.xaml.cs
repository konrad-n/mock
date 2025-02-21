using SledzSpecke.App.ViewModels.Duties;

namespace SledzSpecke.App.Views.Duties;

public partial class DutiesPage : ContentPage
{
    private readonly DutiesViewModel _viewModel;

    public DutiesPage(DutiesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.LoadDataAsync();
    }
}