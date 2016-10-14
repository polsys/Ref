using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
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
            Title = "Untitled - Ref";

            _viewModel = new MainWindowViewModel();
            _viewModel.PropertyChanged += TitleChangeHandler;
            _viewModel.DisruptingEdit += DisruptingEditHandler;
            _viewModel.DiscardingUnsavedChanges += DiscardingChangesHandler;
            _viewModel.RemovingEntry += RemovingEntryHandler;
            _viewModel.SelectingExportFilename += ExportFileHandler;
            _viewModel.SelectingOpenFilename += OpenFileHandler;
            _viewModel.SelectingSaveFilename += SaveFileHandler;
            DataContext = _viewModel;
            catalogueTreeView.Items.SortDescriptions.Add(new SortDescription("Author", ListSortDirection.Ascending));
            catalogueTreeView.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        private void TitleChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ProjectName) || 
                e.PropertyName == nameof(MainWindowViewModel.IsModified))
            {
                var name = _viewModel.ProjectName;
                if (string.IsNullOrEmpty(name))
                    name = "Untitled";

                if (_viewModel.IsModified)
                    name += "*";

                Title = name + " - Ref";
            }
        }

        private MessageBoxResult DisruptingEditHandler()
        {
            return MessageBox.Show("Do you want to save the edit in progress?", "Ref", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private MessageBoxResult DiscardingChangesHandler()
        {
            return MessageBox.Show("There are unsaved changes. Do you want to save the project?", "Ref", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private MessageBoxResult RemovingEntryHandler()
        {
            return MessageBox.Show("Permanently remove \"" + _viewModel.SelectedEntry.Title + "\"?", "Ref", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private string ExportFileHandler()
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            dialog.ValidateNames = true;

            // Add available exporters as filters
            var filter = "";
            foreach (var exporter in _viewModel.Exporters)
            {
                filter += $"{exporter.Name}|*.{exporter.FileExtension}|";
            }
            dialog.Filter = filter.Substring(0, filter.Length - 1); // Remove trailing |
            dialog.FilterIndex = _viewModel.Exporters.IndexOf(_viewModel.SelectedExporter);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                _viewModel.SelectedExporter = _viewModel.Exporters[dialog.FilterIndex - 1];
                return dialog.FileName;
            }
            else
                return null;
        }

        private string OpenFileHandler()
        {
            var dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.ValidateNames = true;
            dialog.Filter = "Ref Projects|*.refproject";

            var result = dialog.ShowDialog();
            if (result == true)
                return dialog.FileName;
            else
                return null;
        }

        private string SaveFileHandler()
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            dialog.ValidateNames = true;
            dialog.Filter = "Ref Projects|*.refproject";

            var result = dialog.ShowDialog();
            if (result == true)
                return dialog.FileName;
            else
                return null;
        }

        private void catalogueTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_viewModel.SelectEntry((EntryViewModelBase)e.NewValue) != OperationResult.Succeeded)
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

        private void addPageButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddPage();
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

        private void exportProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ExportCatalogue() == OperationResult.Failed)
            {
                MessageBox.Show("Could not export the project.", "Ref", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void newProjectButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NewProject();
        }

        private void openProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OpenProject() == OperationResult.Failed)
            {
                MessageBox.Show("Could not open the project.", "Ref", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void removeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveSelected();
        }

        private void saveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            TrySave(false);
        }

        private void saveProjectAsButton_Click(object sender, RoutedEventArgs e)
        {
            TrySave(true);
        }

        private void TrySave(bool saveAs)
        {
            if (_viewModel.SaveProject(saveAs) == OperationResult.Failed)
            {
                MessageBox.Show("Could not save project.", "Ref", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel.Close() != OperationResult.Succeeded)
                e.Cancel = true;
        }
    }
}
