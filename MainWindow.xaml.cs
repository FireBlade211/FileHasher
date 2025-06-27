using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileHasher
{
    /// <summary>
    /// The main application window, containing a <see cref="Frame"/> that hosts the <see cref="RootPage"/>.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Tracker.MainWindow = this;

            MainFrame.Navigate(typeof(RootPage));
        }
    }
}
