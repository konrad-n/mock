using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures;

public partial class ProceduresPage : ContentPage
{
    private readonly ProceduresViewModel _viewModel;

    public ProceduresPage(ProceduresViewModel viewModel)
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