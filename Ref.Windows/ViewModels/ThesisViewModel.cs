using System;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Thesis"/>.
    /// </summary>
    internal class ThesisViewModel : PublicationViewModelBase
    {
        // When adding properties, remember to add them to
        // the reset and commit routines as well!
        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value, nameof(Author)); }
        }
        public string Doi
        {
            get { return _doi; }
            set { SetProperty(ref _doi, value, nameof(Doi)); }
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
        public ThesisKind Kind
        {
            get { return _kind; }
            set { SetProperty(ref _kind, value, nameof(Kind)); }
        }
        public string School
        {
            get { return _school; }
            set { SetProperty(ref _school, value, nameof(School)); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }
        private string _author;
        private string _doi;
        private string _isbn;
        private string _key;
        private ThesisKind _kind;
        private string _school;
        private string _year;

        internal Thesis _thesis
        {
            get { return (Thesis)_model; }
        }

        /// <summary>
        /// Constructs a new ThesisViewModel from the specified <see cref="Thesis"/>.
        /// </summary>
        /// <param name="thesis">The Thesis this ViewModel refers to.</param>
        public ThesisViewModel(Thesis thesis):
            base(thesis)
        {
        }

        protected override void CopyPropertiesFromModel()
        {
            Author = _thesis.Author;
            Doi = _thesis.Doi;
            Isbn = _thesis.Isbn;
            Key = _thesis.Key;
            Kind = _thesis.Kind;
            Notes = _thesis.Notes;
            School = _thesis.School;
            Title = _thesis.Title;
            Year = _thesis.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            _thesis.Author = Author;
            _thesis.Doi = Doi;
            _thesis.Isbn = Isbn;
            _thesis.Key = Key;
            _thesis.Kind = Kind;
            _thesis.Notes = Notes;
            _thesis.School = School;
            _thesis.Title = Title;
            _thesis.Year = Year;
        }
    }
}
