using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;
using AvaloniaApplication1.ViewModels;
using System.Drawing;

namespace AvaloniaApplication1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        var canvas = this.FindControl<Canvas>("MainViewCanvas");
        if (canvas != null)
        {
            canvas.SizeChanged += Canvas_SizeChanged;
            canvas.PointerPressed += Canvas_PointerPressed;
            canvas.KeyDown += Canvas_KeyDown;
            canvas.KeyUp += Canvas_KeyUp;
            canvas.PointerEntered += Canvas_PointerEntered;
            canvas.PointerExited += Canvas_PointerExited;
            canvas.Focusable = true;
            //canvas.Focus();
        }

        var image = this.FindControl<Image>("MainViewImage");
        if (image != null)
        {
            //(image.DataContext as MainViewModel).SetImage(image);
        }
    }
    private void Canvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {

    }

    public void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as MainViewModel).UpdateBitmap(this.FindControl<Image>("MainViewImage"));
    }

    private void Canvas_KeyDown(object? sender, KeyEventArgs e)
    {
    }

    private void Canvas_KeyUp(object? sender, KeyEventArgs e)
    {
    }
    private void Canvas_PointerEntered(object? sender, PointerEventArgs e)
    {
        Canvas canvas = (Canvas)sender;
        canvas.Focus();
    }

    private void Canvas_PointerExited(object? sender, PointerEventArgs e)
    {
        //Canvas canvas = (Canvas)sender;
        //var topLevel = TopLevel.GetTopLevel(this);
        //topLevel.Focus();
    }
}
