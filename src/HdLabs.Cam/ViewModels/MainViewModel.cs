using HdLabs.Common.Mvvm;

namespace HdLabs.Cam.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private string _status = "Ready";

    public string Title => "EzLabs Cam";

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public RelayCommand SetReadyCommand { get; }

    public MainViewModel()
    {
        SetReadyCommand = new RelayCommand(() => Status = "Ready");
    }
}
