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
        public string Isbn
        {
            get { return _isbn; }
            set { SetProperty(ref _isbn, value, nameof(Isbn)); }
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
        public string Translator
        {
            get { return _translator; }
            set { SetProperty(ref _translator, value, nameof(Translator)); }
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
        private string _isbn;
        private string _key;
        private string _number;
        private string _publisher;
        private string _series;
        private string _translator;
        private string _volume;
        private string _year;

        /// <summary>
        /// Constructs a new BookViewModel from the specified <see cref="Book"/>.
        /// </summary>
        /// <param name="book">The Book this ViewModel refers to.</param>
        public BookViewModel(Book book):
            base(book)
        {
        }

        protected override void CopyPropertiesFromModel()
        {
            var book = (Book)_model;

            Address = book.Address;
            Author = book.Author;
            Edition = book.Edition;
            Editor = book.Editor;
            Isbn = book.Isbn;
            Key = book.Key;
            Notes = book.Notes;
            Number = book.Number;
            Publisher = book.Publisher;
            Series = book.Series;
            Title = book.Title;
            Translator = book.Translator;
            Volume = book.Volume;
            Year = book.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            var book = (Book)_model;

            book.Address = Address;
            book.Author = Author;
            book.Edition = Edition;
            book.Editor = Editor;
            book.Isbn = Isbn;
            book.Key = Key;
            book.Notes = Notes;
            book.Number = Number;
            book.Publisher = Publisher;
            book.Series = Series;
            book.Title = Title;
            book.Translator = Translator;
            book.Volume = Volume;
            book.Year = Year;
        }
    }
}
