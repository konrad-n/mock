using SledzSpecke.App.ViewModels.Specializations;

namespace SledzSpecke.App.Views.Specialization;

public partial class SpecializationProgressPage : ContentPage
{
    private readonly SpecializationProgressViewModel _viewModel;

    public SpecializationProgressPage(SpecializationProgressViewModel viewModel)
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
