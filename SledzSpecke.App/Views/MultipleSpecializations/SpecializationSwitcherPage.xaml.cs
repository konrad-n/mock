namespace SledzSpecke.App.Views.MultipleSpecialization;

public partial class SpecializationSwitcherPage : ContentPage
{
    // private readonly SpecializationSwitcherViewModel _viewModel;

    public SpecializationSwitcherPage(/*SpecializationSwitcherViewModel viewModel*/)
    {
        InitializeComponent();
        // _viewModel = viewModel;
        // BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Delay(100);// _viewModel.LoadDataAsync();
    }
}
