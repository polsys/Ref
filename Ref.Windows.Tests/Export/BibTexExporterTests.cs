﻿using System.IO;
using NUnit.Framework;
using Polsys.Ref.Export;
using Polsys.Ref.Models;

namespace Polsys.Ref.Tests.Export
{
    class BibTexExporterTests
    {
        private string TempFolder;

        [OneTimeSetUp]
        public void Setup()
        {
            var testRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var folderName = Path.Combine(testRoot, Path.GetRandomFileName());
            TempFolder = Directory.CreateDirectory(folderName).FullName;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(TempFolder, true);
        }

        [Test]
        public void ExportFile_HandlesInvalidFileName()
        {
            var exporter = new BibTexExporter();
            Assert.That(exporter.Export("C:\\*.bib", new Catalogue()), Is.False);
        }

        [Test]
        public void ExportFile_HandlesOpenFile()
        {
            var filename = Path.Combine(TempFolder, "Export_HandlesOpenFile.bib");
            using (var file = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var exporter = new BibTexExporter();
                Assert.That(exporter.Export(filename, new Catalogue()), Is.False);
            }
        }

        [Test]
        public void ExportFile_WritesFile()
        {
            var filename = Path.Combine(TempFolder, "Export_WritesFile.bib");
            var exporter = new BibTexExporter();
            var catalogue = new Catalogue();
            catalogue.Entries.Add(TestUtility.CreateCrackingMathematics());
            Assert.That(exporter.Export(filename, catalogue), Is.True);

            // Simple verification of the file
            Assert.That(File.ReadAllText(filename), Does.Contain("@book{Beveridge2016"));
        }

        [Test]
        public void ExportStream_EmptyProjectHasOnlyComment()
        {
            using (var stream = new MemoryStream())
            {
                var exporter = new BibTexExporter();
                Assert.That(exporter.Export(stream, new Catalogue()), Is.True);
                
                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.Contain("% Generated by Ref"));
                    Assert.That(contents, Does.Not.Contain("{"));
                }
            }
        }

        [Test]
        public void ExportStream_TwoBooksWritten()
        {
            using (var stream = new MemoryStream())
            {
                var catalogue = new Catalogue();
                var book1 = TestUtility.CreateCrackingMathematics();
                var book2 = TestUtility.CreateMakeAndDo();
                book2.Year = "";
                catalogue.Entries.Add(book1);
                catalogue.Entries.Add(book2);

                var exporter = new BibTexExporter();
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.Contain("% Generated by Ref"));
                    Assert.That(contents, Does.Contain("@book{Beveridge2016,"));
                    Assert.That(contents, Does.Contain("year = \"2016\""));
                    Assert.That(contents, Does.Contain("@book{Parker2014,"));

                    // At least one empty line between the two books
                    var indexOfBookEnd = contents.IndexOf('}');
                    var indexOfBookStart = contents.LastIndexOf('@');
                    var space = contents.Substring(indexOfBookEnd + 1, indexOfBookStart - indexOfBookEnd - 1);
                    // Normalize newlines to \n
                    space = space.Replace("\r", "");
                    Assert.That(space, Is.EqualTo("\n\n"));
                }
            }
        }

        [Test]
        public void WriteBook_DoesNotWriteEmptyFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var book = TestUtility.CreateCrackingMathematics();
                book.Publisher = "";
                book.Volume = null;
                book.Year = null;
                exporter.WriteBook(writer, book);

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.Not.Contain("publisher"));
                Assert.That(contents, Does.Not.Contain("year"));
                Assert.That(contents, Does.StartWith("@book{Beveridge2016,"));
                Assert.That(contents, Does.Contain("author = \"Beveridge, Colin\","));
                Assert.That(contents, Does.Contain("title = \"Cracking Mathematics\""));
                // No trailing comma!
                Assert.That(contents, Does.Not.Contain("title = \"Cracking Mathematics\","));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }

        [Test]
        public void WriteBook_SkipsUnKeyed()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var book = TestUtility.CreateCrackingMathematics();
                book.Key = "";
                exporter.WriteBook(writer, book);
                
                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("% No key defined for \"Cracking Mathematics\", skipping"));
                Assert.That(contents, Does.Not.Contain("@book"));
            }
        }

        [Test]
        public void WriteBook_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                exporter.WriteBook(writer, TestUtility.CreateCrackingMathematics());

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@book{Beveridge2016,"));
                Assert.That(contents, Does.Contain("address = \"London\","));
                Assert.That(contents, Does.Contain("author = \"Beveridge, Colin\","));
                Assert.That(contents, Does.Contain("edition = \"1st\","));
                Assert.That(contents, Does.Contain("editor = \"Poulter, Pollyanna\","));
                Assert.That(contents, Does.Contain("number = \"1\","));
                Assert.That(contents, Does.Contain("publisher = \"Octopus Books\","));
                Assert.That(contents, Does.Contain("series = \"Musings of the Mathematical Ninja\","));
                Assert.That(contents, Does.Contain("title = \"Cracking Mathematics\","));
                Assert.That(contents, Does.Contain("volume = \"1\","));
                Assert.That(contents, Does.Contain("year = \"2016\""));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }
    }
}