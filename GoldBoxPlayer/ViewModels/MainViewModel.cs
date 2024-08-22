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
using System.Reflection.Metadata;
using System.ComponentModel;

namespace GoldBoxPlayer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private WriteableBitmap? _bitmap;
    private Image? _image;
    IStorageFolder? _PoolRadData;
    IStorageFolder? _PoolRadSave;
    IStorageFolder? _CurseData;
    IStorageFolder? _CurseSave;
    IStorageFolder? _SecretData;
    IStorageFolder? _SecretSave;

    public MainViewModel()
    {
        SelectDirectoryCommand = ReactiveCommand.CreateFromTask<string>(RunSelectDirectoryCommand);
        SelectGameCommand = ReactiveCommand.Create<Logging.Game>(RunSelectGameCommand);
    }

    public ReactiveCommand<string, Unit> SelectDirectoryCommand { get; }
    public ReactiveCommand<Logging.Game, Unit> SelectGameCommand { get; }

    public async void LoadConfigs(TopLevel top)
    {
        string path = Logging.Config.GetSavePath(Logging.Game.PoolOfRadiance);
        IStorageFolder folder;
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _PoolRadSave, folder, nameof(PoolRadSave));
        }
        path = Logging.Config.GetDataPath(Logging.Game.PoolOfRadiance);
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _PoolRadData, folder, nameof(PoolRadData));
        }
        path = Logging.Config.GetSavePath(Logging.Game.CurseOfTheAzureBonds);
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _CurseSave, folder, nameof(CurseSave));
        }
        path = Logging.Config.GetDataPath(Logging.Game.CurseOfTheAzureBonds);
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _CurseData, folder, nameof(CurseData));
        }
        path = Logging.Config.GetSavePath(Logging.Game.SecretOfTheSilverBlades);
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _SecretSave, folder, nameof(SecretSave));
        }
        path = Logging.Config.GetDataPath(Logging.Game.SecretOfTheSilverBlades);
        if (path != "")
        {
            folder = await top.StorageProvider.OpenFolderBookmarkAsync(path);
            this.RaiseAndSetIfChanged(ref _SecretData, folder, nameof(SecretData));
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

        var folder = await filesService.OpenFolderAsync(parameter, startLocation);
        if (folder is null) return;

        string path = folder.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

        if (parameter == "PoolRadSave")
        {
            this.RaiseAndSetIfChanged(ref _PoolRadSave, folder, nameof(PoolRadSave));
            Logging.Config.SetSavePath(Logging.Game.PoolOfRadiance, folder);
        }
        else if (parameter == "PoolRadData")
        {
            this.RaiseAndSetIfChanged(ref _PoolRadData, folder, nameof(PoolRadData));
            Logging.Config.SetDataPath(Logging.Game.PoolOfRadiance, folder);
        }
        else if (parameter == "CurseSave")
        {
            this.RaiseAndSetIfChanged(ref _CurseSave, folder, nameof(CurseSave));
            Logging.Config.SetSavePath(Logging.Game.CurseOfTheAzureBonds, folder);
        }
        else if (parameter == "CurseData")
        {
            this.RaiseAndSetIfChanged(ref _CurseData, folder, nameof(CurseData));
            Logging.Config.SetDataPath(Logging.Game.CurseOfTheAzureBonds, folder);
        }
        else if (parameter == "SecretSave")
        {
            this.RaiseAndSetIfChanged(ref _SecretSave, folder, nameof(SecretSave));
            Logging.Config.SetSavePath(Logging.Game.SecretOfTheSilverBlades, folder);
        }
        else if (parameter == "SecretData")
        {
            this.RaiseAndSetIfChanged(ref _SecretData, folder, nameof(SecretData));
            Logging.Config.SetDataPath(Logging.Game.SecretOfTheSilverBlades, folder);
        }
    }

    public void RunSelectGameCommand(Logging.Game parameter)
    {
        if (parameter == Logging.Game.PoolOfRadiance)
        {
            Logging.Config.SetGame(Logging.Game.PoolOfRadiance);
        }
        else if (parameter == Logging.Game.CurseOfTheAzureBonds)
        {
            Logging.Config.SetGame(Logging.Game.CurseOfTheAzureBonds);
        }
        else if (parameter == Logging.Game.SecretOfTheSilverBlades)
        {
            Logging.Config.SetGame(Logging.Game.SecretOfTheSilverBlades);
        }
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

    public void UpdateDisplayCallback()
    {
        try
        {
            Dispatcher.UIThread.Invoke(new Action(UpdateDisplayCallback2));
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


    public void UpdateDisplayCallback2()
    {
        MainViewBitmap = Classes.Display.bm;
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
        get => "Data - " + (_SecretData != null ? _CurseData.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
    public string SecretSave
    {
        get => "Save - " + (_SecretSave != null ? _CurseSave.Path.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : "");
    }
}
