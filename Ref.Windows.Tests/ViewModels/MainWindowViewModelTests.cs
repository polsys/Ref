using System.Windows;
using NUnit.Framework;
using Ref.Windows.Models;
using Ref.Windows.ViewModels;

namespace Ref.Windows.Tests.ViewModels
{
    class MainWindowViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var vm = new MainWindowViewModel();

            Assert.That(vm.Catalogue, Is.Not.Null);
            Assert.That(vm.SelectedEntry, Is.Null);
        }

        [Test]
        public void CancelEdit_DiscardsChanges()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddBook(bookVM);

            vm.SelectEntry(bookVM);
            vm.EditSelected();
            bookVM.Title = "Bad Name";
            vm.CancelEdit();

            Assert.That(bookVM.Title, Is.EqualTo("Nice Name"));
            Assert.That(bookVM.IsReadOnly, Is.True);
        }

        [Test]
        public void CommitEdit_AppliesChanges()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddBook(bookVM);

            vm.SelectEntry(bookVM);
            vm.EditSelected();
            bookVM.Title = "Great Name";
            vm.CommitEdit();

            Assert.That(bookVM.Title, Is.EqualTo("Great Name"));
            Assert.That(bookVM.IsReadOnly, Is.True);
        }

        [Test]
        public void CreateBook_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(bookVM);
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
            var book1 = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(book1);
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
        public void EditSelected_StartsEdit()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(new Book() { Title = "Nice Name" });
            vm.Catalogue.AddBook(bookVM);

            vm.SelectEntry(bookVM);
            Assert.That(bookVM.IsReadOnly, Is.True);

            vm.EditSelected();
            Assert.That(bookVM.IsReadOnly, Is.False);
        }

        [Test]
        public void RemoveSelected_AsksIfEditing()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Cancel; };
            var bookVM = new BookViewModel(CreateCrackingMathematics());
            vm.Catalogue.AddBook(bookVM);
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
            var bookVM = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(bookVM);
            vm.SelectEntry(bookVM);

            vm.RemovingEntry += () => { return MessageBoxResult.No; };
            vm.RemoveSelected();
            Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
        }

        [Test]
        public void RemoveSelected_RemovesFromCatalogue()
        {
            var vm = new MainWindowViewModel();
            var bookVM = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(bookVM);
            vm.SelectEntry(bookVM);

            vm.RemovingEntry += () => { return MessageBoxResult.Yes; };
            vm.RemoveSelected();
            Assert.That(vm.Catalogue.Entries, Is.Empty);
            Assert.That(vm.SelectedEntry, Is.Null);
        }

        [Test]
        public void SelectEntry_AsksAndAppliesPendingEdit()
        {
            var vm = new MainWindowViewModel();
            vm.DisruptingEdit += () => { return MessageBoxResult.Yes; };

            // Set up: have two books and edit one of them
            var book1 = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(book1);
            var book2 = new BookViewModel(CreateCrackingMathematics());
            vm.Catalogue.AddBook(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned Yes, the edit must be applied before switching
            Assert.That(vm.SelectEntry(book2), Is.True);
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
            var book1 = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(book1);
            var book2 = new BookViewModel(CreateCrackingMathematics());
            vm.Catalogue.AddBook(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned No, the edit must be reverted before switching
            Assert.That(vm.SelectEntry(book2), Is.True);
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
            var book1 = new BookViewModel(CreateMakeAndDo());
            vm.Catalogue.AddBook(book1);
            var book2 = new BookViewModel(CreateCrackingMathematics());
            vm.Catalogue.AddBook(book2);
            vm.SelectEntry(book1);
            vm.EditSelected();
            book1.Author = "@standupmaths";

            // Now select the other book
            // Since the handler returned Cancel, there must be no change
            Assert.That(vm.SelectEntry(book2), Is.False);
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
            var book = new BookViewModel(CreateCrackingMathematics());
            vm.Catalogue.AddBook(book);

            // Select the book and enter edit mode
            vm.SelectEntry(book);
            vm.EditSelected();

            // This must return false and not raise the event
            Assert.That(vm.SelectEntry(book), Is.False);
        }

        [Test]
        public void SelectEntry_SelectsAndRaisesEvent()
        {
            var vm = new MainWindowViewModel();
            var entry = new BookViewModel(new Book() { Title = "Test 1" });
            vm.Catalogue.AddBook(entry);

            // Simultaneously check that SelectEntry returns true and raises the event
            TestUtility.AssertRaisesPropertyChanged(vm, () => {
                Assert.That(vm.SelectEntry(entry), Is.True);
                }, "SelectedEntry");
            Assert.That(vm.SelectedEntry, Is.SameAs(entry));
        }

        private static Book CreateCrackingMathematics()
        {
            return new Book()
            {
                Author = "Beveridge, Colin",
                Key = "Beveridge2016",
                Publisher = "Octopus Books",
                Title = "Cracking Mathematics",
                Year = "2016"
            };
        }

        private static Book CreateMakeAndDo()
        {
            return new Book()
            {
                Author = "Parker, Matt",
                Key = "Parker2014",
                Publisher = "Particular Books",
                Title = "Things to Make and Do in the Fourth Dimension",
                Year = "2014"
            };
        }
    }
}
