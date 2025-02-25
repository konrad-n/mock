using SledzSpecke.App.ViewModels.Statistics;

namespace SledzSpecke.App.Views.Statistics;

public partial class SpecializationStatsPage : ContentPage
{
    private readonly SpecializationStatsViewModel _viewModel;

    public SpecializationStatsPage(SpecializationStatsViewModel viewModel)
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
