using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

#pragma warning disable CS8602
#pragma warning disable CS8604
namespace FileHasher
{
    /// <summary>
    /// A page for configuring application settings.
    /// </summary>
    public sealed partial class ConfigPage : Page
    {
        public ConfigPage()
        {
            InitializeComponent();
#if DEBUG
            var card = new SettingsCard();
            card.Header = "You are running a debug build of FileHasher";
            card.Description = "This message will automatically be hidden in release builds.";
            card.HeaderIcon = new FontIcon
            {
                Glyph = "\uEBE8"
            };
            AboutExpander.Items.Add(card);
#endif

            if ((!MicaController.IsSupported()) && (!DesktopAcrylicController.IsSupported()))
            {
                winBgCard.IsEnabled = false;
                winBg.IsEnabled = false;
                winBgCard.Header += " (unsupported)";
                winBgCard.Description = "Transparency effects are not supported on your device.";
            }
            else
            {
                if (!MicaController.IsSupported())
                {
                    winBg.Items.Remove(micaItem);
                    winBg.Items.Remove(mica2Item);
                }

                if (!DesktopAcrylicController.IsSupported())
                {
                    winBg.Items.Remove(acrlItem);
                    winBg.Items.Remove(acrlThItem);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var i = 0;
            foreach (ComboBoxItem item in themeMode.Items)
            {
                if ((string)item.Tag == Enum.GetName(typeof(ElementTheme), ConfigManager.Theme))
                {
                    themeMode.SelectedIndex = i;
                    break;
                }
                i++;
            }
            i = 0;
            foreach (ComboBoxItem item in navigationLocation.Items)
            {
                if ((string)item.Tag == Enum.GetName(typeof(NavigationViewPaneDisplayMode), ConfigManager.NavBarPaneNavigMode))
                {
                    navigationLocation.SelectedIndex = i;
                    break;
                }
                i++;
            }
            soundToggle.IsOn = ConfigManager.SoundEnabled;
            spatialSoundBox.IsOn = ConfigManager.SpatialSoundEnabled;
            SoundVolumeSlider.Value = ConfigManager.SoundVolume * 100;
            i = 0;
            foreach (ComboBoxItem item in winBg.Items)
            {
                if ((string)item.Tag == Enum.GetName(typeof(ConfigManager.SysBackdropType), ConfigManager.WindowBackdrop))
                {
                    winBg.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }

        private void themeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigManager.Theme = (ElementTheme)Enum.Parse(typeof(ElementTheme), (e.AddedItems[0] as ComboBoxItem).Tag as string);
        }

        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mode = (NavigationViewPaneDisplayMode)Enum.Parse(typeof(NavigationViewPaneDisplayMode), (e.AddedItems[0] as ComboBoxItem).Tag as string);
            ConfigManager.NavBarPaneNavigMode = mode;
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ConfigManager.SoundEnabled = soundToggle.IsOn;
            SpatialAudioCard.IsEnabled = soundToggle.IsOn;
            spatialSoundBox.IsEnabled = soundToggle.IsOn;
            SoundVolumeCard.IsEnabled = soundToggle.IsOn;
            SoundVolumeSlider.IsEnabled = soundToggle.IsOn;
        }

        private void spatialSoundBox_Toggled(object sender, RoutedEventArgs e)
        {
            ConfigManager.SpatialSoundEnabled = spatialSoundBox.IsOn;
        }

        private void bugRequestCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void githubCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SoundVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ConfigManager.SoundVolume = e.NewValue / 100;
        }

        private void AudioTestCard_Click(object sender, RoutedEventArgs e)
        {
            var items = Enum.GetValues(typeof(ElementSoundKind)).Cast<ElementSoundKind>().ToList();
            var idx = Random.Shared.Next(items.Count);
            ElementSoundPlayer.Play(items[idx]);
        }

        private void winBg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (winBg.SelectedItem as ComboBoxItem).Tag as string;

            ConfigManager.WindowBackdrop = Enum.Parse<ConfigManager.SysBackdropType>(t);
        }

        private void AboutExpander_Expanded(object sender, EventArgs e)
        {
            WinAppSdkLink.Content = Globals.WinAppSdkRuntimeDetails;
        }
    }
}
#pragma warning restore