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
    }
}
