using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.IO;

namespace FileHasher
{
    /// <summary>
    /// A page that allows generating hashes with a specific algorithm.
    /// </summary>
    public sealed partial class HashPage : Page
    {
        private HashHelper.HashAlgorithmType _type;

        public HashPage()
        {
            InitializeComponent();

            HashResultBox.Text = HashHelper.ComputeHash(_type, SourceBox.Text);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is HashHelper.HashAlgorithmType type)
            {
                _type = type;

                LiveUpdateToggleButton.IsChecked = ConfigManager.HashLiveUpdate;

                if (ConfigManager.HashLiveUpdate)
                {
                    SourceGrid.Children.Remove(GenHashButton);
                }
                else
                {
                    SourceGrid.Children.Add(GenHashButton);
                    Grid.SetColumn(GenHashButton, 2);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(e.Parameter), "Error: Failed to initialize HashPage: Frame navigation parameter is null.");
            }
        }

        private void LiveUpdateToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.HashLiveUpdate = LiveUpdateToggleButton.IsChecked ?? false;

            if (LiveUpdateToggleButton.IsChecked == true)
            {
                SourceGrid.Children.Remove(GenHashButton);
                HashResultBox.Text = HashHelper.ComputeHash(_type, SourceBox.Text);
            }
            else
            {
                SourceGrid.Children.Add(GenHashButton);
                Grid.SetColumn(GenHashButton, 2);
            }
        }

        private void SourceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LiveUpdateToggleButton.IsChecked == true)
            {
                HashResultBox.Text = HashHelper.ComputeHash(_type, SourceBox.Text);
            }
        }

        private void GenHashButton_Click(object sender, RoutedEventArgs e)
        {
            HashResultBox.Text = HashHelper.ComputeHash(_type, SourceBox.Text);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(HashResultBox.Text);
            Clipboard.SetContent(package);
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = false;

            // Create a file picker
            var openPicker = new FileOpenPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = Tracker.MainWindow;

            if (window == null)
            {
                LoadButton.IsEnabled = true;
                return;
            }

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                SourceBox.Text = File.ReadAllText(file.Path);
            }

            LoadButton.IsEnabled = true;

        }
    }
}
