using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ACAT.Core.Utility
{
    public static class WpfInitializationHelper
    {
        public static bool _initialized = false;

        public static void EnsureApplicationResources()
        {
            if (_initialized)
            {
                return;
            }

            if (Application.Current == null)
            {
                new Application();
            }

            Collection<ResourceDictionary> resources = Application.Current.Resources.MergedDictionaries;

            bool hasMahApps = resources.OfType<ResourceDictionary>().Any(rd =>
                rd.Source?.ToString()?.Contains("MahApps.Metro") == true);

            if (!hasMahApps)
            {
                resources.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.Absolute)
                });
                resources.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.Absolute)
                });
                resources.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Amber.xaml", UriKind.Absolute)
                });
            }

            _initialized = true;
        }
    }
}
