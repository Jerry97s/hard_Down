using System.Windows;
using HdLabs.Zip.ViewModels;

namespace HdLabs.Zip;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}