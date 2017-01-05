using System;
using System.Collections.ObjectModel;
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

        internal Article _article;

        /// <summary>
        /// Constructs a new Article from the specified <see cref="Article"/>.
        /// </summary>
        /// <param name="article">The Article this ViewModel refers to.</param>
        public ArticleViewModel(Article article)
        {
            _article = article;

            CopyPropertiesFromModel();
            Pages = new ObservableCollection<PageViewModel>();
            foreach (var page in _article.Pages)
                Pages.Add(new PageViewModel(page, this));
            IsReadOnly = true;
        }

        public override void AddPage(PageViewModel pageViewModel)
        {
            if (pageViewModel == null)
                throw new ArgumentNullException(nameof(pageViewModel));

            Pages.Add(pageViewModel);
            _article.Pages.Add(pageViewModel._page);
        }

        public override void RemovePage(PageViewModel pageViewModel)
        {
            Pages.Remove(pageViewModel);
            _article.Pages.Remove(pageViewModel._page);
        }

        protected override void CopyPropertiesFromModel()
        {
            Author = _article.Author;
            Doi = _article.Doi;
            Issn = _article.Issn;
            Journal = _article.Journal;
            Key = _article.Key;
            Notes = _article.Notes;
            Number = _article.Number;
            PageRange = _article.PageRange;
            Title = _article.Title;
            Volume = _article.Volume;
            Year = _article.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            _article.Author = Author;
            _article.Doi = Doi;
            _article.Issn = Issn;
            _article.Journal = Journal;
            _article.Key = Key;
            _article.Notes = Notes;
            _article.Number = Number;
            _article.PageRange = PageRange;
            _article.Title = Title;
            _article.Volume = Volume;
            _article.Year = Year;
        }
    }
}
