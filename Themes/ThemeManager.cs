using System;
using System.Windows;

namespace CarDealership.Themes
{
    public static class ThemeManager
    {
        private static bool _isDark = true;
        public static bool IsDark => _isDark;

        public static void Toggle()
        {
            _isDark = !_isDark;
            Apply();
        }

        public static void Apply()
        {
            var appDict = Application.Current.Resources.MergedDictionaries;

            // Find and remove the current theme dictionary
            ResourceDictionary? existing = null;
            foreach (var d in appDict)
            {
                var src = d.Source?.OriginalString ?? "";
                if (src.Contains("Theme.xaml"))
                {
                    existing = d;
                    break;
                }
            }

            // Load new theme
            var uri = new Uri(_isDark
                ? "Themes/DarkTheme.xaml"
                : "Themes/LightTheme.xaml", UriKind.Relative);

            var newTheme = new ResourceDictionary { Source = uri };

            // Swap atomically: add new first, then remove old
            appDict.Insert(0, newTheme);

            if (existing != null)
                appDict.Remove(existing);
        }
    }
}
