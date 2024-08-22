using Avalonia.Controls;
using GoldBoxPlayer.ViewModels;
using System.ComponentModel;

namespace GoldBoxPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        (DataContext as MainViewModel).Close();
    }
}
