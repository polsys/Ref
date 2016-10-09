using System.Windows;
using Ref.Windows.Models;

namespace Ref.Windows.ViewModels
{
    /// <summary>
    /// The View Model for the main window.
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public CatalogueViewModel Catalogue { get; }
        public BookViewModel SelectedEntry
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
        private BookViewModel _selectedEntry;

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
        /// Cancels the current edit and stops editing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if not editing.</exception>
        public void CancelEdit()
        {
            SelectedEntry.Cancel();

            // If the book was being added, cancel should result in the book getting destroyed
            if (!Catalogue.Entries.Contains(SelectedEntry))
                SelectEntry(null);
        }

        /// <summary>
        /// Commits the current edit and stops editing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if not editing.</exception>
        public void CommitEdit()
        {
            SelectedEntry.Commit();

            // Adding the book
            if (!Catalogue.Entries.Contains(SelectedEntry))
            {
                Catalogue.AddBook(SelectedEntry);
                // Change the selection to the new book
                // WPF makes sure that the last entry gets deselected
                // TODO: Do not actually depend on that
                SelectedEntry.IsSelected = true;
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
                var entry = SelectedEntry;
                SelectedEntry = null;
                Catalogue.RemoveBook(entry);
            }
        }

        /// <summary>
        /// Tries to select the specified entry for detailed view.
        /// </summary>
        /// <returns>
        /// True if the selection was changed; false if it was not.
        /// </returns>
        public bool SelectEntry(BookViewModel entry)
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
