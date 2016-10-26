using System;
using System.Collections.ObjectModel;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Catalogue"/>.
    /// </summary>
    internal class CatalogueViewModel
    {
        public ObservableCollection<PublicationViewModelBase> Entries { get; }

        internal Catalogue _catalogue;

        /// <summary>
        /// Constructs a new CatalogueViewModel from the specified <see cref="Catalogue"/>.
        /// </summary>
        /// <param name="catalogue">The Catalogue this ViewModel refers to.</param>
        public CatalogueViewModel(Catalogue catalogue)
        {
            _catalogue = catalogue;

            Entries = new ObservableCollection<PublicationViewModelBase>();
            foreach (var entry in catalogue.Entries)
            {
                if (entry is Article)
                    Entries.Add(new ArticleViewModel((Article)entry));
                else if (entry is Book)
                    Entries.Add(new BookViewModel((Book)entry));
                else
                    throw new NotImplementedException("The specified entry type is not implemented.");
            }
        }

        /// <summary>
        /// Adds the specified book to the catalogue.
        /// </summary>
        public void AddEntry(PublicationViewModelBase entry)
        {
            if (entry is ArticleViewModel)
                _catalogue.Entries.Add(((ArticleViewModel)entry)._article);
            else if (entry is BookViewModel)
                _catalogue.Entries.Add(((BookViewModel)entry)._book);
            else
                throw new NotImplementedException("The specified entry type is not implemented.");
            Entries.Add(entry);
        }

        /// <summary>
        /// Removes the specified book from the catalogue.
        /// </summary>
        /// <param name="book">The book to remove.</param>
        public void RemoveEntry(PublicationViewModelBase entry)
        {
            if (entry is ArticleViewModel)
                _catalogue.Entries.Remove(((ArticleViewModel)entry)._article);
            else if (entry is BookViewModel)
                _catalogue.Entries.Remove(((BookViewModel)entry)._book);
            else
                throw new NotImplementedException("The specified entry type is not implemented.");
            Entries.Remove(entry);
        }
    }
}
