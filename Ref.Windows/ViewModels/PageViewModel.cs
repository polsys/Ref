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
        public string PageRange
        {
            get { return _pageRange; }
            set { SetProperty(ref _pageRange, value, nameof(PageRange)); }
        }
        private string _notes;
        private string _pageRange;

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
            _page.PageRange = PageRange;
            _page.Title = Title;
            IsReadOnly = true;
        }

        private void CopyPropertiesFromModel()
        {
            Notes = _page.Notes;
            PageRange = _page.PageRange;
            Title = _page.Title;
        }
    }
}
