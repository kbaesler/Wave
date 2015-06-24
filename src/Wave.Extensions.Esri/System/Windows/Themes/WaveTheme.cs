using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace System.Windows
{
    /// <summary>
    /// The default WPF theme that should be used by all customizations in ArcMap customizations.
    /// </summary>
    public class WaveTheme : ResourceDictionary
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveTheme"/> class.
        /// </summary>
        public WaveTheme()
        {
            Uri uri = new Uri("/Wave.Extensions.Esri;component/System/Windows/Themes/Wave.xaml", UriKind.Relative);
            var dict = new ResourceDictionary { Source = uri };
            dict.BeginInit();
            MergedDictionaries.Add(dict);
        }

        #endregion
    }
}
