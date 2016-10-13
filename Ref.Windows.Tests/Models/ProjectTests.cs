using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Polsys.Ref.Models;

namespace Polsys.Ref.Tests.Models
{
    class ProjectTests
    {
        [Test]
        public void Deserialize_InvalidStream()
        {
            using (var stream = new MemoryStream())
            {
                Assert.That(Project.Deserialize(stream), Is.Null);
            }
        }

        [Test]
        public void Deserialize_NewerVersionFails()
        {
            var testRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(testRoot, "TestFiles\\NewerVersion.refproject");

            using (var stream = File.OpenRead(filename))
            {
                Assert.That(Project.Deserialize(stream), Is.Null);
            }
        }

        [Test]
        public void Serialize_HandlesAllTypes()
        {
            // This catalogue should contain all possible types
            var catalogue = new Catalogue();
            var book = new Book();
            book.Pages.Add(new Page());
            catalogue.Entries.Add(book);

            using (var stream = new MemoryStream())
            {
                Assert.That(() => Project.Serialize(stream, catalogue), Throws.Nothing);
                Assert.That(stream.Position, Is.Not.Zero);
            }
        }
    }
}
