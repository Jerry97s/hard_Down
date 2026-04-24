using System.Windows;
using HdLabs.Capture.ViewModels;

namespace HdLabs.Capture;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}