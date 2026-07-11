using Avalonia.Controls;
using GoldBoxPlayer.ViewModels;
using MiniAudioEx.Core.StandardAPI;
using System;

namespace GoldBoxPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RequestAnimationFrame(OnAudioUpdateLoop);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        (DataContext as MainViewModel).Close();
    }

    private void OnAudioUpdateLoop(TimeSpan obj)
    {
        // 3. Call your Audio Update 
        AudioContext.Update();

        // 4. Request the next frame loop immediately to keep it running
        RequestAnimationFrame(OnAudioUpdateLoop);
    }
}
