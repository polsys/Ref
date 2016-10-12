using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// A value converter that always returns <see cref="Visibility.Visible"/>.
    /// This is used in XAML markup to hide unbound elements.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/9893968.
    /// </remarks>
    /// <example>
    /// In XAML: Visibility="{Binding X, Converter=ReferenceToAlwaysVisibleConverter, FallbackValue=Collapsed}"
    /// </example>
    public class AlwaysVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
