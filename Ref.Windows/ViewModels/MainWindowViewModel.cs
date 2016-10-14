using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Polsys.Ref.Export;
using Polsys.Ref.Models;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The View Model for the main window.
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public CatalogueViewModel Catalogue
        {
            get { return _catalogue; }
            private set
            {
                if (_catalogue != value)
                {
                    _catalogue = value;
                    NotifyPropertyChanged(nameof(Catalogue));
                }
            }
        }
        private CatalogueViewModel _catalogue;

        public List<CatalogueExporter> Exporters
        {
            get;
            private set;
        }

        public string Filename
        {
            get { return _filename; }
            private set
            {
                if (_filename != value)
                {
                    _filename = value;
                    NotifyPropertyChanged(nameof(Filename));
                }
            }
        }
        private string _filename;

        public bool IsModified
        {
            get { return _isModified; }
            private set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    NotifyPropertyChanged(nameof(IsModified));
                }
            }
        }
        private bool _isModified;

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    NotifyPropertyChanged(nameof(ProjectName));
                }
            }
        }
        private string _projectName;

        public EntryViewModelBase SelectedEntry
        {
            get { return _selectedEntry; }
            private set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    NotifyPropertyChanged(nameof(SelectedEntry));
                }
            }
        }
        private EntryViewModelBase _selectedEntry;

        public CatalogueExporter SelectedExporter
        {
            get;
            set;
        }
        
        public delegate string StringCallback();
        // TODO: Replace with platform-neutral enums
        public delegate MessageBoxResult YesNoCallback();
        public delegate MessageBoxResult YesNoCancelCallback();

        /// <summary>
        /// Raised when another action is initiated while edit is in progress.
        /// The callback should ask the user whether to apply the edit.
        /// </summary>
        public event YesNoCancelCallback DisruptingEdit;

        /// <summary>
        /// Raised when an action would result in losing unsaved changes.
        /// The callback should ask the user whether to save the project.
        /// </summary>
        public event YesNoCancelCallback DiscardingUnsavedChanges;

        /// <summary>
        /// Raised before removing an entry.
        /// The callback should confirm that the removal is intentional.
        /// </summary>
        public event YesNoCallback RemovingEntry;

        /// <summary>
        /// Raised when a file name is required for an open operation.
        /// The callback should open a file save dialog, with the default filter matching <see cref="SelectedExporter"/>.
        /// If the filter is changed, <see cref="SelectedExporter"/> must be updated.
        /// If the operation is canceled, null must be returned.
        /// </summary>
        public event StringCallback SelectingExportFilename;

        /// <summary>
        /// Raised when a file name is required for an open operation.
        /// The callback should open a file open dialog.
        /// If the operation is canceled, null must be returned.
        /// </summary>
        public event StringCallback SelectingOpenFilename;

        /// <summary>
        /// Raised when a file name is required for a save operation.
        /// The callback should open a file save dialog.
        /// If the save is canceled, null must be returned.
        /// </summary>
        public event StringCallback SelectingSaveFilename;

        /// <summary>
        /// Constructs a MainWindowViewModel with an empty catalogue.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize to an empty catalogue
            Catalogue = new CatalogueViewModel(new Catalogue());
            ProjectName = string.Empty;

            // Initialize the exporters
            Exporters = new List<CatalogueExporter>();
            Exporters.Add(new BibTexExporter());
            SelectedExporter = Exporters[0];
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
            IsModified = true;

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
        /// Exports the catalogue.
        /// </summary>
        /// <returns>True if successful, false if canceled or failed.</returns>
        public bool ExportCatalogue()
        {
            if (ShouldCancelBecauseOfEdit())
                return false;

            // Crashes if the event is not assigned
            var filename = SelectingExportFilename();
            if (filename == null)
                return false;

            return SelectedExporter.Export(filename, Catalogue._catalogue);
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public void NewProject()
        {
            if (ShouldCancelBecauseOfEdit())
                return;
            if (ShouldCancelBecauseOfUnsavedChanges())
                return;

            // Clear the selected item and the catalogue
            SelectedEntry = null;
            Catalogue = new CatalogueViewModel(new Catalogue());
            ProjectName = string.Empty;
        }

        /// <summary>
        /// Opens a project from the disk.
        /// </summary>
        /// <returns>True if the project was opened, false if canceled or failed.</returns>
        public bool OpenProject()
        {
            if (ShouldCancelBecauseOfEdit())
                return false;
            if (ShouldCancelBecauseOfUnsavedChanges())
                return false;
            
            var filename = SelectingOpenFilename();
            try
            {
                using (var stream = File.OpenRead(filename))
                {
                    var catalogue = Project.Deserialize(stream);
                    if (catalogue == null)
                        return false;

                    Catalogue = new CatalogueViewModel(catalogue);
                }
            }
            catch (ArgumentException) { return false; }
            catch (IOException) { return false; }

            Filename = filename;
            ProjectName = Path.GetFileNameWithoutExtension(filename);
            IsModified = false;
            SelectedEntry = null;

            return true;
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

                IsModified = true;
            }
        }

        /// <summary>
        /// Saves the project to file.
        /// </summary>
        /// <param name="saveAs">If true, a new file name is asked.</param>
        /// <returns>True if the project was saved, false if the save was canceled or failed.</returns>
        public bool SaveProject(bool saveAs)
        {
            if (ShouldCancelBecauseOfEdit())
                return false;

            // This crashes if the event is not hooked
            var filename = Filename;
            if (saveAs || string.IsNullOrEmpty(filename))
                filename = SelectingSaveFilename();
            if (filename == null)
                return false;

            // Open the file stream and serialize
            try
            {
                using (var stream = File.Open(filename, FileMode.Create))
                {
                    Project.Serialize(stream, Catalogue._catalogue);
                }
            }
            catch (ArgumentException) { return false; }
            catch (IOException) { return false; }

            // After a successful save the filename may be saved for later
            Filename = filename;
            ProjectName = Path.GetFileNameWithoutExtension(filename);
            IsModified = false;

            return true;
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

        private bool ShouldCancelBecauseOfUnsavedChanges()
        {
            if (IsModified)
            {
                // This will crash if the handler is not registered.
                // It is the sanest thing to do.
                var result = DiscardingUnsavedChanges();
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        return !SaveProject(false);
                    case MessageBoxResult.No:
                        return false;
                    default:
                        return true;
                }
            }
            return false;
        }
    }
}
