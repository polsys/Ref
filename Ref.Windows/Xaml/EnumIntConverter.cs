using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// Converts enums into integers and back so that they can be used as SelectedIndex in XAML.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/33611155.
    /// </remarks>
    public class EnumIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.IsEnum)
            {
                return Enum.ToObject(targetType, value);
            }
            else if (value.GetType().IsEnum)
            {
                return System.Convert.ToInt32(value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
