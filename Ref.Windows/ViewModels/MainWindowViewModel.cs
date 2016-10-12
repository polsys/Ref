using System.Windows;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for the main window.
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public CatalogueViewModel Catalogue { get; }
        public EntryViewModelBase SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    NotifyPropertyChanged(nameof(SelectedEntry));
                }
            }
        }
        private EntryViewModelBase _selectedEntry;

        // TODO: Replace with platform-neutral enums
        public delegate MessageBoxResult YesNoCallback();
        public delegate MessageBoxResult YesNoCancelCallback();

        /// <summary>
        /// Raised when another action is initiated while edit is in progress.
        /// The callback should ask the user whether to apply the edit.
        /// </summary>
        public event YesNoCancelCallback DisruptingEdit;

        /// <summary>
        /// Raised before removing an entry.
        /// The callback should confirm that the removal is intentional.
        /// </summary>
        public event YesNoCallback RemovingEntry;

        /// <summary>
        /// Constructs a MainWindowViewModel with an empty catalogue.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize to an empty catalogue
            Catalogue = new CatalogueViewModel(new Catalogue());
        }

        /// <summary>
        /// Adds a new page to the selected entry and selects it for editing.
        /// If a page is currently selected, the page is added to its parent.
        /// </summary>
        public void AddPage()
        {
            if (ShouldCancelBecauseOfEdit())
                return;

            BookViewModel parent = SelectedEntry as BookViewModel;
            if (SelectedEntry is PageViewModel)
                parent = ((PageViewModel)SelectedEntry)._parent;
            SelectedEntry = new PageViewModel(new Page(), parent);
            SelectedEntry.Edit();
        }

        /// <summary>
        /// Cancels the current edit and stops editing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if not editing.</exception>
        public void CancelEdit()
        {
            SelectedEntry.Cancel();

            // If an entry was being added, cancel should result in it getting destroyed
            // This is done by dropping the reference
            if (SelectedEntry is BookViewModel)
            {
                var entryAsBook = SelectedEntry as BookViewModel;
                if (!Catalogue.Entries.Contains(entryAsBook))
                    SelectEntry(null);
            }
            else if (SelectedEntry is PageViewModel)
            {
                var entryAsPage = SelectedEntry as PageViewModel;
                // Select the parent entry
                if (!entryAsPage._parent.Pages.Contains(entryAsPage))
                    SelectEntry(entryAsPage._parent);
            }
        }

        /// <summary>
        /// Commits the current edit and stops editing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if not editing.</exception>
        public void CommitEdit()
        {
            SelectedEntry.Commit();

            // Adding the book
            if (SelectedEntry is BookViewModel)
            {
                var entryAsBook = SelectedEntry as BookViewModel;
                if (!Catalogue.Entries.Contains(entryAsBook))
                {
                    Catalogue.AddBook(entryAsBook);
                    // Change the selection to the new book
                    // WPF makes sure that the last entry gets deselected
                    // TODO: Do not actually depend on that
                    SelectedEntry.IsSelected = true;
                }
            }
            else if (SelectedEntry is PageViewModel)
            {
                var entryAsPage = SelectedEntry as PageViewModel;
                if (!entryAsPage._parent.Pages.Contains(entryAsPage))
                {
                    entryAsPage._parent.AddPage(entryAsPage);
                    // See above comment
                    SelectedEntry.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Adds a new book to the catalogue and selects it.
        /// </summary>
        public void CreateBook()
        {
            if (ShouldCancelBecauseOfEdit())
                return;

            var newBook = new BookViewModel(new Book());
            SelectEntry(newBook);
            newBook.Edit();
        }

        /// <summary>
        /// Starts editing the selected entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if already editing.</exception>
        public void EditSelected()
        {
            SelectedEntry.Edit();
        }

        /// <summary>
        /// Permanently removes the selected entry from the catalogue.
        /// </summary>
        public void RemoveSelected()
        {
            if (ShouldCancelBecauseOfEdit())
                return;

            // This crashes if there is no handler associated
            if (RemovingEntry() == MessageBoxResult.Yes)
            {
                // Null out SelectedEntry first since WPF will react instantly to RemoveBook
                if (SelectedEntry is BookViewModel)
                {
                    var entry = SelectedEntry as BookViewModel;
                    SelectedEntry = null;
                    Catalogue.RemoveBook(entry);
                }
                else if (SelectedEntry is PageViewModel)
                {
                    var entry = SelectedEntry as PageViewModel;
                    SelectedEntry = entry._parent;
                    entry._parent.RemovePage(entry);
                }
            }
        }

        /// <summary>
        /// Tries to select the specified entry for detailed view.
        /// </summary>
        /// <returns>
        /// True if the selection was changed; false if it was not.
        /// </returns>
        public bool SelectEntry(EntryViewModelBase entry)
        {
            // If the selection did not change, return
            if (entry == SelectedEntry)
                return false;

            // If an edit is in progress, ask what to do
            if (ShouldCancelBecauseOfEdit())
            {
                // Roll back the selection
                return false;
            }
            
            SelectedEntry = entry;
            return true;
        }

        private bool ShouldCancelBecauseOfEdit()
        {
            if (SelectedEntry?.IsReadOnly == false)
            {
                // This will crash if the handler is not registered.
                // It is the sanest thing to do.
                var result = DisruptingEdit();
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CommitEdit();
                        return false;
                    case MessageBoxResult.No:
                        CancelEdit();
                        return false;
                    default:
                        return true;
                }
            }
            return false;
        }
    }
}
