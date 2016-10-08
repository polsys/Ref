using NUnit.Framework;
using Ref.Windows.Models;
using Ref.Windows.ViewModels;

namespace Ref.Windows.Tests.ViewModels
{
    class BookViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var book = CreateCrackingMathematics();
            var vm = new BookViewModel(book);

            Assert.That(vm.Author, Is.EqualTo(book.Author));
            Assert.That(vm.Key, Is.EqualTo(book.Key));
            Assert.That(vm.Publisher, Is.EqualTo(book.Publisher));
            Assert.That(vm.Title, Is.EqualTo(book.Title));
            Assert.That(vm.Year, Is.EqualTo(book.Year));

            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Cancel_ResetsProperties()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());
            vm.Edit();
            vm.Author = "@icecolbeveridge";

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "Author");
            Assert.That(vm.Author, Is.EqualTo("Beveridge, Colin"));
        }

        [Test]
        public void Cancel_SetsReadOnly()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());
            vm.Edit();
            
            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Cancel_ThrowsIfReadOnly()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());
            Assert.That(() => vm.Cancel(), Throws.InvalidOperationException);
        }

        [Test]
        public void Commit_CommitsChanges()
        {
            var book = CreateCrackingMathematics();
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
            var vm = new BookViewModel(CreateCrackingMathematics());
            vm.Edit();

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Commit(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Commit_ThrowsIfReadOnly()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());
            Assert.That(() => vm.Commit(), Throws.InvalidOperationException);
        }

        [Test]
        public void Edit_SetsReadOnly()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Edit(), "IsReadOnly");
            Assert.That(vm.IsReadOnly, Is.False);
        }

        [Test]
        public void Edit_ThrowsIfCalledTwice()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());

            Assert.That(() => vm.Edit(), Throws.Nothing);
            Assert.That(() => vm.Edit(), Throws.InvalidOperationException);
        }

        [Test]
        public void IsReadOnly_IsEnforced()
        {
            var vm = new BookViewModel(CreateCrackingMathematics());

            Assert.That(() => { vm.Author = "@icecolbeveridge"; }, Throws.InvalidOperationException);
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
    }
}
