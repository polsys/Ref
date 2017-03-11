using System;
using System.Globalization;
using System.Windows.Data;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// A value converter that returns either the title, augmented with possible volume information,
    /// or an "Untitled" value.
    /// </summary>
    public class EntryTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 1 && values[0] is string)
            {
                var title = (string)values[0];

                // If the entry is a book with a specified volume
                if (values.Length >= 2 && values[1] is string)
                {
                    var volume = (string)values[1];
                    if (!string.IsNullOrWhiteSpace(volume))
                    {
                        return title + " (" + volume + ")";
                    }
                }

                // Else just return the entry title, unless it is empty/null
                return string.IsNullOrWhiteSpace(title) ? "(Untitled)" : title;
            }

            // If the value is not an entry, signal an error
            return "???";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
