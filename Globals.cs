using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHasher
{
    static class Globals
    {
        public static string AppTitleName
        {
            get
            {
#if DEBUG
                return "FileHasher (Debug)";
#else
                return "FileHasher";
#endif
            }
        }

        public static string AppVersion
        {
            get
            {
                var appendString = string.Empty;
#if DEBUG
                appendString = "-Debug";
#endif
                return "v1.0" + appendString;
            }
        }

        public static Version? WinAppSdkVersion
        {
            get
            {
                return typeof(Microsoft.UI.Xaml.Application)
    .Assembly
    .GetName()
    .Version;
            }
        }

        public static string WinAppSdkRuntimeDetails
        {
            get
            {
                try
                {
                    return $"Windows App SDK {WinAppSdkVersion?.Major}.{WinAppSdkVersion?.Minor}, Windows App Runtime " + RuntimeInfo.AsString;
                }
                catch
                {
                    return $"Windows App SDK {WinAppSdkVersion?.Major}.{WinAppSdkVersion?.Minor}";
                }
            }
        }
    }
}
