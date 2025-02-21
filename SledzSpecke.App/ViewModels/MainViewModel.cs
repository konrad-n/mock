using SledzSpecke.Core.Interfaces.Services;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }
}