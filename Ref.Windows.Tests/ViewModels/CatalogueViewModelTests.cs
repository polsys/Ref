using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class CatalogueViewModelTests
    {
        [Test]
        public void Ctor_CreatesEntries()
        {
            var catalogue = new Catalogue();
            catalogue.Entries.Add(new Book() { Title = "Python Guido" });
            catalogue.Entries.Add(new Article() { Title = "Spam + Eggs" });

            var vm = new CatalogueViewModel(catalogue);

            Assert.That(vm.Entries.Count, Is.EqualTo(2));
            Assert.That(vm.Entries, Has.Exactly(1).With.Property("Title").EqualTo("Python Guido"));
            Assert.That(vm.Entries, Has.Exactly(1).With.Property("Title").EqualTo("Spam + Eggs"));
        }

        [Test]
        public void AddEntry_AddsArticle()
        {
            var catalogue = new Catalogue();
            var catalogueVM = new CatalogueViewModel(catalogue);
            var article = new ArticleViewModel(new Article());
            catalogueVM.AddEntry(article);

            Assert.That(catalogueVM.Entries, Has.Exactly(1).InstanceOf<ArticleViewModel>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Article>());
        }

        [Test]
        public void AddEntry_AddsBook()
        {
            var catalogue = new Catalogue();
            var catalogueVM = new CatalogueViewModel(catalogue);
            var book = new BookViewModel(new Book());
            catalogueVM.AddEntry(book);
            
            Assert.That(catalogueVM.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Book>());
        }

        [Test]
        public void RemoveEntry_RemovesArticle()
        {
            var catalogue = new Catalogue();
            var catalogueVM = new CatalogueViewModel(catalogue);
            var article = new ArticleViewModel(new Article());
            catalogueVM.AddEntry(article);

            catalogueVM.RemoveEntry(article);

            Assert.That(catalogueVM.Entries, Is.Empty);
            Assert.That(catalogue.Entries, Is.Empty);
        }

        [Test]
        public void RemoveEntry_RemovesBook()
        {
            var catalogue = new Catalogue();
            var catalogueVM = new CatalogueViewModel(catalogue);
            var book = new BookViewModel(new Book());
            catalogueVM.AddEntry(book);

            catalogueVM.RemoveEntry(book);

            Assert.That(catalogueVM.Entries, Is.Empty);
            Assert.That(catalogue.Entries, Is.Empty);
        }
    }
}
