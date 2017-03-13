using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Polsys.Ref.Models;

namespace Polsys.Ref.Tests
{
    class FileCompatibilityTest
    {
        /*
         * The test files are each a superset of each other.
         * This means that:
         *   - each test method checks properties specific to that test file,
         *   - the methods then call each applicable Assert[...]Properties method
         *     to check the common values.
         * These tests are not intended to test every property.
         * The intention is to ensure Ref remaining backwards-compatible.
         */

        [Test]
        public void Project_010()
        {
            var catalogue = OpenCatalogue("Ref010.refproject");

            Assert.That(catalogue.Entries, Has.Count.EqualTo(2));
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Article>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Book>());

            Assert010Properties(catalogue);
        }

        [Test]
        public void Project_020()
        {
            var catalogue = OpenCatalogue("Ref020.refproject");

            Assert.That(catalogue.Entries, Has.Count.EqualTo(4));
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Article>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Book>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<Thesis>());
            Assert.That(catalogue.Entries, Has.Exactly(1).InstanceOf<WebSite>());

            Assert010Properties(catalogue);
            Assert020Properties(catalogue);
        }

        private void Assert010Properties(Catalogue catalogue)
        {
            var book = FirstInstanceOf<Book>(catalogue.Entries);
            Assert.That(book.Title, Is.EqualTo("Book Title"));
            Assert.That(book.Author, Is.EqualTo("Lastname, Firstname"));
            Assert.That(book.Translator, Is.EqualTo("Sukunimi, Etunimi"));
            Assert.That(book.Pages, Has.Exactly(1).InstanceOf<Page>());
            Assert.That(book.Pages[0].Title, Is.EqualTo("A note by me"));
            Assert.That(book.Pages[0].Notes, Is.EqualTo("This note was written by me."));

            var article = FirstInstanceOf<Article>(catalogue.Entries);
            Assert.That(article.Title, Is.EqualTo("Article Title"));
            Assert.That(article.Author, Is.EqualTo("Lastname, Firstname B."));
            Assert.That(article.Journal, Is.EqualTo("Bulletin of Example Stuff"));
        }

        private void Assert020Properties(Catalogue catalogue)
        {
            var thesis = FirstInstanceOf<Thesis>(catalogue.Entries);
            Assert.That(thesis.Title, Is.EqualTo("Thesis Title"));
            Assert.That(thesis.School, Is.EqualTo("University of Neverland"));
            Assert.That(thesis.Kind, Is.EqualTo(ThesisKind.Masters));

            var site = FirstInstanceOf<WebSite>(catalogue.Entries);
            Assert.That(site.Title, Is.EqualTo("Web Site"));
            Assert.That(site.Author, Is.EqualTo("Nom, Prénom"));
            Assert.That(site.Url, Is.EqualTo("www.example.com"));
            // TODO: Does this fail somehow thanks to different time zones?
            // Using ticks to avoid that... maybe.
            Assert.That(site.AccessDate, Is.EqualTo(new DateTime(636247872000000000)));
        }

        private T FirstInstanceOf<T>(List<ICatalogueEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry is T)
                    return (T)entry;
            }

            throw new KeyNotFoundException($"No instance of {typeof(T)} found.");
        }

        private Catalogue OpenCatalogue(string filename)
        {
            var testRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(testRoot, "TestFiles", filename);

            using (var file = new FileStream(path, FileMode.Open))
            {
                return Project.Deserialize(file);
            }
        }
    }
}
