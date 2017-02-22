using System.Windows;
using NUnit.Framework;
using Polsys.Ref.Export;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    partial class MainWindowViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var vm = new MainWindowViewModel();

            Assert.That(vm.Catalogue, Is.Not.Null);
            Assert.That(vm.ProjectName, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.Null);
        }

        [Test]
        public void Ctor_InitializesExporters()
        {
            var vm = new MainWindowViewModel();

            Assert.That(vm.Exporters, Has.Exactly(1).InstanceOf<BibTexExporter>());
            Assert.That(vm.SelectedExporter, Is.InstanceOf<BibTexExporter>());
        }

        [Test]
        public void AddPage_AddsPageToSelectedPagesParent()
        {
            var vm = new MainWindowViewModel();
            var book = new BookViewModel(TestUtility.CreateMakeAndDo());
            var page = new PageViewModel(CreateOnKnotsPage());
            page._parent = book;
            book.AddPage(page);
            vm.Catalogue.AddEntry(book);

            // Select the existing page and then add a new page
            vm.SelectEntry(page);
            Assert.That(() => vm.AddPage(), Throws.Nothing);
            Assert.That(vm.SelectedEntry, Is.InstanceOf<PageViewModel>().And.Not.EqualTo(page));
            Assert.That(((PageViewModel)vm.SelectedEntry)._parent, Is.SameAs(book));
        }

        [Test]
        public void AddPage_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            bool eventFired = false;
            vm.DisruptingEdit += () => {

                eventFired = true;
                return MessageBoxResult.Cancel;
            };
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);
            vm.EditSelected();

            Assert.That(() => vm.AddPage(), Throws.Nothing);
            Assert.That(eventFired, Is.True);
            Assert.That(vm.SelectedEntry, Is.SameAs(bookVM));
            Assert.That(bookVM.IsReadOnly, Is.False);
        }

        [Test]
        public void AddPage_CancelDiscardsPage()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            vm.AddPage();
            vm.CancelEdit();

            Assert.That(bookVM.Pages, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.SameAs(bookVM));
        }

        [Test]
        public void AddPage_CommitAddsPage()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            vm.AddPage();
            vm.SelectedEntry.Title = "Nice Quote";
            vm.CommitEdit();

            Assert.That(bookVM.Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
            Assert.That(bookVM.Pages[0].Title, Is.EqualTo("Nice Quote"));
            Assert.That(vm.SelectedEntry, Is.InstanceOf<PageViewModel>());
            Assert.That(((PageViewModel)vm.SelectedEntry)._parent, Is.SameAs(bookVM));
            Assert.That(vm.IsModified, Is.True);
        }

        [Test]
        public void AddPage_CreatesAndEditsPage()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            vm.AddPage();
            Assert.That(vm.SelectedEntry, Is.InstanceOf<PageViewModel>());
            Assert.That(vm.SelectedEntry.IsReadOnly, Is.False);
            Assert.That(((PageViewModel)vm.SelectedEntry)._parent, Is.SameAs(bookVM));
        }

        [Test]
        public void CancelEdit_DiscardsChanges()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddEntry(bookVM);

            vm.SelectEntry(bookVM);
            vm.EditSelected();
            bookVM.Title = "Bad Name";
            vm.CancelEdit();

            Assert.That(bookVM.Title, Is.EqualTo("Nice Name"));
            Assert.That(bookVM.IsReadOnly, Is.True);
        }

        [Test]
        public void Close_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Cancel; };
            vm.CreateBook();

            Assert.That(vm.Close(), Is.EqualTo(OperationResult.Canceled));
        }

        [Test]
        public void Close_AsksIfUnsavedChanges()
        {
            var vm = new MainWindowViewModel();
            vm.DiscardingUnsavedChanges += () => { return MessageBoxResult.Cancel; };
            vm.CreateBook();
            vm.CommitEdit();

            Assert.That(vm.Close(), Is.EqualTo(OperationResult.Canceled));
        }

        [Test]
        public void Close_Succeeds()
        {
            var vm = new MainWindowViewModel();
            Assert.That(vm.Close(), Is.EqualTo(OperationResult.Succeeded));
        }

        [Test]
        public void CommitEdit_AppliesChanges()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddEntry(bookVM);

            vm.SelectEntry(bookVM);
            vm.EditSelected();
            bookVM.Title = "Great Name";
            vm.CommitEdit();

            Assert.That(bookVM.Title, Is.EqualTo("Great Name"));
            Assert.That(bookVM.IsReadOnly, Is.True);
            Assert.That(vm.IsModified, Is.True);
        }

        [Test]
        public void CreateArticle_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            vm.CreateBook(); // Starts editing a book

            bool wasAsked = false;
            vm.DisruptingEdit += () =>
            {
                wasAsked = true;
                return MessageBoxResult.Cancel;
            };

            // Since the handler returned Cancel, the article must not be created
            vm.CreateArticle();
            Assert.That(wasAsked, Is.True);
            Assert.That(vm.SelectedEntry, Is.InstanceOf<BookViewModel>());
        }

        [Test]
        public void CreateArticle_CancelDoesNotAddArticle()
        {
            var vm = new MainWindowViewModel();

            vm.CreateArticle();
            vm.CancelEdit();

            Assert.That(vm.Catalogue.Entries, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.Null);
        }

        [Test]
        public void CreateArticle_CommitAddsArticle()
        {
            var vm = new MainWindowViewModel();
            vm.CreateArticle();
            vm.SelectedEntry.Title = "Modular elliptic curves and Fermat's last theorem";
            vm.CommitEdit();

            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<ArticleViewModel>());
            Assert.That(vm.Catalogue.Entries[0].Title, Does.Contain("Fermat"));
        }

        [Test]
        public void CreateArticle_CreatesAndEditsArticle()
        {
            var vm = new MainWindowViewModel();
            vm.CreateArticle();

            Assert.That(vm.SelectedEntry, Is.InstanceOf<ArticleViewModel>());
            Assert.That(vm.SelectedEntry.IsReadOnly, Is.False);
        }

        [Test]
        public void CreateBook_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);
            vm.EditSelected();

            bool wasAsked = false;
            vm.DisruptingEdit += () =>
            {
                wasAsked = true;
                return MessageBoxResult.Cancel;
            };

            // Since the handler returned Cancel, the book must not be created
            vm.CreateBook();
            Assert.That(wasAsked, Is.True);
            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
            Assert.That(vm.SelectedEntry, Is.SameAs(bookVM));
        }

        [Test]
        public void CreateBook_CancelDoesNotAddBook()
        {
            var vm = new MainWindowViewModel();
            vm.CreateBook();

            vm.CancelEdit();
            Assert.That(vm.Catalogue.Entries, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.Null);
        }

        [Test]
        public void CreateBook_CommitAddsBook()
        {
            var vm = new MainWindowViewModel();
            vm.CreateBook();

            vm.SelectedEntry.Title = "Nice Book";
            vm.CommitEdit();

            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).With.Property("Title").EqualTo("Nice Book"));
            Assert.That(vm.IsModified, Is.True);
        }

        [Test]
        public void CreateBook_CommitSelectsBook()
        {
            // This test is not entirely correct...
            // WPF ensures that only one item is selected, and therefore no such logic is in the view model.
            // SelectEntry does not actually set IsSelected.
            // Still leaving this here for future generations (= me next month) to wonder about.
            // At least the "book2.IsSelected == true" assert is correct.

            var vm = new MainWindowViewModel();
            var book1 = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(book1);
            vm.SelectEntry(book1);

            vm.CreateBook();
            var book2 = vm.SelectedEntry;
            vm.CommitEdit();

            Assert.That(book1.IsSelected, Is.False);
            Assert.That(book2.IsSelected, Is.True);
        }

        [Test]
        public void CreateBook_CreatesAndEditsNewBook()
        {
            var vm = new MainWindowViewModel();
            Assert.That(vm.SelectedEntry, Is.Null);

            vm.CreateBook();
            Assert.That(vm.SelectedEntry, Is.InstanceOf<BookViewModel>());
            Assert.That(vm.SelectedEntry.IsReadOnly, Is.False);
        }

        [Test]
        public void CreateThesis_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            vm.CreateBook(); // Starts editing a book

            bool wasAsked = false;
            vm.DisruptingEdit += () =>
            {
                wasAsked = true;
                return MessageBoxResult.Cancel;
            };

            // Since the handler returned Cancel, the article must not be created
            vm.CreateThesis();
            Assert.That(wasAsked, Is.True);
            Assert.That(vm.SelectedEntry, Is.InstanceOf<BookViewModel>());
        }

        [Test]
        public void CreateThesis_CreatesAndEditsThesis()
        {
            var vm = new MainWindowViewModel();
            vm.CreateThesis();

            Assert.That(vm.SelectedEntry, Is.InstanceOf<ThesisViewModel>());
            Assert.That(vm.SelectedEntry.IsReadOnly, Is.False);
        }

        [Test]
        public void EditSelected_StartsEdit()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddEntry(bookVM);

            vm.SelectEntry(bookVM);
            Assert.That(bookVM.IsReadOnly, Is.True);

            vm.EditSelected();
            Assert.That(bookVM.IsReadOnly, Is.False);
        }

        [Test]
        public void OpenCopyReferenceDialog_CallsCallbackWithEntry()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "To be copied" });
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            int timesCalled = 0;
            vm.OpeningCopyReferenceDialog += (PublicationViewModelBase entry) =>
            {
                Assert.That(entry.Title, Is.EqualTo("To be copied"));
                timesCalled++;
            };
            Assert.That(() => vm.OpenCopyReferenceDialog(), Throws.Nothing);
            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void OpenCopyReferenceDialog_UsesParentEntryFromPage()
        {
            var vm = new MainWindowViewModel();
            var book = new Book() { Title = "To be copied" };
            book.Pages.Add(new Page() { Title = "Hello" });
            var bookVM = new BookViewModel(book);
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM.Pages[0]);

            int timesCalled = 0;
            vm.OpeningCopyReferenceDialog += (PublicationViewModelBase entry) =>
            {
                Assert.That(entry.Title, Is.EqualTo("To be copied"));
                timesCalled++;
            };
            Assert.That(() => vm.OpenCopyReferenceDialog(), Throws.Nothing);
            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void OpenCopyReferenceDialog_DoesNotThrowIfNoCallback()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "To be copied" });
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            Assert.That(() => vm.OpenCopyReferenceDialog(), Throws.Nothing);
        }

        [Test]
        public void OpenCopyReferenceDialog_ThrowsIfNoneSelected()
        {
            var vm = new MainWindowViewModel();

            Assert.That(() => vm.OpenCopyReferenceDialog(), Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveSelected_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Cancel; };
            var bookVM = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);
            vm.EditSelected();

            // The remove operation must be canceled
            vm.RemoveSelected();
            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
        }

        [Test]
        public void RemoveSelected_CancelsRemove()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            vm.RemovingEntry += () => { return MessageBoxResult.No; };
            vm.RemoveSelected();
            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
        }

        [Test]
        public void RemoveSelected_RemovesFromCatalogue()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(bookVM);
            vm.SelectEntry(bookVM);

            vm.RemovingEntry += () => { return MessageBoxResult.Yes; };
            vm.RemoveSelected();
            Assert.That(vm.Catalogue.Entries, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.Null);
            Assert.That(vm.IsModified, Is.True);
        }

        [Test]
        public void RemoveSelected_RemovesPage()
        {
            var vm = new MainWindowViewModel();
            vm.RemovingEntry += () => { return MessageBoxResult.Yes; };
            var book = new BookViewModel(TestUtility.CreateMakeAndDo());
            var page = new PageViewModel(CreateOnKnotsPage());
            page._parent = book;
            book.AddPage(page);
            vm.Catalogue.AddEntry(book);

            vm.SelectEntry(page);
            Assert.That(() => vm.RemoveSelected(), Throws.Nothing);
            Assert.That(book.Pages, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.SameAs(book));
            Assert.That(vm.IsModified, Is.True);
        }

        [Test]
        public void SelectEntry_AsksAndAppliesPendingEdit()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Yes; };

            // Set up: have two books and edit one of them
            var book1 = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(book1);
            var book2 = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Catalogue.AddEntry(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned Yes, the edit must be applied before switching
            Assert.That(vm.SelectEntry(book2), Is.EqualTo(OperationResult.Succeeded));
            Assert.That(book1.IsReadOnly, Is.True);
            Assert.That(book1.Author, Is.EqualTo("@standupmaths"));
            Assert.That(vm.SelectedEntry, Is.SameAs(book2));
        }

        [Test]
        public void SelectEntry_AsksAndCancelsPendingEdit()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.No; };

            // Set up: have two books and edit one of them
            var book1 = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(book1);
            var book2 = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Catalogue.AddEntry(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned No, the edit must be reverted before switching
            Assert.That(vm.SelectEntry(book2), Is.EqualTo(OperationResult.Succeeded));
            Assert.That(book1.IsReadOnly, Is.True);
            Assert.That(book1.Author, Is.EqualTo("Parker, Matt"));
            Assert.That(vm.SelectedEntry, Is.SameAs(book2));
        }

        [Test]
        public void SelectEntry_AsksAndReturnsToPendingEdit()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Cancel; };

            // Set up: have two books and edit one of them
            var book1 = new BookViewModel(TestUtility.CreateMakeAndDo());
            vm.Catalogue.AddEntry(book1);
            var book2 = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Catalogue.AddEntry(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned Cancel, there must be no change
            Assert.That(vm.SelectEntry(book2), Is.EqualTo(OperationResult.Canceled));
            Assert.That(book1.IsReadOnly, Is.False);
            Assert.That(book1.Author, Is.EqualTo("@standupmaths"));
            Assert.That(vm.SelectedEntry, Is.SameAs(book1));
        }

        [Test]
        public void SelectEntry_DoesNotSelectSame()
        {
            // The no-op behavior is checked by having an edit in progress.
            // Selecting the selected item should not raise the disruption event.
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { Assert.Fail(); return MessageBoxResult.No; };
            var book = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Catalogue.AddEntry(book);

            // Select the book and enter edit mode
            vm.SelectEntry(book);
            vm.EditSelected();

            // This must not raise the event
            // The result must be Canceled, since the selection was not applied
            Assert.That(vm.SelectEntry(book), Is.EqualTo(OperationResult.Canceled));
        }

        [Test]
        public void SelectEntry_SelectsAndRaisesEvent()
        {
            var vm = new MainWindowViewModel();
            var entry = new BookViewModel(new Book() { Title = "Test 1" });
            vm.Catalogue.AddEntry(entry);

            // Simultaneously check that SelectEntry succeeds and raises the event
            TestUtility.AssertRaisesPropertyChanged(vm, () => {
                Assert.That(vm.SelectEntry(entry), Is.EqualTo(OperationResult.Succeeded));
                }, "SelectedEntry");
            Assert.That(vm.SelectedEntry, Is.SameAs(entry));
        }

        [Test]
        public void SelectEntry_SelectsPage()
        {
            var vm = new MainWindowViewModel();
            var book = new BookViewModel(TestUtility.CreateMakeAndDo());
            var page = new PageViewModel(CreateOnKnotsPage());
            book.AddPage(page);
            vm.Catalogue.AddEntry(book);

            Assert.That(vm.SelectEntry(page), Is.EqualTo(OperationResult.Succeeded));
            Assert.That(vm.SelectedEntry, Is.SameAs(page));
        }

        private static Page CreateOnKnotsPage()
        {
            return new Page()
            {
                Notes = "Technically, the unknot is not not a knot, it's a trivial knot.",
                PageRange = "165",
                Title = "On knots"
            };
        }
    }
}
