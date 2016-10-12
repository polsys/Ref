using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Page"/>.
    /// </summary>
    internal class PageViewModel : EntryViewModelBase
    {
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value, nameof(Notes)); }
        }
        public string Pages
        {
            get { return _pages; }
            set { SetProperty(ref _pages, value, nameof(Pages)); }
        }
        private string _notes;
        private string _pages;

        internal Page _page;
        internal BookViewModel _parent;

        /// <summary>
        /// Constructs a new PageViewModel from the specified <see cref="Page"/>.
        /// </summary>
        /// <param name="page">The Page this view model refers to.</param>
        public PageViewModel(Page page)
        {
            _page = page;
            CopyPropertiesFromModel();
            IsReadOnly = true;
        }

        internal PageViewModel(Page page, BookViewModel parent)
            : this(page)
        {
            _parent = parent;
        }

        public override void Cancel()
        {
            CopyPropertiesFromModel();
            IsReadOnly = true;
        }

        public override void Commit()
        {
            _page.Notes = Notes;
            _page.Pages = Pages;
            _page.Title = Title;
            IsReadOnly = true;
        }

        private void CopyPropertiesFromModel()
        {
            Notes = _page.Notes;
            Pages = _page.Pages;
            Title = _page.Title;
        }
    }
}
