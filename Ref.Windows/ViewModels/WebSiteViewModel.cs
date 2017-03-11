using System;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="WebSite"/>.
    /// </summary>
    internal class WebSiteViewModel : PublicationViewModelBase
    {
        // When adding properties, remember to add them to
        // the reset and commit routines as well!
        public DateTime AccessDate
        {
            get { return _accessDate; }
            set { SetProperty(ref _accessDate, value, nameof(AccessDate)); }
        }
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
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value, nameof(Url)); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value, nameof(Year)); }
        }
        private DateTime _accessDate;
        private string _author;
        private string _key;
        private string _url;
        private string _year;

        /// <summary>
        /// Constructs a new WebSiteViewModel from the specified <see cref="WebSite"/>.
        /// </summary>
        /// <param name="webSite">The web site this ViewModel refers to.</param>
        public WebSiteViewModel(WebSite webSite):
            base(webSite)
        {
        }

        protected override void CopyPropertiesFromModel()
        {
            var site = (WebSite)_model;

            AccessDate = site.AccessDate;
            Author = site.Author;
            Key = site.Key;
            Notes = site.Notes;
            Title = site.Title;
            Url = site.Url;
            Year = site.Year;
        }

        protected override void CopyPropertiesToModel()
        {
            var site = (WebSite)_model;

            site.AccessDate = AccessDate;
            site.Author = Author;
            site.Key = Key;
            site.Notes = Notes;
            site.Title = Title;
            site.Url = Url;
            site.Year = Year;
        }
    }
}
