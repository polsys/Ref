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
            Assert.That(vm.Pages, Is.EqualTo(page.Pages));
            Assert.That(vm.Title, Is.EqualTo(page.Title));
        }

        [Test]
        public void Cancel_DiscardsChanges()
        {
            var vm = new PageViewModel(CreateOnElements());
            vm.Edit();
            vm.Pages = "34--37";

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.Cancel(), "Pages");

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.Pages, Is.EqualTo("34"));
            Assert.That(vm._page.Pages, Is.EqualTo("34"));
        }

        [Test]
        public void Commit_CommitsChanges()
        {
            var vm = new PageViewModel(CreateOnElements());
            vm.Edit();
            vm.Pages = "34--37";
            vm.Commit();

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.Pages, Is.EqualTo("34--37"));
            Assert.That(vm._page.Pages, Is.EqualTo("34--37"));
        }

        private static Page CreateOnElements()
        {
            return new Page()
            {
                Notes = "[Euclid's Elements] was the Basic Maths For Dummies of its day[...]",
                Pages = "34",
                Title = "On Elements"
            };
        }
    }
}
