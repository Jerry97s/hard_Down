using System.Windows;
using HdLabs.Down.ViewModels;

namespace HdLabs.Down;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}