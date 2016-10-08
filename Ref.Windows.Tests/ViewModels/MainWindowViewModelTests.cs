using System;
using System.ComponentModel;
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

            AssertRaisesPropertyChanged(vm, () => vm.SelectEntry(entry), "SelectedEntry");
            Assert.That(vm.SelectedEntry, Is.SameAs(entry));
        }

        private void AssertRaisesPropertyChanged(MainWindowViewModel viewModel, TestDelegate func, string propertyName)
        {
            int propertyChangedCount = 0;
            PropertyChangedEventHandler handler = (object sender, PropertyChangedEventArgs e) =>
                   {
                       propertyChangedCount++;
                   };

            try
            {
                viewModel.PropertyChanged += handler;
                func.Invoke();
                Assert.That(propertyChangedCount, Is.EqualTo(1), "The PropertyChanged event was not fired or was fired more than once.");
            }
            finally
            {
                viewModel.PropertyChanged -= handler;
            }
        }
    }
}
