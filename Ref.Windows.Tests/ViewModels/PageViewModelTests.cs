using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class PageViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var page = CreateOnElements();
            var vm = new PageViewModel(page);

            Assert.That(vm.Notes, Is.EqualTo(page.Notes));
            Assert.That(vm.PageRange, Is.EqualTo(page.PageRange));
            Assert.That(vm.Title, Is.EqualTo(page.Title));
        }

        [Test]
        public void Cancel_DiscardsChanges()
        {
            var vm = new PageViewModel(CreateOnElements());
            vm.Edit();
            vm.PageRange = "34--37";

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "PageRange");

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.PageRange, Is.EqualTo("34"));
            Assert.That(vm._page.PageRange, Is.EqualTo("34"));
        }

        [Test]
        public void Commit_CommitsChanges()
        {
            var vm = new PageViewModel(CreateOnElements());
            vm.Edit();
            vm.PageRange = "34--37";
            vm.Commit();

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.PageRange, Is.EqualTo("34--37"));
            Assert.That(vm._page.PageRange, Is.EqualTo("34--37"));
        }

        [TestCase("1-2", 1)]
        [TestCase("13--16", 13)]
        [TestCase("128", 128)]
        [TestCase("aaaaa", int.MaxValue)]
        [TestCase("-13", int.MaxValue)]
        [TestCase("2147483648", int.MaxValue)]
        [TestCase("", int.MaxValue)]
        [TestCase(null, int.MaxValue)]
        public void FirstPage_ReturnsCorrectFirstPage(string pages, int expectedFirstPage)
        {
            var vm = new PageViewModel(new Page() { PageRange = pages });

            Assert.That(vm.FirstPage, Is.EqualTo(expectedFirstPage));
        }

        private static Page CreateOnElements()
        {
            return new Page()
            {
                Notes = "[Euclid's Elements] was the Basic Maths For Dummies of its day[...]",
                PageRange = "34",
                Title = "On Elements"
            };
        }
    }
}
