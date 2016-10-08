using System.ComponentModel;
using Ref.Windows.Models;

namespace Ref.Windows.ViewModels
{
    /// <summary>
    /// The View Model for the main window.
    /// </summary>
    internal class MainWindowViewModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
