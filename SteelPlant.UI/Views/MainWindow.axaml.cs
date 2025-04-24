using Avalonia.Controls;
using SteelPlant.UI.ViewModels;

namespace SteelPlant.UI.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        (DataContext as MainWindowViewModel)?.StopAll();
    }
}