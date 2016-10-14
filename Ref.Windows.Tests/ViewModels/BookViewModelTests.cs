using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class BookViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var book = TestUtility.CreateCrackingMathematics();
            var vm = new BookViewModel(book);

            Assert.That(vm.Author, Is.EqualTo(book.Author));
            Assert.That(vm.Key, Is.EqualTo(book.Key));
            Assert.That(vm.Publisher, Is.EqualTo(book.Publisher));
            Assert.That(vm.Title, Is.EqualTo(book.Title));
            Assert.That(vm.Year, Is.EqualTo(book.Year));

            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Ctor_CopiesPages()
        {
            var book = TestUtility.CreateCrackingMathematics();
            book.Pages.Add(CreateOnEuler());
            book.Pages.Add(CreateHilbertQuote());

            var vm = new BookViewModel(book);
            Assert.That(vm.Pages.Count, Is.EqualTo(2));
            Assert.That(vm.Pages[0].Title, Is.EqualTo("On Euler"));
            Assert.That(vm.Pages[1].Title, Is.EqualTo("Hilbert Quote"));
        }

        [Test]
        public void AddPage_AddsPage()
        {
            var book = TestUtility.CreateCrackingMathematics();
            var vm = new BookViewModel(book);
            var page = new PageViewModel(CreateHilbertQuote());

            Assert.That(() => vm.AddPage(page), Throws.Nothing);
            Assert.That(vm.Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
            Assert.That(vm.Pages[0].Title, Is.EqualTo("Hilbert Quote"));
            Assert.That(book.Pages, Has.Exactly(1).InstanceOf<Page>());
            Assert.That(book.Pages[0].Title, Is.EqualTo("Hilbert Quote"));
        }

        [Test]
        public void AddPage_NotAffectedByCancel()
        {
            // Editing properties and adding pages are separate operations,
            // and must not affect each other

            var book = TestUtility.CreateCrackingMathematics();
            var vm = new BookViewModel(book);
            var page = new PageViewModel(CreateHilbertQuote());

            vm.Edit();
            vm.AddPage(page);
            vm.Cancel();

            Assert.That(vm.Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
            Assert.That(vm.Pages[0].Title, Is.EqualTo("Hilbert Quote"));
        }

        [Test]
        public void AddPage_ThrowsIfNull()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            Assert.That(() => vm.AddPage(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Cancel_ResetsProperties()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Edit();
            vm.Author = "@icecolbeveridge";

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "Author");
            Assert.That(vm.Author, Is.EqualTo("Beveridge, Colin"));
        }

        [Test]
        public void Cancel_SetsReadOnly()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Edit();
            
            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Cancel_ThrowsIfReadOnly()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            Assert.That(() => vm.Cancel(), Throws.InvalidOperationException);
        }

        [Test]
        public void Commit_CommitsChanges()
        {
            var book = TestUtility.CreateCrackingMathematics();
            var vm = new BookViewModel(book);
            vm.Edit();
            vm.Author = "@icecolbeveridge";
            vm.Commit();

            Assert.That(vm.Author, Is.EqualTo("@icecolbeveridge"));
            Assert.That(book.Author, Is.EqualTo("@icecolbeveridge"));
        }

        [Test]
        public void Commit_SetsReadOnly()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            vm.Edit();

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Commit(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Commit_ThrowsIfReadOnly()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            Assert.That(() => vm.Commit(), Throws.InvalidOperationException);
        }

        [Test]
        public void Edit_SetsReadOnly()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Edit(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.False);
        }

        [Test]
        public void Edit_ThrowsIfCalledTwice()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());

            Assert.That(() => vm.Edit(), Throws.Nothing);
            Assert.That(() => vm.Edit(), Throws.InvalidOperationException);
        }

        [Test]
        public void IsReadOnly_IsEnforced()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());

            Assert.That(() => { vm.Author = "@icecolbeveridge"; }, Throws.InvalidOperationException);
        }

        [Test]
        public void RemovePage_RemovesPage()
        {
            var vm = new BookViewModel(TestUtility.CreateCrackingMathematics());
            var page = new PageViewModel(CreateHilbertQuote());
            vm.AddPage(page);

            Assert.That(() => vm.RemovePage(page), Throws.Nothing);
            Assert.That(vm.Pages, Is.Empty);
            Assert.That(vm._book.Pages, Is.Empty);
        }

        private static Page CreateHilbertQuote()
        {
            return new Page()
            {
                Notes = @"'Good, he didn't have enough imagination to become a mathematician.'
-- [David] Hilbert, on hearing a student had dropped out to become a poet.",
                PageRange = "207",
                Title = "Hilbert Quote"
            };
        }

        private static Page CreateOnEuler()
        {
            return new Page()
            {
                Notes = "There's a joke in maths that everything is named after the second person to discover it, " +
                "or else nearly everything would be named after Leonard Euler.",
                PageRange = "164",
                Title = "On Euler"
            };
        }
    }
}
