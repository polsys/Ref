using System.Collections.ObjectModel;
using Ref.Windows.Models;

namespace Ref.Windows.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Catalogue"/>.
    /// </summary>
    internal class CatalogueViewModel
    {
        public ObservableCollection<BookViewModel> Entries { get; }

        /// <summary>
        /// Constructs a new CatalogueViewModel from the specified <see cref="Catalogue"/>.
        /// </summary>
        /// <param name="catalogue">The Catalogue this ViewModel refers to.</param>
        public CatalogueViewModel(Catalogue catalogue)
        {
            Entries = new ObservableCollection<BookViewModel>();
            foreach (var entry in catalogue.Entries)
            {
                Entries.Add(new BookViewModel(entry));
            }
        }
    }
}
