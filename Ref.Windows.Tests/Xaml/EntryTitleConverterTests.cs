using System;
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;
using Polsys.Ref.Xaml;

namespace Polsys.Ref.Tests.Xaml
{
    class EntryTitleConverterTests
    {
        [Test]
        public void Convert_ReturnsErrorWhenUnknownType()
        {
            var converter = new EntryTitleConverter();

            Assert.That(converter.Convert(new Catalogue(), typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("???"));
        }

        [Test]
        public void Convert_ReturnsErrorWhenNull()
        {
            var converter = new EntryTitleConverter();

            Assert.That(converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("???"));
        }

        [Test]
        public void Convert_ReturnsPlainBookName()
        {
            var converter = new EntryTitleConverter();
            var book = new BookViewModel(new Book() { Title = "Book Title" });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("Book Title"));
        }

        [Test]
        public void Convert_ReturnsBookNameWithVolume()
        {
            var converter = new EntryTitleConverter();
            var book = new BookViewModel(new Book() { Title = "Book Title", Volume = "2" });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("Book Title (2)"));
        }

        [Test]
        public void Convert_ReturnsPlainArticleName()
        {
            var converter = new EntryTitleConverter();
            var book = new ArticleViewModel(new Article() { Title = "Article Title" });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("Article Title"));
        }

        [Test]
        public void Convert_ReturnsPlainPageName()
        {

            var converter = new EntryTitleConverter();
            var book = new PageViewModel(new Page() { Title = "Page Title" });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("Page Title"));
        }

        [Test]
        public void Convert_ReturnsUntitled_Empty()
        {
            var converter = new EntryTitleConverter();
            var book = new ArticleViewModel(new Article() { Title = "" });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("(Untitled)"));
        }

        [Test]
        public void Convert_ReturnsUntitled_Null()
        {
            var converter = new EntryTitleConverter();
            var book = new ArticleViewModel(new Article() { Title = null });

            Assert.That(converter.Convert(book, typeof(string), null, CultureInfo.InvariantCulture),
                Is.EqualTo("(Untitled)"));
        }
    }
}
