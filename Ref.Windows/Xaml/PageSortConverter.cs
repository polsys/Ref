using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Polsys.Ref.Xaml
{
    /// <summary>
    /// A value converter that returns a sorted <see cref="ListCollectionView"/> based on the items.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/5730402.
    /// </remarks>
    /// <example>
    /// In XAML: ItemsSource="{Binding X, Converter=ReferenceToPageSortConverter}"
    /// </example>
    public class PageSortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IList;
            if (collection == null)
                return null;

            // Sort the collection first by page and then by title
            var view = new ListCollectionView(collection);
            view.SortDescriptions.Add(new SortDescription("FirstPage", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
