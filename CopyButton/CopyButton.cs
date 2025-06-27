using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileHasher
{
    public sealed class CopyButton : Button
    {
        public CopyButton()
        {
            DefaultStyleKey = typeof(CopyButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var storyboard = GetTemplateChild("CopyToClipboardSuccessAnimation") as Storyboard;
            Click += (_, _) => storyboard?.Begin();
        }
    }
}
