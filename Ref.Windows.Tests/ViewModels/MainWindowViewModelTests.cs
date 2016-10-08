using NUnit.Framework;
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
        public void SelectEntry_SelectsAndRaisesEvent()
        {
            var vm = new MainWindowViewModel();
            var entry = new BookViewModel(new Models.Book() { Title = "Test 1" });
            vm.Catalogue.Entries.Add(entry);

            TestUtility.AssertRaisesPropertyChanged(vm, () => vm.SelectEntry(entry), "SelectedEntry");
            Assert.That(vm.SelectedEntry, Is.SameAs(entry));
        }
    }
}
