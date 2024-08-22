using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using GoldBoxPlayer.ViewModels;

namespace GoldBoxPlayer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        MainViewUserControl.SizeChanged += Control_SizeChanged;
        //MainViewCanvas.Focusable = true;
        //canvas.SizeChanged += Canvas_SizeChanged;
        //canvas.PointerPressed += Canvas_PointerPressed;
        MainViewUserControl.KeyDown += Canvas_KeyDown;
        //canvas.KeyUp += Canvas_KeyUp;
        //MainViewCanvas.PointerEntered += Canvas_PointerEntered;
        //canvas.PointerExited += Canvas_PointerExited;
        //MainViewCanvas.Focusable = true;
        //canvas.Focus();
        //MainViewCanvas.SizeChanged += Canvas_SizeChanged;

        PoolRadMenu.PointerPressed += Menu_PointerPressed;
        CurseMenu.PointerPressed += Menu_PointerPressed;
        SecretMenu.PointerPressed += Menu_PointerPressed;
    }

    private void Menu_PointerPressed(object? sender, PointerEventArgs e)
    {
        MenuItem menu = (MenuItem)sender;
        if (menu.IsChecked == false)
        {
            menu.IsChecked = true;
            if (menu.Command != null)
            {
                menu.Command.Execute(menu.CommandParameter);
            }
        }
    }
    private void Canvas_PointerEntered(object? sender, PointerEventArgs e)
    {
        MainViewCanvas.Focus();
        MainViewCanvas.PointerEntered -= Canvas_PointerEntered;
        //(DataContext as MainViewModel).SetImage(this.FindControl<Image>("MainViewImage"));
    }

    private void Canvas_KeyDown(object? sender, KeyEventArgs e)
    {
         engine.seg049.AddKey(KeyToIBMKey(e.Key));
    }

    public static ushort KeyToIBMKey(Key key)
    {
        if (key >= Key.D0 && key <= Key.D9)
        {
            return (ushort)((key - Key.D0) + '0');
        }
        if (key >= Key.A && key <= Key.Z)
        {
            return (ushort)((key - Key.A) + 'a');
        }

        if (key == Key.Enter)
        {
            return 0x1C0D;
        }

        if (key == Key.Space)
        {
            return 0x20;
        }

        if (key == Key.Delete)
        {
            return 0x5300;
        }

        if (key == Key.Back)
        {
            return 0x08;
        }

        if (key == Key.Home || key == Key.NumPad7 || key == Key.OemOpenBrackets)
        {
            return 0x4700;
        }

        if (key == Key.Up || key == Key.NumPad8)
        {
            return 0x4800;
        }

        if (key == Key.PageUp || key == Key.NumPad9)
        {
            return 0x4900;
        }

        if (key == Key.Left || key == Key.NumPad4)
        {
            return 0x4B00;
        }

        if (key == Key.NumPad5)
        {
            return 0x4C00;
        }

        if (key == Key.Right || key == Key.NumPad6)
        {
            return 0x4D00;
        }


        if (key == Key.End || key == Key.NumPad1 || key == Key.OemCloseBrackets)
        {
            return 0x4F00;
        }

        if (key == Key.Down || key == Key.NumPad2)
        {
            return 0x5000;
        }

        if (key == Key.PageDown || key == Key.NumPad3)
        {
            return 0x5100;
        }

        if (key == Key.OemMinus)
        {
            return 0x2d00;
        }

        if (key == Key.Escape)
        {
            return 0x1b;
        }

        if (key == Key.OemComma)
        {
            return 0x2c;
        }

        if (key == Key.OemPeriod)
        {
            return 0x2e;
        }

        return 0x0020;
    }
    private void Canvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged || e.HeightChanged)
        {
            var size = e.NewSize;
        }
    }
    private void Control_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged || e.HeightChanged)
        {
            var size = e.NewSize;
            double scale;
            double scale_height = size.Height / 200;
            double scale_width = size.Width / 320;
            if (scale_height > scale_width)
            {
                scale = scale_width; 
            }
            else
            {
                scale = scale_height;
            }
            int height = (int)(200 * scale);
            int width = (int)(320 * scale);
            MainViewCanvas.Width = width;
            MainViewCanvas.Height = height;
            MainViewImage.Width = width;
            MainViewImage.Height = height;
        }
    }
}