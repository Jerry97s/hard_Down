using System.Windows;
using HdLabs.Finder.ViewModels;

namespace HdLabs.Finder;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}