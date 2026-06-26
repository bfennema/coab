using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using GoldBoxPlayer.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace GoldBoxPlayer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private WriteableBitmap _bitmap;
    private Image? _image;
    private CancellationTokenSource? _cts;
    IStorageFolder? _PoolRadData;
    IStorageFolder? _PoolRadSave;
    IStorageFolder? _CurseData;
    IStorageFolder? _CurseSave;
    IStorageFolder? _SecretData;
    IStorageFolder? _SecretSave;
    IStorageFolder? _ChampData;
    IStorageFolder? _ChampSave;
    Settings _settings;

    public MainViewModel()
    {
        _bitmap = new WriteableBitmap(new Avalonia.PixelSize(320, 200), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormats.Bgr24);
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask<string>(RunSelectDirectoryCommand);
        SelectGameCommand = ReactiveCommand.Create<Logging.Game>(RunSelectGameCommand);
        _settings = Settings.LoadSettings(Logging.Config.AppDataPath, "", Logging.Config.SaveBasePath);
        _settings.Set();
    }

    public ReactiveCommand<string, Unit> SelectDirectoryCommand { get; }
    public ReactiveCommand<Logging.Game, Unit> SelectGameCommand { get; }

    public async void LoadConfigs(TopLevel top)
    {
        string path = _settings.PoolOfRadianceSavePath;
        IStorageFolder folder;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _PoolRadSave, folder, nameof(PoolRadSave));
        }
        path = _settings.PoolOfRadianceDataPath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _PoolRadData, folder, nameof(PoolRadData));
        }
        path = _settings.CurseOfTheAzureBondsSavePath;
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
        path = _settings.SecretOfTheSilverBladesSavePath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _SecretSave, folder, nameof(SecretSave));
        }
        path = _settings.SecretOfTheSilverBladesDataPath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _SecretData, folder, nameof(SecretData));
        }
        path = _settings.ChampionsOfKrynnSavePath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _ChampSave, folder, nameof(ChampSave));
        }
        path = _settings.ChampionsOfKrynnDataPath;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _ChampData, folder, nameof(ChampData));
        }
    }

    public async Task RunSelectDirectoryCommand(string parameter)
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        IStorageFolder? startLocation = null;

        if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

        if (parameter == "PoolRadSave")
        {
            startLocation = _PoolRadSave;
        }
        else if (parameter == "PoolRadData")
        {
            startLocation = _PoolRadData;
        }
        else if (parameter == "CurseSave")
        {
            startLocation = _CurseSave;
        }
        else if (parameter == "CurseData")
        {
            startLocation = _CurseData;
        }
        else if (parameter == "SecretSave")
        {
            startLocation = _SecretSave;
        }
        else if (parameter == "SecretData")
        {
            startLocation = _SecretData;
        }
        else if (parameter == "ChampSave")
        {
            startLocation = _ChampSave;
        }
        else if (parameter == "ChampData")
        {
            startLocation = _ChampData;
        }

        var folder = await filesService.OpenFolderAsync(parameter, startLocation);
        if (folder is null) return;

        string path = folder.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

        if (parameter == "PoolRadSave")
        {
            this.RaiseAndSetIfChanged(ref _PoolRadSave, folder, nameof(PoolRadSave));
            _settings.PoolOfRadianceSavePath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "PoolRadData")
        {
            this.RaiseAndSetIfChanged(ref _PoolRadData, folder, nameof(PoolRadData));
            _settings.PoolOfRadianceDataPath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "CurseSave")
        {
            this.RaiseAndSetIfChanged(ref _CurseSave, folder, nameof(CurseSave));
            _settings.CurseOfTheAzureBondsSavePath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "CurseData")
        {
            this.RaiseAndSetIfChanged(ref _CurseData, folder, nameof(CurseData));
            _settings.CurseOfTheAzureBondsDataPath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "SecretSave")
        {
            this.RaiseAndSetIfChanged(ref _SecretSave, folder, nameof(SecretSave));
            _settings.SecretOfTheSilverBladesSavePath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "SecretData")
        {
            this.RaiseAndSetIfChanged(ref _SecretData, folder, nameof(SecretData));
            _settings.SecretOfTheSilverBladesDataPath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "ChampSave")
        {
            this.RaiseAndSetIfChanged(ref _ChampSave, folder, nameof(ChampSave));
            _settings.ChampionsOfKrynnSavePath = await folder.SaveBookmarkAsync();
        }
        else if (parameter == "ChampData")
        {
            this.RaiseAndSetIfChanged(ref _ChampData, folder, nameof(ChampData));
            _settings.ChampionsOfKrynnDataPath = await folder.SaveBookmarkAsync();
        }
    }

    public void RunSelectGameCommand(Logging.Game parameter)
    {
        _settings.Game = parameter;
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
        StopEngine();
        //Classes.gbl.Exit = true;
        //Classes.Input.AddKey('Q');
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

    public async Task StartEngineAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Run(() => App.EngineThread(this, _cts.Token), _cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
            EngineStopped();
        }
    }

    public void StopEngine() => _cts?.Cancel();

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

    public static event Action? MapWindowRefreshed;

    public void UpdateDisplay()
    {
        if (Classes.gbl.saveData != null && Classes.gbl.geo_ptr?.maps != null)
        {
            Classes.MapTracker.MarkExplored(Classes.gbl.game_area, Classes.gbl.area_ptr.current_3DMap_block_id, Classes.gbl.mapPosX, Classes.gbl.mapPosY);
            MapWindowRefreshed?.Invoke();
        }
        _image?.InvalidateVisual();
    }

    public WriteableBitmap? MainViewBitmap
    {
        get => _bitmap;
        private set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    public string PoolRadData
    {
        get => "Data - " + (_PoolRadData != null ? _PoolRadData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string PoolRadSave
    {
        get => "Save - " + (_PoolRadSave != null ? _PoolRadSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string CurseData
    {
        get => "Data - " + (_CurseData != null ? _CurseData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string CurseSave
    {
        get => "Save - " + (_CurseSave != null ? _CurseSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string SecretData
    {
        get => "Data - " + (_SecretData != null ? _SecretData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string SecretSave
    {
        get => "Save - " + (_SecretSave != null ? _SecretSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string ChampData
    {
        get => "Data - " + (_ChampData != null ? _ChampData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string ChampSave
    {
        get => "Save - " + (_ChampSave != null ? _ChampSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
}
