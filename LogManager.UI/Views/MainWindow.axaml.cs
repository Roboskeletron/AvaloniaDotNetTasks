using Avalonia.Controls;
using LogManager.UI.ViewModels;

namespace LogManager.UI.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(GetTopLevel(this)!.StorageProvider);
        InitializeComponent();
    }
}