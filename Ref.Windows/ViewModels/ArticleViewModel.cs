using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Article"/>.
    /// </summary>
    internal class ArticleViewModel : PublicationViewModelBase
    {
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
        public string Issn
        {
            get { return _issn; }
            set { SetProperty(ref _issn, value, nameof(Issn)); }
        }
        public string Journal
        {
            get { return _journal; }
            set { SetProperty(ref _journal, value, nameof(Journal)); }
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
        public string PageRange
        {
            get { return _pageRange; }
            set { SetProperty(ref _pageRange, value, nameof(PageRange)); }
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
        private string _author;
        private string _doi;
        private string _issn;
        private string _journal;
        private string _key;
        private string _number;
        private string _pageRange;
        private string _volume;
        private string _year;

        /// <summary>
        /// Constructs a new Article from the specified <see cref="Article"/>.
        /// </summary>
        /// <param name="article">The Article this ViewModel refers to.</param>
        public ArticleViewModel(Article article) :
            base(article)
        {
        }

        protected override void CopyPropertiesFromModel()
        {
            var article = (Article)_model;

            Author = article.Author;
            Doi = article.Doi;
            Issn = article.Issn;
            Journal = article.Journal;
            Key = article.Key;
            Notes = article.Notes;
            Number = article.Number;
            PageRange = article.PageRange;
            Title = article.Title;
            Volume = article.Volume;
            Year = article.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            var article = (Article)_model;

            article.Author = Author;
            article.Doi = Doi;
            article.Issn = Issn;
            article.Journal = Journal;
            article.Key = Key;
            article.Notes = Notes;
            article.Number = Number;
            article.PageRange = PageRange;
            article.Title = Title;
            article.Volume = Volume;
            article.Year = Year;
        }
    }
}
