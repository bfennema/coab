using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private WriteableBitmap? _bitmap;
    int _index = 0;


    public MainViewModel()
    {
        _bitmap = new WriteableBitmap(new Avalonia.PixelSize(320, 200), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormats.Rgb24);
        _index = 1;
        UpdateBitmap(null);
    }

    public void UpdateBitmap(Image image)
    {
        byte[] array = ChooseColor(_index);
        using (var fb = _bitmap.Lock())
        {
            Marshal.Copy(array, 0, fb.Address, array.Length);
            image?.InvalidateVisual();
        }
        _index = (_index + 1) % 3;
    }

    byte[] ChooseColor(int index)
    {
        byte[] array = new byte[320 * 200 * 3];

        for (int i = 0; i < 320 * 200 * 3; i += 3)
        {
            switch (index)
            {
                case 0:
                    array[i + 0] = 0xFF;
                    array[i + 1] = 0x00;
                    array[i + 2] = 0x00;
                    break;
                case 1:
                    array[i + 0] = 0x00;
                    array[i + 1] = 0xFF;
                    array[i + 2] = 0x00;
                    break;
                case 2:
                    array[i + 0] = 0x00;
                    array[i + 1] = 0x00;
                    array[i + 2] = 0xFF;
                    break;
            }
        }

        return array;
    }

    public WriteableBitmap? MainViewBitmap
    {
        get => _bitmap;
        private set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }
}
