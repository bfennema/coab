using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.Reflection.Metadata;

namespace GoldBoxPlayer.Views;

public partial class MainView : UserControl
{
    private Settings settings;
    public MainView()
    {
        InitializeComponent();
    }

    public void UpdateMenuIsChecked(Settings _settings)
    {
        settings = _settings;
        if (settings.Game == Logging.Game.PoolOfRadiance)
        {
            PoolRadMenu.IsChecked = true;
        }
        else if (settings.Game == Logging.Game.CurseOfTheAzureBonds)
        {
            CurseMenu.IsChecked = true;
        }
        else if (settings.Game == Logging.Game.SecretOfTheSilverBlades)
        {
            SecretMenu.IsChecked = true;
        }
        else if (settings.Game == Logging.Game.ChampionsOfKrynn)
        {
            ChampMenu.IsChecked = true;
        }

        if (settings.PlayerAlwaysSaves)
        {
            PlayerAlwaysMakesSavingThrows.IsChecked = true;
        }
        if (settings.AlwaysShowAreaMap)
        {
            AllowAreaMap.IsChecked = true;
        }
        if (settings.FreeTraining)
        {
            FreeTraining.IsChecked = true;
        }
        if (settings.SkipCopyProtection)
        {
            SkipCopyProtection.IsChecked = true;
        }
        if (settings.AllowPlayerModify)
        {
            AllowPlayerModify.IsChecked = true;
        }
        if (settings.AllowGodsIntervene)
        {
            AllowGodsIntervene.IsChecked = true;
        }
        if (settings.DisplayFullItemNames)
        {
            DisplayItemsFullName.IsChecked = true;
        }
        if (settings.ViewItemStats)
        {
            ViewItemsStats.IsChecked = true;
        }
        if (settings.SkipTitleScreen)
        {
            SkipTitleScreen.IsChecked = true;
        }
        if (settings.ImprovedAreaMap)
        {
            ImprovedAreaMap.IsChecked = true;
        }
        if (settings.NoRaceLevelLimits)
        {
            NoRaceLevelLimits.IsChecked = true;
        }
        if (settings.NoRaceClassLimits)
        {
            NoRaceClassRestrictions.IsChecked = true;
        }
        if (settings.SortTreasure)
        {
            SortTreasure.IsChecked = true;
        }

        if (settings.SoundOn)
        {
            SoundOn.IsChecked = true;
        }
        if (settings.AnimationOn)
        {
            AnimationsOn.IsChecked = true;
        }
        if (settings.PictureOn)
        {
            PicturesOn.IsChecked = true;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var insetsManager = TopLevel.GetTopLevel(this)?.InsetsManager;

        if (insetsManager != null)
        {
            insetsManager.DisplayEdgeToEdge = true;
            insetsManager.IsSystemBarVisible = false;
        }

        MainViewUserControl.Focus();
    }

    private void Debugging_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MenuItem menu = (MenuItem)sender;

        if (CommandDebugging == menu)
        {
            engine.seg043.ToggleCommandDebugging();
        }
        else if (DumpPlayerAffects == menu)
        {
            engine.seg043.DumpPlayerAffects();
        }
        else if (DumpMonsters == menu)
        {
            engine.seg043.DumpMonsters();
        }
        else if (DumpTreasureItems == menu)
        {
            engine.seg043.DumpTreasureItems();
        }
    }

    private void Cheats_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MenuItem menu = (MenuItem)sender;
        bool toggle = !menu.IsChecked;

        if (PlayerAlwaysMakesSavingThrows == menu)
        {
            settings.PlayerAlwaysSaves = toggle;
        }
        else if (AllowAreaMap == menu)
        {
            settings.AlwaysShowAreaMap = toggle;
        }
        else if (FreeTraining == menu)
        {
            settings.FreeTraining = toggle;
        }
        else if (SkipCopyProtection == menu)
        {
            settings.SkipCopyProtection = toggle;
        }
        else if (AllowPlayerModify == menu)
        {
            settings.AllowPlayerModify = toggle;
        }
        else if (AllowGodsIntervene == menu)
        {
            settings.AllowGodsIntervene = toggle;
        }
        else if (DisplayItemsFullName == menu)
        {
            settings.DisplayFullItemNames = toggle;
        }
        else if (ViewItemsStats == menu)
        {
            settings.ViewItemStats = toggle;
        }
        else if (SkipTitleScreen == menu)
        {
            settings.SkipTitleScreen = toggle;
        }
        else if (ImprovedAreaMap == menu)
        {
            settings.ImprovedAreaMap = toggle;
        }
        else if (NoRaceLevelLimits == menu)
        {
            settings.NoRaceLevelLimits = toggle;
        }
        else if (NoRaceClassRestrictions == menu)
        {
            settings.NoRaceClassLimits = toggle;
        }
        else if (SortTreasure == menu)
        {
            settings.SortTreasure = toggle;
        }
    }
    private void Options_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MenuItem menu = (MenuItem)sender;
        bool toggle = !menu.IsChecked;

        if (SoundOn == menu)
        {
            settings.SoundOn = toggle;
        }
        else if (AnimationsOn == menu)
        {
            settings.AnimationOn = toggle;
        }
        else if (PicturesOn == menu)
        {
            settings.PictureOn = toggle;
        }
    }
    private void Menu_PointerPressed(object? sender, PointerPressedEventArgs e)
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

    private void MainViewImage_KeyDown(object? sender, KeyEventArgs e)
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

    private void MainViewUserControl_SizeChanged(object? sender, SizeChangedEventArgs e)
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
            MainViewImage.Width = width;
            MainViewImage.Height = height;
        }
    }
}