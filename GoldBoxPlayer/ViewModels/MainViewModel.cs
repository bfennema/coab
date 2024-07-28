using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GoldBoxPlayer.Services;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.Controls.ApplicationLifetimes;

namespace GoldBoxPlayer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private WriteableBitmap _bitmap;
    private Image? _image;
    IStorageFolder? _CurseData;
    IStorageFolder? _CurseSave;
    Settings _settings;

    public MainViewModel()
    {
        _bitmap = new WriteableBitmap(new Avalonia.PixelSize(320, 200), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormats.Bgr24);
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask<string>(RunSelectDirectoryCommand);
        _settings = Settings.LoadSettings(Logging.Config.AppDataPath, "", Logging.Config.SaveBasePath);
        _settings.Set();
    }

    public ReactiveCommand<string, Unit> SelectDirectoryCommand { get; }

    public async void LoadConfigs(TopLevel top)
    {
        IStorageFolder folder;
        string path = _settings.CurseOfTheAzureBondsSavePath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _CurseSave, folder, nameof(CurseSave));
        }
        path = _settings.CurseOfTheAzureBondsDataPath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _CurseData, folder, nameof(CurseData));
        }
    }

    public async Task RunSelectDirectoryCommand(string parameter)
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        IStorageFolder? startLocation = null;

        if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

        if (parameter == "CurseSave")
        {
            startLocation = _CurseSave;
        }
        else if (parameter == "CurseData")
        {
            startLocation = _CurseData;
        }

        var folder = await filesService.OpenFolderAsync(parameter, startLocation);
        if (folder is null) return;

        string path = folder.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

        if (parameter == "CurseSave")
        {
            this.RaiseAndSetIfChanged(ref _CurseSave, folder, nameof(CurseSave));
            _settings.CurseOfTheAzureBondsSavePath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "CurseData")
        {
            this.RaiseAndSetIfChanged(ref _CurseData, folder, nameof(CurseData));
            _settings.CurseOfTheAzureBondsDataPath = await folder.SaveBookmarkAsync();
        }
    }

    internal Settings Settings
    {
        get => _settings;
    }

    public void SetImage(Image image)
    {
        _image = image;
        Classes.Display.UpdateCallback = UpdateDisplayCallback;
    }

    public void Close()
    {
        Classes.gbl.Exit = true;
    }

    public void UpdateDisplayCallback(byte[] videoRam, int videoRamSize)
    {
        try
        {
            using (var fb = _bitmap.Lock())
            {
                System.Runtime.InteropServices.Marshal.Copy(videoRam, 0, fb.Address, videoRamSize);
            }
            Dispatcher.UIThread.Invoke(new Action(UpdateDisplay));
        }
        catch (Exception ex)
        {
        }
    }

    public void EngineStopped()
    {
        try
        {
            Dispatcher.UIThread.Invoke(new Action(EngineStopped2));
        }
        catch (Exception ex)
        {

        }
    }

    public void EngineStopped2()
    {
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }


    public void UpdateDisplay()
    {
        _image?.InvalidateVisual();
    }

    public WriteableBitmap? MainViewBitmap
    {
        get => _bitmap;
        private set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    public string CurseData
    {
        get => "Data - " + (_CurseData != null ? _CurseData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string CurseSave
    {
        get => "Save - " + (_CurseSave != null ? _CurseSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
}
