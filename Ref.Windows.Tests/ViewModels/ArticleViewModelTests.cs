using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class ArticleViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var article = TestUtility.CreateCounterexample();
            var vm = new ArticleViewModel(article);

            Assert.That(vm.Author, Is.EqualTo(article.Author));
            Assert.That(vm.Doi, Is.EqualTo(article.Doi));
            Assert.That(vm.Issn, Is.EqualTo(article.Issn));
            Assert.That(vm.Journal, Is.EqualTo(article.Journal));
            Assert.That(vm.Key, Is.EqualTo(article.Key));
            Assert.That(vm.Notes, Is.EqualTo(article.Notes));
            Assert.That(vm.Number, Is.EqualTo(article.Number));
            Assert.That(vm.PageRange, Is.EqualTo(article.PageRange));
            Assert.That(vm.Title, Is.EqualTo(article.Title));
            Assert.That(vm.Volume, Is.EqualTo(article.Volume));
            Assert.That(vm.Year, Is.EqualTo(article.Year));

            Assert.That(vm.IsReadOnly, Is.True);
        }

        [Test]
        public void Ctor_CopiesPages()
        {
            var article = TestUtility.CreateCounterexample();
            article.Pages.Add(CreateExampleQuote());
            var vm = new ArticleViewModel(article);

            Assert.That(vm.Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
            Assert.That(vm.Pages[0].PageRange, Is.EqualTo("1079"));
        }

        [Test]
        public void AddPage_AddsPage()
        {
            var article = TestUtility.CreateCounterexample();
            var vm = new ArticleViewModel(article);
            var page = CreateExampleQuote();
            var pageVM = new PageViewModel(page);

            Assert.That(() => vm.AddPage(pageVM), Throws.Nothing);
            Assert.That(vm.Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
            Assert.That(vm.Pages[0].PageRange, Is.EqualTo("1079"));
            Assert.That(article.Pages, Has.Exactly(1).InstanceOf<Page>());
            Assert.That(article.Pages[0].PageRange, Is.EqualTo("1079"));
        }

        [Test]
        public void AddPage_ThrowsIfNull()
        {
            var vm = new ArticleViewModel(TestUtility.CreateCounterexample());

            Assert.That(() => vm.AddPage(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Cancel_ResetsProperties()
        {
            var vm = new ArticleViewModel(TestUtility.CreateCounterexample());
            var originalDoi = vm.Doi;

            vm.Edit();
            vm.Doi = "10.404";

            Assert.That(() => vm.Cancel(), Throws.Nothing);
            Assert.That(vm.Doi, Is.EqualTo(originalDoi));
            Assert.That(((Article)vm._model).Doi, Is.EqualTo(originalDoi));
        }

        [Test]
        public void Commit_CommitsChanges()
        {
            var vm = new ArticleViewModel(TestUtility.CreateCounterexample());

            vm.Edit();
            vm.Doi = "10.404";

            Assert.That(() => vm.Commit(), Throws.Nothing);
            Assert.That(vm.Doi, Is.EqualTo("10.404"));
            Assert.That(((Article)vm._model).Doi, Is.EqualTo("10.404"));
        }

        [Test]
        public void IsReadOnly_IsEnforced()
        {
            var vm = new ArticleViewModel(TestUtility.CreateCounterexample());

            Assert.That(() => { vm.Doi = "10.404"; }, Throws.InvalidOperationException);
        }

        [Test]
        public void RemovePage_RemovesPage()
        {
            var vm = new ArticleViewModel(TestUtility.CreateCounterexample());
            var page = new PageViewModel(CreateExampleQuote());
            vm.AddPage(page);

            Assert.That(() => vm.RemovePage(page), Throws.Nothing);
            Assert.That(vm.Pages, Is.Empty);
            Assert.That(vm._model.Pages, Is.Empty);
        }

        private static Page CreateExampleQuote()
        {
            return new Page()
            {
                Notes = "27^5 + 84^5 + 110^5 + 133^5 = 144^5",
                PageRange = "1079",
                Title = "The Counterexample"
            };
        }
    }
}
