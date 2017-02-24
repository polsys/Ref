﻿using System.IO;
using NUnit.Framework;
using Polsys.Ref.Export;
using Polsys.Ref.Models;

namespace Polsys.Ref.Tests.Export
{
    class WordExporterTests
    {
        [Test]
        public void ExportStream_Empty()
        {
            var expectedStart = "<?xml version=\"1.0\"?>";
            var expectedEnd = "<b:Sources SelectedStyle=\"\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" "+
                "xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\"></b:Sources>";

            using (var stream = new MemoryStream())
            {
                var exporter = new WordExporter();
                var catalogue = new Catalogue();
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.StartWith(expectedStart));
                    Assert.That(contents, Does.Contain("<!-- Generated by Ref"));
                    Assert.That(contents, Does.Contain("-->"));
                    Assert.That(contents, Does.EndWith(expectedEnd));
                }
            }
        }

        [Test]
        public void ExportStream_WritesEntries()
        {
            using (var stream = new MemoryStream())
            {
                var exporter = new WordExporter();
                var catalogue = new Catalogue();
                catalogue.Entries.Add(TestUtility.CreateCounterexample());
                catalogue.Entries.Add(TestUtility.CreateCrackingMathematics());
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.StartWith("<?xml version=\"1.0\"?>"));
                    Assert.That(contents, Does.Contain("<!-- Generated by Ref"));
                    Assert.That(contents, Does.Contain("<b:SourceType>JournalArticle</b:SourceType>"));
                    Assert.That(contents, Does.Contain("<b:SourceType>Book</b:SourceType>"));
                }
            }
        }

        [Test]
        public void WriteEntry_Article()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteEntry(new Article(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.StartWith("<b:Source>"));
                Assert.That(result, Does.Contain("<b:Guid>"));
                Assert.That(result, Does.Contain("<b:SourceType>JournalArticle</b:SourceType>"));
                Assert.That(result, Does.EndWith("</b:Source>"));
            }
        }

        [Test]
        public void WriteEntry_Book()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteEntry(new Book(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.StartWith("<b:Source>"));
                Assert.That(result, Does.Contain("<b:Guid>"));
                Assert.That(result, Does.Contain("<b:SourceType>Book</b:SourceType>"));
                Assert.That(result, Does.EndWith("</b:Source>"));
            }
        }

        [Test]
        public void WriteEntry_Thesis()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteEntry(new Thesis(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.StartWith("<b:Source>"));
                Assert.That(result, Does.Contain("<b:Guid>"));
                Assert.That(result, Does.Contain("<b:SourceType>Report</b:SourceType>"));
                Assert.That(result, Does.EndWith("</b:Source>"));
            }
        }

        [Test]
        public void WriteArticle_EmptyArticle()
        {
            // Word does just fine without a tag
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteArticle(new Article(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:SourceType>JournalArticle</b:SourceType>"));
                Assert.That(result, Does.Not.Contain("<b:Tag>"));
                Assert.That(result, Does.Not.Contain("<b:Author>"));
            }
        }

        [Test]
        public void WriteArticle_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteArticle(TestUtility.CreateCounterexample(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:Tag>Lander1966</b:Tag>"));
                Assert.That(result, Does.Contain("<b:SourceType>JournalArticle</b:SourceType>"));
                Assert.That(result, Does.Contain("<b:Title>Counterexample to Euler's conjecture on sums of like powers</b:Title>"));
                Assert.That(result, Does.Contain("<b:Year>1966</b:Year>"));
                Assert.That(result, Does.Contain("<b:JournalName>Bull. Amer. Math. Soc.</b:JournalName>"));
                Assert.That(result, Does.Contain("<b:Pages>1079</b:Pages>"));
                Assert.That(result, Does.Contain("<b:Volume>72</b:Volume>"));
                Assert.That(result, Does.Contain("<b:Issue>6</b:Issue>"));
                Assert.That(result, Does.Contain("<b:Author><b:Author>"));
                Assert.That(result, Does.Contain("<b:NameList><b:Person><b:Last>Lander</b:Last><b:First>L.J.</b:First></b:Person>"));
                Assert.That(result, Does.Contain("<b:Person><b:Last>Parkin</b:Last><b:First>T.R.</b:First></b:Person></b:NameList>"));
                Assert.That(result, Does.Contain("</b:Author></b:Author>"));
                Assert.That(result, Does.Contain("<b:StandardNumber>0273-0979</b:StandardNumber>"));
                Assert.That(result, Does.Contain("<b:DOI>10.1090/S0002-9904-1966-11654-3</b:DOI>"));
            }
        }

        [Test]
        public void WriteBook_EmptyBook()
        {
            // Word does just fine without a tag
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteBook(new Book(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:SourceType>Book</b:SourceType>"));
                Assert.That(result, Does.Not.Contain("<b:Tag>"));

                // Unlike an article, a book always has the parent Author tag.
                // However, its children Author, Editor, Translator might exist or not
                Assert.That(result, Does.Contain("<b:Author>"));
                Assert.That(result, Does.Contain("</b:Author>"));
            }
        }

        [Test]
        public void WriteBook_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteBook(TestUtility.CreateCrackingMathematics(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:Tag>Beveridge2016</b:Tag>"));
                Assert.That(result, Does.Contain("<b:SourceType>Book</b:SourceType>"));
                Assert.That(result, Does.Contain("<b:Title>Cracking Mathematics</b:Title>"));
                Assert.That(result, Does.Contain("<b:Year>2016</b:Year>"));
                Assert.That(result, Does.Contain("<b:Publisher>Octopus Books</b:Publisher>"));
                Assert.That(result, Does.Contain("<b:City>London</b:City>"));
                Assert.That(result, Does.Contain("<b:Author><b:NameList><b:Person><b:Last>Beveridge</b:Last><b:First>Colin</b:First></b:Person></b:NameList></b:Author>"));
                Assert.That(result, Does.Contain("<b:Editor><b:NameList><b:Person><b:Last>Poulter</b:Last><b:First>Pollyanna</b:First></b:Person></b:NameList></b:Editor>"));
                Assert.That(result, Does.Contain("<b:Translator><b:NameList><b:Person><b:Last>Ninja</b:Last><b:First>Mathematical</b:First></b:Person></b:NameList></b:Translator>"));
                Assert.That(result, Does.Contain("<b:Volume>1</b:Volume>"));
                Assert.That(result, Does.Contain("<b:StandardNumber>978-1-84403-862-6</b:StandardNumber>"));
                Assert.That(result, Does.Contain("<b:Edition>1st</b:Edition>"));
            }
        }

        [TestCase(ThesisKind.Doctoral, "PhD thesis")]
        [TestCase(ThesisKind.Licentiate, "Licentiate thesis")]
        [TestCase(ThesisKind.Masters, "Master's thesis")]
        public void WriteThesis_EmptyThesis(ThesisKind thesisKind, string thesisString)
        {
            // Word does not have a separate type for thesis - it is a kind of report
            // Verify here that all the thesis kinds are handled correctly

            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteThesis(new Thesis() { Kind = thesisKind }, writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:SourceType>Report</b:SourceType>"));
                Assert.That(result, Does.Contain("<b:ThesisType>" + thesisString + "</b:ThesisType>"));
                Assert.That(result, Does.Not.Contain("<b:Tag>"));
            }
        }

        [Test]
        public void WriteThesis_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new WordExporter();
                exporter.WriteThesis(TestUtility.CreateShannonThesis(), writer);

                var result = writer.ToString();
                Assert.That(result, Does.Contain("<b:Tag>Shannon1937</b:Tag>"));
                Assert.That(result, Does.Contain("<b:SourceType>Report</b:SourceType>"));
                Assert.That(result, Does.Contain("<b:Title>A symbolic analysis of relay and switching circuits</b:Title>"));
                Assert.That(result, Does.Contain("<b:Year>1937</b:Year>"));
                Assert.That(result, Does.Contain("<b:ThesisType>Master's thesis</b:ThesisType>"));
                Assert.That(result, Does.Contain("<b:Author><b:Author><b:NameList><b:Person><b:Last>Shannon</b:Last><b:First>Claude</b:First></b:Person></b:NameList></b:Author></b:Author>"));
                Assert.That(result, Does.Contain("<b:Institution>Massachusetts Institute of Technology</b:Institution>"));
                Assert.That(result, Does.Contain("<b:StandardNumber>111111111X</b:StandardNumber>"));
                Assert.That(result, Does.Contain("<b:DOI>1721.1/11173</b:DOI>"));
            }
        }

        [Test]
        public void WriteNameList_Empty()
        {
            // Technically there shouldn't be empty name lists, but handle the case anyway
            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("", writer);
                WordExporter.WriteNameList(null, writer);

                Assert.That(writer.ToString(), Is.EqualTo("<b:NameList></b:NameList><b:NameList></b:NameList>"));
            }
        }
        
        [Test]
        public void WriteNameList_SingleFirstLast()
        {
            var expected = "<b:NameList><b:Person><b:Last>Gardner</b:Last><b:First>Martin</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("Martin Gardner", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_SingleLastFirst()
        {
            var expected = "<b:NameList><b:Person><b:Last>Gardner</b:Last><b:First>Martin</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("Gardner, Martin", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_MultipleFirstLast()
        {
            var expected = "<b:NameList><b:Person><b:Last>Gardner</b:Last><b:First>Martin</b:First></b:Person>" + 
                "<b:Person><b:Last>Grime</b:Last><b:First>James</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("Martin Gardner; James Grime", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_MultipleLastFirst()
        {
            var expected = "<b:NameList><b:Person><b:Last>Gardner</b:Last><b:First>Martin</b:First></b:Person>" +
                "<b:Person><b:Last>Grime</b:Last><b:First>James</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("Gardner, Martin; Grime, James", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_SinglePartNames()
        {
            var expected = "<b:NameList><b:Person><b:Last>Eratosthenes</b:Last></b:Person><b:Person><b:Last>Euclid</b:Last></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("Eratosthenes; Euclid", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_ManyPartName()
        {
            var expected = "<b:NameList><b:Person><b:Last>von Neumann</b:Last><b:First>John</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("von Neumann, John", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_ManyPartName_BadCase()
        {
            // Use the BibTeX behavior where only the last token is considered the last name.
            // Don't bother mucking around with middle names
            var expected = "<b:NameList><b:Person><b:Last>Neumann</b:Last><b:First>John von</b:First></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList("John von Neumann", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNameList_MalformedName()
        {
            var expected = "<b:NameList><b:Person></b:Person></b:NameList>";

            using (var writer = new StringWriter())
            {
                WordExporter.WriteNameList(",", writer);

                Assert.That(writer.ToString(), Is.EqualTo(expected));
            }
        }

        [Test]
        public void WriteNonEmptyElement_DoesNotWriteEmpty()
        {
            using (var writer = new StringWriter())
            {
                WordExporter.WriteNonEmptyElement("EmptyElement", "", writer);
                WordExporter.WriteNonEmptyElement("NullElement", null, writer);

                Assert.That(writer.ToString(), Is.Empty);
            }
        }

        [Test]
        public void WriteNonEmptyElement_WritesString()
        {
            using (var writer = new StringWriter())
            {
                WordExporter.WriteNonEmptyElement("Name", "Value", writer);

                Assert.That(writer.ToString(), Is.EqualTo("<Name>Value</Name>"));
            }
        }

        [Test]
        public void WriteRandomGuid()
        {
            using (var writer = new StringWriter())
            {
                WordExporter.WriteRandomGuid(writer);

                var result = writer.ToString();
                Assert.That(result, Does.StartWith("<b:Guid>{"));
                Assert.That(result, Does.EndWith("}</b:Guid>"));
                Assert.That(result, Has.Length.EqualTo(55));
                // Word GUIDs are uppercase
                Assert.That(result.Substring(8, 38), Is.EqualTo(result.Substring(8,38).ToUpper()));
            }
        }
    }
}
