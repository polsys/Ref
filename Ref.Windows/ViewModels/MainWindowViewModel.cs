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

        /// <summary>
        /// Constructs a MainWindowViewModel with an empty catalogue.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize to an empty catalogue
            Catalogue = new CatalogueViewModel(new Catalogue());
        }

        /// <summary>
        /// Selects the specified entry for detailed view.
        /// </summary>
        public void SelectEntry(BookViewModel entry)
        {
            SelectedEntry = entry;
        }
    }
}
