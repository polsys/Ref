using System.Collections.ObjectModel;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Catalogue"/>.
    /// </summary>
    internal class CatalogueViewModel
    {
        public ObservableCollection<BookViewModel> Entries { get; }

        private Catalogue _catalogue;

        /// <summary>
        /// Constructs a new CatalogueViewModel from the specified <see cref="Catalogue"/>.
        /// </summary>
        /// <param name="catalogue">The Catalogue this ViewModel refers to.</param>
        public CatalogueViewModel(Catalogue catalogue)
        {
            _catalogue = catalogue;

            Entries = new ObservableCollection<BookViewModel>();
            foreach (var entry in catalogue.Entries)
            {
                Entries.Add(new BookViewModel(entry));
            }
        }

        /// <summary>
        /// Adds the specified book to the catalogue.
        /// </summary>
        public void AddBook(BookViewModel book)
        {
            _catalogue.Entries.Add(book._book);
            Entries.Add(book);
        }

        /// <summary>
        /// Removes the specified book from the catalogue.
        /// </summary>
        /// <param name="book">The book to remove.</param>
        public void RemoveBook(BookViewModel book)
        {
            _catalogue.Entries.Remove(book._book);
            Entries.Remove(book);
        }
    }
}
