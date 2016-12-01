using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// A value converter that returns either the title, augmented with possible volume information,
    /// or an "Untitled" value.
    /// </summary>
    public class EntryTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EntryViewModelBase)
            {
                var entry = value as EntryViewModelBase;

                // If the entry is a book with a specified volume
                if (entry is BookViewModel)
                {
                    var book = entry as BookViewModel;
                    if (!string.IsNullOrWhiteSpace(book.Volume))
                    {
                        return book.Title + " (" + book.Volume + ")";
                    }
                }

                // Else just return the entry title
                return entry.Title;
            }

            // If the value is not an entry, signal an error
            return "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
