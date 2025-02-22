using SledzSpecke.App.ViewModels.Duties;

namespace SledzSpecke.App.Views.Duties;

public partial class DutyAddPage : ContentPage
{
    private readonly DutyAddViewModel _viewModel;

    public DutyAddPage(DutyAddViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.InitializeAsync();
    }
}