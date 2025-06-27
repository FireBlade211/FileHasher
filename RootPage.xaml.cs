using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Linq;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System;

namespace FileHasher
{
    /// <summary>
    /// The primary root page of the application, containing the main navigation bar, the primary content area, and the custom title bar.
    /// </summary>
    public sealed partial class RootPage : Page
    {
        public RootPage()
        {
            InitializeComponent();
            Tracker.RootPage = this;

            MainFrame.Navigate(typeof(Page), "showSelHashAlgMsg");

            Loaded += (s, e) => ApplyTitleBar();

            var t = typeof(ConfigManager);
            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                // Invoke the setter of every property
                if (prop.CanRead && prop.CanWrite)
                    prop.SetValue(null, prop.GetValue(null));
            }

            SearchBox.StartTrackFocusState();
        }

        private void ApplyTitleBar()
        {
            if (Tracker.MainWindow != null)
            {
                Tracker.MainWindow.ExtendsContentIntoTitleBar = true;
                Tracker.MainWindow.Title = Globals.AppTitleName;
                Tracker.MainWindow.SetTitleBar(AppTitleBar);
                Tracker.MainWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                if (Environment.ProcessPath == null) return;

                Tracker.MainWindow.AppWindow.SetIcon(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty, "Assets", "Branding", "Logo_new.ico"));
            }
        }

        private void CtrlF_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (SearchBox.IsFocused() == true)
            {
                Focus(FocusState.Keyboard);
                NavBar.IsPaneOpen = false;
            }
            else
            {
                NavBar.IsPaneOpen = true;
                SearchBox.Focus(FocusState.Keyboard);
            }
        }

        private void FilterSearchBoxItems(string text)
        {
            var navItems = UIHelper.GetNavigationViewItemsRecursive(NavBar);
            var filtered = navItems.Where(x => {
                return x.SelectsOnInvoked;
            });

            var querySplit = text.ToLower().Split(" ");

            var f2 = filtered.Where(x =>
            {
                // Idea: check for every word entered (separated by space) if it is in the name,
                // e.g. for query "md 5" the only result should be "MD5" since its the only query to contain "md" and "5"
                // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words
                var str = x.Content?.ToString()?.ToLower();
                return str != null && querySplit.All(token => str.Contains(token));
            });

            var converted = new List<SearchBoxItem>();

            foreach (var item in f2)
            {
                converted.Add(new SearchBoxItem(item));
            }

            SearchBox.ItemsSource = converted;
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                FilterSearchBoxItems(SearchBox.Text);
            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is SearchBoxItem item)
            {
                NavBar.SelectedItem = item.OnClickItem;
                var parent = UIHelper.GetParentNavigationViewItem(NavBar, item.OnClickItem);
                if (parent != null)
                {
                    parent.IsExpanded = true;
                }
                item.OnClickItem.StartBringIntoView();
            }
            else
            {
                FilterSearchBoxItems(args.QueryText);

                var src = SearchBox.ItemsSource;
                if (src is List<SearchBoxItem> list)
                {
                    var i = list.FirstOrDefault();
                    if (i != null)
                    {
                        NavBar.SelectedItem = i.OnClickItem;
                        var parent = UIHelper.GetParentNavigationViewItem(NavBar, i.OnClickItem);
                        if (parent != null)
                        {
                            parent.IsExpanded = true;
                        }
                        i.OnClickItem.StartBringIntoView();
                    }
                }
            }
        }

        private void NavBar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                MainFrame.Navigate(typeof(ConfigPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                if (NavBar.SelectedItem is NavigationViewItem item)
                {
                    MainFrame.Navigate(typeof(HashPage), HashHelper.GetHashAlgorithmForString(item.Content.ToString() ?? string.Empty), args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        private void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                VisualStateManager.GoToState(this, "Top", true);
            }
            else
            {
                if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    VisualStateManager.GoToState(this, "Compact", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Default", true);
                }
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is HashPage page)
            {
                if (NavBar.SelectedItem is NavigationViewItem navItem)
                {
                    page.TitleBlock.Text = navItem.Content.ToString() + " Hash";
                    page.HashResultBox.PlaceholderText = navItem.Content.ToString() + " Hash String...";
                }
            }
            else if (e.SourcePageType == typeof(Page) && e.Parameter?.ToString() == "showSelHashAlgMsg")
            {
                if (e.Content is Page p)
                {
                    p.Content = new TextBlock
                    {
                        Text = "Select a hash algorithm from the navigation bar to begin",
                        Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = (Brush)Application.Current.Resources["TextFillColorTertiaryBrush"]
                    };
                }
            }

            switch (e.Content)
            {
                case HashPage:
                    // HashPage requires additional logic to compute the proper item
                    if (e.Parameter == null) return;
                    var str = HashHelper.GetStringForHashAlgorithm((HashHelper.HashAlgorithmType)e.Parameter);
                    if (str != null)
                    {
                        var items = UIHelper.GetNavigationViewItemsRecursive(NavBar);
                        var item = items.Find(x =>
                        {
                            return x.Content.ToString() == str;
                        });

                        if (item != null)
                        {
                            NavBar.SelectedItem = item;
                        }
                    }
                    break;
                case ConfigPage:
                    NavBar.SelectedItem = NavBar.SettingsItem;
                    break;
                case Page:
                    if (e.Parameter?.ToString() == "showSelHashAlgMsg")
                    {
                        NavBar.SelectedItem = null;
                    }
                    break;
            }
        }

        private void MainFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var text = "A navigation error occured.";
#if DEBUG
            text += "\n- Debug Info -" +
                $"\nMessage: {e.Exception.Message}" +
                $"\nType: {e.Exception.GetType().FullName ?? e.Exception.GetType().Name}" +
                $"\nPage type: {e.SourcePageType.FullName ?? e.SourcePageType.Name}" +
                $"\nHRESULT: {e.Exception.HResult}" +
                $"\nStack trace: {e.Exception.StackTrace}";
#endif

            e.Handled = true;

            var dlg = new ContentDialog
            {
                Content = new TextBlock { Text = text },
                Title = "Navigation Error",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            _ = dlg.ShowAsync();
        }

        private void NavBar_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            MainFrame.GoBack();
        }
    }

    public class SearchBoxItem(NavigationViewItem item)
    {
        public NavigationViewItem OnClickItem = item;

        public override string ToString()
        {
            return OnClickItem.Content.ToString() ?? string.Empty;
        }
    }
}
