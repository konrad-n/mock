using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures;

[QueryProperty(nameof(ProcedureId), "id")]
public partial class ProcedureEditPage : ContentPage
{
    private readonly ProcedureEditViewModel _viewModel;

    public ProcedureEditPage(ProcedureEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public int ProcedureId
    {
        set
        {
            _viewModel.LoadProcedure(value);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}
