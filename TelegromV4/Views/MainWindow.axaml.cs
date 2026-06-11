using Avalonia.Controls;
using TelegromV4.ViewModels;

namespace TelegromV4.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}