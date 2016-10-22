using System;
using System.Collections.ObjectModel;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Book"/>.
    /// </summary>
    internal class BookViewModel : PublicationViewModelBase
    {
        // When adding properties, remember to add them to
        // the reset and commit routines as well!
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value, nameof(Address)); }
        }
        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value, nameof(Author)); }
        }
        public string Edition
        {
            get { return _edition; }
            set { SetProperty(ref _edition, value, nameof(Edition)); }
        }
        public string Editor
        {
            get { return _editor; }
            set { SetProperty(ref _editor, value, nameof(Editor)); }
        }
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value, nameof(Key)); }
        }
        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value, nameof(Number)); }
        }
        public string Publisher
        {
            get { return _publisher; }
            set { SetProperty(ref _publisher, value, nameof(Publisher)); }
        }
        public string Series
        {
            get { return _series; }
            set { SetProperty(ref _series, value, nameof(Series)); }
        }
        public string Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value, nameof(Volume)); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }
        private string _address;
        private string _author;
        private string _edition;
        private string _editor;
        private string _key;
        private string _number;
        private string _publisher;
        private string _series;
        private string _volume;
        private string _year;

        internal Book _book;

        /// <summary>
        /// Constructs a new BookViewModel from the specified <see cref="Book"/>.
        /// </summary>
        /// <param name="book">The Book this ViewModel refers to.</param>
        public BookViewModel(Book book)
        {
            _book = book;

            // Copy the properties and pages
            CopyPropertiesFromModel();
            Pages = new ObservableCollection<PageViewModel>();
            foreach (var page in _book.Pages)
                Pages.Add(new PageViewModel(page, this));
            IsReadOnly = true;
        }

        /// <summary>
        /// Adds the specified page to the book.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pageViewModel"/> is null.</exception>
        public override void AddPage(PageViewModel pageViewModel)
        {
            if (pageViewModel == null)
                throw new ArgumentNullException(nameof(pageViewModel));

            _book.Pages.Add(pageViewModel._page);
            Pages.Add(pageViewModel);
        }

        /// <summary>
        /// Removes the specified page from the book.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to remove.</param>
        public override void RemovePage(PageViewModel pageViewModel)
        {
            Pages.Remove(pageViewModel);
            _book.Pages.Remove(pageViewModel._page);
        }

        protected override void CopyPropertiesFromModel()
        {
            Address = _book.Address;
            Author = _book.Author;
            Edition = _book.Edition;
            Editor = _book.Editor;
            Key = _book.Key;
            Number = _book.Number;
            Publisher = _book.Publisher;
            Series = _book.Series;
            Title = _book.Title;
            Volume = _book.Volume;
            Year = _book.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            _book.Address = Address;
            _book.Author = Author;
            _book.Edition = Edition;
            _book.Editor = Editor;
            _book.Key = Key;
            _book.Number = Number;
            _book.Publisher = Publisher;
            _book.Series = Series;
            _book.Title = Title;
            _book.Volume = Volume;
            _book.Year = Year;
        }
    }
}
