using System;
using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class WebSiteViewModelTests
    {
        [Test]
        public void Ctor_InitializesProperties()
        {
            var site = TestUtility.CreateMersenneWebSite();
            site.Pages.Add(new Page());
            var vm = new WebSiteViewModel(site);

            Assert.That(vm.AccessDate, Is.EqualTo(site.AccessDate));
            Assert.That(vm.Author, Is.EqualTo(site.Author));
            Assert.That(vm.Key, Is.EqualTo(site.Key));
            Assert.That(vm.Notes, Is.EqualTo(site.Notes));
            Assert.That(vm.Title, Is.EqualTo(site.Title));
            Assert.That(vm.Url, Is.EqualTo(site.Url));
            Assert.That(vm.Year, Is.EqualTo(site.Year));

            Assert.That(vm.IsReadOnly, Is.True);
            Assert.That(vm.Pages, Has.Exactly(1).Items);
        }

        [Test]
        public void PropertyChanged_RaisesEvent()
        {
            // This tests the generic property change handler in EntryViewModelBase

            var site = new WebSiteViewModel(new WebSite());
            site.Edit();

            TestUtility.AssertRaisesPropertyChanged(site,
                () => { site.AccessDate = DateTime.MaxValue; }, "AccessDate");
        }
    }
}
