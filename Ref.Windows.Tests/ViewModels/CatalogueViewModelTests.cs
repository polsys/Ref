using NUnit.Framework;
using Ref.Windows.Models;
using Ref.Windows.ViewModels;

namespace Ref.Windows.Tests.ViewModels
{
    class CatalogueViewModelTests
    {
        [Test]
        public void Ctor_CreatesEntries()
        {
            var catalogue = new Catalogue();
            catalogue.Entries.Add(new Book() { Title = "Python Guido" });
            catalogue.Entries.Add(new Book() { Title = "Spam + Eggs" });

            var vm = new CatalogueViewModel(catalogue);

            Assert.That(vm.Entries.Count, Is.EqualTo(2));
            Assert.That(vm.Entries, Has.Exactly(1).With.Property("Title").EqualTo("Python Guido"));
            Assert.That(vm.Entries, Has.Exactly(1).With.Property("Title").EqualTo("Spam + Eggs"));
        }

        [Test]
        public void AddBook_AddsBook()
        {
            var catalogue = new Catalogue();
            var catalogueVM = new CatalogueViewModel(catalogue);
            var book = new BookViewModel(new Book());
            catalogueVM.AddBook(book);

            Assert.That(book, Is.InstanceOf<BookViewModel>());
            Assert.That(catalogueVM.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Book>());
        }
    }
}
