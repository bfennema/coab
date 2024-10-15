using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using GoldBoxPlayer.Services;
using GoldBoxPlayer.ViewModels;
using GoldBoxPlayer.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GoldBoxPlayer;

public partial class App : Application
{
    static Thread? engineThread;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        MainViewModel model = null;
        MainView view = null;
        Classes.gbl.file = new File();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            model = new MainViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = model
            };
            services.AddSingleton<IFilesService>(x => new FilesService(desktop.MainWindow));
            model.LoadConfigs(desktop.MainWindow);
            view = desktop.MainWindow.GetControl<MainView>("MainView");
            File.SetStorageProvider(desktop.MainWindow.StorageProvider);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            model = new MainViewModel();
            singleViewPlatform.MainView = new MainView
            {
                DataContext = model
            };
            services.AddSingleton<IFilesService>(x => new FilesService(singleViewPlatform.MainView));
            model.LoadConfigs(TopLevel.GetTopLevel(singleViewPlatform.MainView));
            view = (MainView)singleViewPlatform.MainView;
            File.SetStorageProvider(TopLevel.GetTopLevel(singleViewPlatform.MainView).StorageProvider);
        }
        else
        {
            return;
        }

        model.SetImage(view.GetControl<Image>("MainViewImage"));

        view.UpdateMenuIsChecked(model.Settings);

        Services = services.BuildServiceProvider();

        base.OnFrameworkInitializationCompleted();

        if (!Design.IsDesignMode)
        {
            engineThread = new Thread(() => EngineThread(model));
            engineThread.Name = "Engine";
            engineThread.Start();
        }
    }

    public new static App? Current => Application.Current as App;

    public IServiceProvider? Services { get; private set; }


    static void EngineThread(MainViewModel model)
    {
        engine.seg001.__SystemInit(model.EngineStopped, "GoldBoxPlayer.Desktop.Properties.Resources");
        Classes.gbl.games[(int)Logging.Game.PoolOfRadiance] = new Classes.PoolRad.Game();
        Classes.gbl.games[(int)Logging.Game.CurseOfTheAzureBonds] = new Classes.Curse.Game();
        Classes.gbl.games[(int)Logging.Game.SecretOfTheSilverBlades] = new Classes.Secret.Game();
        Classes.gbl.games[(int)Logging.Game.ChampionsOfKrynn] = new Classes.Champ.Game();
        engine.seg001.PROGRAM();

        model.EngineStopped();
    }
}
