using System;
using System.Collections.ObjectModel;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Book"/>.
    /// </summary>
    internal class BookViewModel : EntryViewModelBase
    {
        // When adding properties, remember to add them to
        // the reset and commit routines as well!
        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value, nameof(Author)); }
        }
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value, nameof(Key)); }
        }
        public string Publisher
        {
            get { return _publisher; }
            set { SetProperty(ref _publisher, value, nameof(Publisher)); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }
        private string _author;
        private string _key;
        private string _publisher;
        private string _year;

        public ObservableCollection<PageViewModel> Pages { get; private set; }

        internal Book _book;

        /// <summary>
        /// Constructs a new BookViewModel from the specified <see cref="Book"/>.
        /// </summary>
        /// <param name="book">The Book this ViewModel refers to.</param>
        public BookViewModel(Book book)
        {
            _book = book;

            // Copy the properties and pages
            CopyPropertiesFromBook();
            Pages = new ObservableCollection<PageViewModel>();
            foreach (var page in _book.Pages)
                Pages.Add(new PageViewModel(page));
            IsReadOnly = true;
        }

        /// <summary>
        /// Adds the specified page to the book.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pageViewModel"/> is null.</exception>
        public void AddPage(PageViewModel pageViewModel)
        {
            if (pageViewModel == null)
                throw new ArgumentNullException(nameof(pageViewModel));

            _book.Pages.Add(pageViewModel._page);
            Pages.Add(pageViewModel);
        }
        
        public override void Cancel()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Not in edit mode.");

            CopyPropertiesFromBook();
            IsReadOnly = true;
        }

        public override void Commit()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Not in edit mode.");

            _book.Author = Author;
            _book.Key = Key;
            _book.Publisher = Publisher;
            _book.Title = Title;
            _book.Year = Year;

            IsReadOnly = true;
        }

        /// <summary>
        /// Removes the specified page from the book.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to remove.</param>
        public void RemovePage(PageViewModel pageViewModel)
        {
            Pages.Remove(pageViewModel);
            _book.Pages.Remove(pageViewModel._page);
        }

        private void CopyPropertiesFromBook()
        {
            Author = _book.Author;
            Key = _book.Key;
            Publisher = _book.Publisher;
            Title = _book.Title;
            Year = _book.Year;
        }
    }
}
