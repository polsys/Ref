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

        // TODO: Replace with platform-neutral enum
        public delegate MessageBoxResult YesNoCancelCallback();

        /// <summary>
        /// Raised when another action is initiated while edit is in progress.
        /// </summary>
        public event YesNoCancelCallback DisruptingEdit;

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
        /// Selects the specified entry for detailed view.
        /// </summary>
        public void SelectEntry(BookViewModel entry)
        {
            // If an edit is in progress, ask what to do
            if (ShouldCancelBecauseOfEdit())
                return;
            
            SelectedEntry = entry;
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
