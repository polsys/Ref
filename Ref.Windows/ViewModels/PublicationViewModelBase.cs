using System;
using System.Collections.ObjectModel;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The base class for publications, for example books and articles.
    /// </summary>
    internal abstract class PublicationViewModelBase : EntryViewModelBase
    {
        public ObservableCollection<PageViewModel> Pages { get; protected set; }

        /// <summary>
        /// Adds the specified page to this entry.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pageViewModel"/> is null.</exception>
        public abstract void AddPage(PageViewModel pageViewModel);

        public override void Cancel()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Not in edit mode.");

            CopyPropertiesFromModel();
            IsReadOnly = true;
        }

        public override void Commit()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Not in edit mode.");

            CopyPropertiesToModel();
            IsReadOnly = true;
        }

        /// <summary>
        /// Copies all the properties from model as part of initialization or <see cref="Cancel"/>.
        /// </summary>
        protected abstract void CopyPropertiesFromModel();

        /// <summary>
        /// Copies all the properties back to model as part of <see cref="Commit"/>.
        /// </summary>
        protected abstract void CopyPropertiesToModel();

        /// <summary>
        /// Removes the specified page from this entry.
        /// </summary>
        /// <param name="pageViewModel">The view model of the page to remove.</param>
        public abstract void RemovePage(PageViewModel pageViewModel);
    }
}
