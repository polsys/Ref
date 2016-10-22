using System;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Page"/>.
    /// </summary>
    internal class PageViewModel : EntryViewModelBase
    {
        /// <summary>
        /// Gets the first page in the page range, or <see cref="int.MaxValue"/> if unspecified.
        /// </summary>
        /// <remarks>
        /// This property exists since WPF sort implementation does not understand numbers.
        /// </remarks>
        public int FirstPage
        {
            get
            {
                if (string.IsNullOrEmpty(PageRange))
                    return int.MaxValue;

                // Read the string until a non-digit is found
                int i = 0;
                for (; i < PageRange.Length; i++)
                {
                    if (!char.IsDigit(PageRange[i]))
                        break;
                }

                // No digits or too many of them for a 32-bit integer
                if (i == 0 || i > 9)
                    return int.MaxValue;
                else
                    return Convert.ToInt32(PageRange.Substring(0, i));
            }
        }
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
        internal PublicationViewModelBase _parent;

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

        internal PageViewModel(Page page, PublicationViewModelBase parent)
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
