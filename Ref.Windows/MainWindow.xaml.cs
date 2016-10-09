using System;
using System.Windows;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            _viewModel.DisruptingEdit += DisruptingEditHandler;
            _viewModel.RemovingEntry += RemovingEntryHandler;
            DataContext = _viewModel;
        }

        private MessageBoxResult DisruptingEditHandler()
        {
            return MessageBox.Show("Do you want to save the edit in progress?", "Ref", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private MessageBoxResult RemovingEntryHandler()
        {
            return MessageBox.Show("Permanently remove \"" + _viewModel.SelectedEntry.Title + "\"?", "Ref", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private void catalogueTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_viewModel.SelectEntry((BookViewModel)e.NewValue))
            {
                // SelectEntry returned false so we must roll back
                // Apparently this should be done in the LayoutUpdated event (http://stackoverflow.com/a/4029075)
                EventHandler handler = null; // null because this must be initialized for the lambda
                handler = (object s, EventArgs args) =>
                {
                    if (_viewModel.SelectedEntry != null)
                        _viewModel.SelectedEntry.IsSelected = true;
                    catalogueTreeView.LayoutUpdated -= handler;
                };
                catalogueTreeView.LayoutUpdated += handler;
            }
        }

        private void addEntryButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateBook();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CancelEdit();
        }

        private void commitButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CommitEdit();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.EditSelected();
        }

        private void removeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveSelected();
        }
    }
}
