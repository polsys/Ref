using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class ThesisViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var thesis = TestUtility.CreateShannonThesis();
            thesis.Pages.Add(new Page());
            var vm = new ThesisViewModel(thesis);
            
            Assert.That(vm.Author, Is.EqualTo(thesis.Author));
            Assert.That(vm.Doi, Is.EqualTo(thesis.Doi));
            Assert.That(vm.Isbn, Is.EqualTo(thesis.Isbn));
            Assert.That(vm.Key, Is.EqualTo(thesis.Key));
            Assert.That(vm.Kind, Is.EqualTo(thesis.Kind));
            Assert.That(vm.Notes, Is.EqualTo(thesis.Notes));
            Assert.That(vm.School, Is.EqualTo(thesis.School));
            Assert.That(vm.Title, Is.EqualTo(thesis.Title));
            Assert.That(vm.Year, Is.EqualTo(thesis.Year));

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.Pages, Has.Exactly(1).Items);
        }
    }
}
