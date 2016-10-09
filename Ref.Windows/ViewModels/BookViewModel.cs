using System;
using Ref.Windows.Models;

namespace Ref.Windows.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Book"/>.
    /// </summary>
    internal class BookViewModel : ViewModelBase
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
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, nameof(Title)); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }
        private string _author;
        private string _key;
        private string _publisher;
        private string _title;
        private string _year;

        /// <summary>
        /// Gets whether this entry is read-only or editable.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            private set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
            }
        }
        private bool _isReadOnly;

        /// <summary>
        /// Gets or sets whether this is the currently selected item.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(nameof(IsSelected));
                }
            }
        }
        private bool _isSelected;

        internal Book _book;

        /// <summary>
        /// Constructs a new BookViewModel from the specified <see cref="Book"/>.
        /// </summary>
        /// <param name="book">The Book this ViewModel refers to.</param>
        public BookViewModel(Book book)
        {
            _book = book;
            CopyPropertiesFromBook();
            _isReadOnly = true;
        }

        /// <summary>
        /// Cancels the pending changes and resets the properties to original values.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is true.</exception>
        public void Cancel()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Not in edit mode.");

            CopyPropertiesFromBook();
            IsReadOnly = true;
        }

        /// <summary>
        /// Commits the changed properties to the <see cref="Book"/> model.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is true.</exception>
        public void Commit()
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
        /// Makes this book editable.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is already false.</exception>
        public void Edit()
        {
            if (!IsReadOnly)
                throw new InvalidOperationException("Already in edit mode.");

            IsReadOnly = false;
        }

        private void CopyPropertiesFromBook()
        {
            Author = _book.Author;
            Key = _book.Key;
            Publisher = _book.Publisher;
            Title = _book.Title;
            Year = _book.Year;
        }

        private void SetProperty(ref string property, string value, string propertyName)
        {
            if (property != value)
            {
                if (IsReadOnly)
                    throw new InvalidOperationException("Not in edit mode.");

                property = value;
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
