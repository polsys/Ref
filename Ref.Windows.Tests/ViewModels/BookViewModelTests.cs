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
            var book = new Book()
            {
                Author = "Beveridge, Colin",
                Key = "Beveridge2016",
                Publisher = "Octopus Books",
                Title = "Cracking Mathematics",
                Year = "2016"
            };

            var vm = new BookViewModel(book);

            Assert.That(vm.Author, Is.EqualTo(book.Author));
            Assert.That(vm.Key, Is.EqualTo(book.Key));
            Assert.That(vm.Publisher, Is.EqualTo(book.Publisher));
            Assert.That(vm.Title, Is.EqualTo(book.Title));
            Assert.That(vm.Year, Is.EqualTo(book.Year));
        }
    }
}
