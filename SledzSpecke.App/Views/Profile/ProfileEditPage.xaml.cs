using SledzSpecke.App.ViewModels.Profile;

namespace SledzSpecke.App.Views.Profile;

public partial class ProfileEditPage : ContentPage
{
    private readonly ProfileEditViewModel _viewModel;

    public ProfileEditPage(ProfileEditViewModel viewModel)
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
