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
            Classes.gbl.StorageProvider = desktop.MainWindow.StorageProvider;
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
            Classes.gbl.StorageProvider = TopLevel.GetTopLevel(singleViewPlatform.MainView).StorageProvider;
        }
        else
        {
            return;
        }

        model.SetImage(view.GetControl<Image>("MainViewImage"));
        if (Logging.Config.GetGame() == Logging.Game.PoolOfRadiance)
        {
            view.GetControl<MenuItem>("PoolRadMenu").IsChecked = true;
        }
        else if (Logging.Config.GetGame() == Logging.Game.CurseOfTheAzureBonds)
        {
            view.GetControl<MenuItem>("CurseMenu").IsChecked = true;
        }
        else if (Logging.Config.GetGame() == Logging.Game.SecretOfTheSilverBlades)
        {
            view.GetControl<MenuItem>("SecretMenu").IsChecked = true;
        }

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
        engine.seg001.__SystemInit(model.EngineStopped);
        engine.seg001.PROGRAM();

        model.EngineStopped();
    }
}
