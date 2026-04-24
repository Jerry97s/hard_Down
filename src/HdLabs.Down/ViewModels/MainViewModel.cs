using HdLabs.Common.Mvvm;

namespace HdLabs.Down.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private string _url = "";
    private string _status = "Paste a URL (next step).";

    public string Title => "EzLabs Down";

    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public RelayCommand AnalyzeCommand { get; }

    public MainViewModel()
    {
        AnalyzeCommand = new RelayCommand(() =>
        {
            Status = string.IsNullOrWhiteSpace(Url) ? "Enter a URL." : $"(Next) Would analyze: {Url}";
        });
    }
}
