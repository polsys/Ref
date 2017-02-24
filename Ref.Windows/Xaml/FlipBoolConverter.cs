using System;
using System.Globalization;
using System.Windows.Data;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// Put in a bool, get its inverse back.
    /// Put in something else, get an exception.
    /// </summary>
    /// <remarks>
    /// Because IsReadOnly does not prevent combo box selection, which can be prevented with IsEnabled,
    /// which is of course the opposite of IsReadOnly.
    /// </remarks>
    public class FlipBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
