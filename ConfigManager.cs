using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT;

namespace FileHasher
{
    /// <summary>
    /// Manages the application configuration.
    /// </summary>
    public static class ConfigManager
    {
        private static SystemBackdropConfiguration? _sysBgAcrylicConfigSrc;
        private static DesktopAcrylicController? _sysBgAcrylicController;
        private static IPropertySet _localCfg => ApplicationData.Current.LocalSettings.Values; // Shortcut
        
        public static ElementTheme Theme
        {
            get
            {
                int val;
                if (_localCfg.TryGetValue("Theme", out object? v))
                {
                    val = (int)v;
                }
                else
                {
                    val = 0; // 0 = ElementTheme.Default
                }

                return (ElementTheme)val;
            }
            set
            {
                _localCfg["Theme"] = (int)value;
                if (Tracker.RootPage != null)
                {
                    Tracker.RootPage.RequestedTheme = value;
                }
            }
        }

        public static NavigationViewPaneDisplayMode NavBarPaneNavigMode
        {
            get
            {
                int val;
                if (_localCfg.TryGetValue("NavBarPaneNavigMode", out object? mode))
                {
                    val = (int)mode;
                }
                else
                {
                    val = 0; // 0 = Auto
                }

                return (NavigationViewPaneDisplayMode)val;
            }
            set
            {
                _localCfg["NavBarPaneNavigMode"] = (int)value;
                if (Tracker.RootPage != null)
                {
                    Tracker.RootPage.NavBar.PaneDisplayMode = value;
                    if (value == NavigationViewPaneDisplayMode.Top)
                    {
                        Grid.SetRow(Tracker.RootPage.NavBar, 1);
                    }
                    else
                    {
                        Grid.SetRow(Tracker.RootPage.NavBar, 0);
                    }
                }
            }
        }

        public static bool SoundEnabled
        {
            get
            {
                bool val;
                if (_localCfg.TryGetValue("SoundEnabled", out object? v))
                {
                    val = (bool)v;
                }
                else
                {
                    val = false;
                }

                return val;
            }
            set
            {
                _localCfg["SoundEnabled"] = value;
                if (Tracker.RootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.State = value ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
                }
            }
        }

        public static bool SpatialSoundEnabled
        {
            get
            {
                bool val;
                if (_localCfg.TryGetValue("SpatialSoundEnabled", out object? v))
                {
                    val = (bool)v;
                }
                else
                {
                    val = false;
                }

                return val;
            }
            set
            {
                _localCfg["SpatialSoundEnabled"] = value;

                if (Tracker.RootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.SpatialAudioMode = value ? ElementSpatialAudioMode.On : ElementSpatialAudioMode.Off;
                }
            }
        }

        public static double SoundVolume
        {
            get
            {
                double val;
                if (_localCfg.TryGetValue("SoundVolume", out object? v))
                {
                    val = (double)v;
                }
                else
                {
                    val = 1.0;
                }

                return val;
            }
            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("SoundVolume", "SoundVolume must be a 0-100% percentage (0.0 - 1.0 in code).");
                }
                _localCfg["SoundVolume"] = value;
                if (Tracker.RootPage != null) // Even though this doesn't require a page, this validates that the ConfigManager isn't in save-only mode.
                {
                    ElementSoundPlayer.Volume = value;
                }
            }
        }

        public static SysBackdropType WindowBackdrop
        {
            get
            {
                SysBackdropType val;
                if (_localCfg.TryGetValue("WindowBackdrop", out object? v))
                {
                    val = (SysBackdropType)v;
                }
                else
                {
                    val = SysBackdropType.None;
                }

                return val;
            }
            set
            {
                _localCfg["WindowBackdrop"] = (int)value;
                if (Tracker.RootPage != null && Tracker.MainWindow != null)
                {
                    switch (value)
                    {
                        case SysBackdropType.None:
                            Tracker.RootPage.Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
                            DisposeAcrylicControllers();
                            Tracker.MainWindow.SystemBackdrop = null;
                            break;
                        case SysBackdropType.Mica:
                            Tracker.RootPage.Background = null;
                            DisposeAcrylicControllers();
                            Tracker.MainWindow.SystemBackdrop = new MicaBackdrop();
                            break;
                        case SysBackdropType.Mica2:
                            Tracker.RootPage.Background = null;
                            DisposeAcrylicControllers();
                            var bd = new MicaBackdrop();
                            bd.Kind = MicaKind.BaseAlt;
                            Tracker.MainWindow.SystemBackdrop = bd;
                            break;
                        case SysBackdropType.Acrylic:
                            Tracker.RootPage.Background = null;
                            DisposeAcrylicControllers();
                            Tracker.MainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
                            break;
                        case SysBackdropType.AcrylicThin:
                            Tracker.RootPage.Background = null;
                            Tracker.MainWindow.SystemBackdrop = new DesktopAcrylicBackdrop();
                            if (_sysBgAcrylicController != null)
                            {
                                _sysBgAcrylicController.Dispose();
                            }
                            _sysBgAcrylicConfigSrc = new();
                            _sysBgAcrylicConfigSrc.IsInputActive = true;
                            _sysBgAcrylicController = new();
                            _sysBgAcrylicController.Kind = DesktopAcrylicKind.Thin;
                            _sysBgAcrylicController.AddSystemBackdropTarget(Tracker.MainWindow.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                            _sysBgAcrylicController.SetSystemBackdropConfiguration(_sysBgAcrylicConfigSrc);

                            Tracker.MainWindow.Closed -= MainWindow_Closed;
                            Tracker.MainWindow.Closed += MainWindow_Closed;

                            Tracker.MainWindow.Activated -= MainWindow_Activated;
                            Tracker.MainWindow.Activated += MainWindow_Activated;

                            Tracker.RootPage.ActualThemeChanged -= RootPage_ActualThemeChanged;
                            Tracker.RootPage.ActualThemeChanged += RootPage_ActualThemeChanged;
                            break;
                    }
                }
            }
        }

        public static bool HashLiveUpdate
        {
            get
            {
                bool val;
                if (_localCfg.TryGetValue("HashLiveUpdate", out object? v))
                {
                    val = (bool)v;
                }
                else
                {
                    val = false;
                }

                return val;
            }
            set
            {
                _localCfg["HashLiveUpdate"] = value;
            }
        }

        private static void RootPage_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (Tracker.RootPage != null && _sysBgAcrylicConfigSrc != null)
            {
                switch (Tracker.RootPage.ActualTheme)
                {
                    case ElementTheme.Default:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Default;
                        break;
                    case ElementTheme.Light:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Light;
                        break;
                    case ElementTheme.Dark:
                        _sysBgAcrylicConfigSrc.Theme = SystemBackdropTheme.Dark;
                        break;
                }
            }
        }

        private static void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            DisposeAcrylicControllers();
        }

        private static void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (_sysBgAcrylicConfigSrc != null)
            {
                _sysBgAcrylicConfigSrc.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
        }

        private static void DisposeAcrylicControllers()
        {
            _sysBgAcrylicController?.Dispose();
            _sysBgAcrylicConfigSrc = null;
        }

        public enum SysBackdropType
        {
            None,
            Mica,
            Mica2,
            Acrylic,
            AcrylicThin
        }
    }
}
