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

        // LaTeX special characters that should be escaped in text mode
        [TestCase("example.com/~me", "example.com/\\~{}me")]
        [TestCase("Test#case", "Test\\#case")]
        [TestCase("Test&case", "Test\\&case")]
        [TestCase("Test_case", "Test\\_case")]
        [TestCase("Test^case", "Test\\^{}case")]
        [TestCase("100 % right", "100 \\% right")]
        // Do not escape {} or backslash, their user probably knows what they're doing
        [TestCase("{Test}", "{Test}")]
        [TestCase("\\LaTeX", "\\LaTeX")]
        // Do not escape manually escaped characters
        [TestCase("Some \\_ text", "Some \\_ text")]
        // In math mode, do not escape characters
        [TestCase("$A_n$", "$A_n$")]
        [TestCase("A_n $B^n$ C%", "A\\_n $B^n$ C\\%")]
        public void EscapeSpecialCharacters(string input, string expected)
        {
            Assert.That(BibTexExporter.EscapeSpecialCharacters(input), Is.EqualTo(expected));
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
        public void ExportStream_ArticleWritten()
        {
            using (var stream = new MemoryStream())
            {
                var exporter = new BibTexExporter();
                var catalogue = new Catalogue();
                catalogue.Entries.Add(TestUtility.CreateCounterexample());
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.Contain("% Generated by Ref"));
                    Assert.That(contents, Does.Contain("@article{Lander1966"));
                }
            }
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
        public void ExportStream_ThesisWritten()
        {
            using (var stream = new MemoryStream())
            {
                var exporter = new BibTexExporter();
                var catalogue = new Catalogue();
                catalogue.Entries.Add(TestUtility.CreateShannonThesis());
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.Contain("% Generated by Ref"));
                    Assert.That(contents, Does.Contain("@mastersthesis{Shannon1937"));
                }
            }
        }

        [Test]
        public void ExportStream_WebSiteWritten()
        {
            using (var stream = new MemoryStream())
            {
                var exporter = new BibTexExporter();
                var catalogue = new Catalogue();
                catalogue.Entries.Add(TestUtility.CreateMersenneWebSite());
                Assert.That(exporter.Export(stream, catalogue), Is.True);

                // Verify the contents
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    Assert.That(contents, Does.Contain("% Generated by Ref"));
                    Assert.That(contents, Does.Contain("@electronic{Gimps"));
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
                    Assert.That(contents, Does.Contain("\n@book{Parker2014,"));
                }
            }
        }

        [Test]
        public void WriteArticle_DoesNotWriteEmptyFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var article = new Article()
                {
                    Key = "Ref2016",
                    Title = "On Ref"
                };
                exporter.WriteArticle(writer, article);

                // Verify the contents - remove all whitespace
                var contents = writer.ToString().Replace(" ", "").Replace("\n", "").Replace("\r", "");
                Assert.That(contents, Is.EqualTo("@article{Ref2016,title=\"{OnRef}\"}"));
            }
        }

        [Test]
        public void WriteArticle_EscapesSpecialCharacters()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var article = new Article()
                {
                    Key = "key",
                    Title = "Special_Char"
                };
                exporter.WriteArticle(writer, article);
                
                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("\\_"));
            }
        }

        [Test]
        public void WriteArticle_ReplacesSemicolonsInAuthors()
        {
            // Since it is just a naïve replacement, the double space is expected
            var original = "Boyer, Carl; Merzbach, Uta;Pietiläinen, Kimmo";
            var expected = "Boyer, Carl and  Merzbach, Uta and Pietiläinen, Kimmo";

            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var article = new Article() { Key = "Boyer", Author = original };
                exporter.WriteArticle(writer, article);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain(expected));
            }
        }

        [Test]
        public void WriteArticle_SkipsUnKeyed()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var article = new Article() { Title = "On Ref" };
                exporter.WriteArticle(writer, article);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("% No key defined for \"On Ref\", skipping"));
                Assert.That(contents, Does.Not.Contain("@article"));
            }
        }

        [Test]
        public void WriteArticle_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                exporter.WriteArticle(writer, TestUtility.CreateCounterexample());

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@article{Lander1966,"));
                Assert.That(contents, Does.Contain("author = \"Lander, L.J. and  Parkin, T.R.\","));
                Assert.That(contents, Does.Contain("doi = \"10.1090/S0002-9904-1966-11654-3\","));
                Assert.That(contents, Does.Contain("issn = \"0273-0979\","));
                Assert.That(contents, Does.Contain("journal = \"Bull. Amer. Math. Soc.\","));
                Assert.That(contents, Does.Contain("number = \"6\","));
                Assert.That(contents, Does.Contain("pages = \"1079\","));
                Assert.That(contents, Does.Contain("title = \"{Counterexample to Euler's conjecture on sums of like powers}\","));
                Assert.That(contents, Does.Contain("volume = \"72\","));
                Assert.That(contents, Does.Contain("year = \"1966\""));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
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
                book.Translator = null;
                book.Volume = null;
                book.Year = null;
                exporter.WriteBook(writer, book);

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.Not.Contain("publisher"));
                Assert.That(contents, Does.Not.Contain("year"));
                Assert.That(contents, Does.StartWith("@book{Beveridge2016,"));
                Assert.That(contents, Does.Contain("author = \"Beveridge, Colin\","));
                Assert.That(contents, Does.Contain("title = \"{Cracking Mathematics}\""));
                // No trailing comma!
                Assert.That(contents, Does.Not.Contain("title = \"{Cracking Mathematics}\","));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }

        [Test]
        public void WriteBook_EscapesSpecialCharacters()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var book = new Book()
                {
                    Key = "key",
                    Title = "Special_Char"
                };
                exporter.WriteBook(writer, book);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("\\_"));
            }
        }

        [Test]
        public void WriteBook_ReplacesSemicolonsInAuthors()
        {
            // Since it is just a naïve replacement, the double space is expected
            var original = "Boyer, Carl; Merzbach, Uta;Pietiläinen, Kimmo";
            var expected = "Boyer, Carl and  Merzbach, Uta and Pietiläinen, Kimmo";

            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var book = new Book() { Key = "Boyer", Author = original };
                exporter.WriteBook(writer, book);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain(expected));
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
                Assert.That(contents, Does.Contain("isbn = \"978-1-84403-862-6\","));
                Assert.That(contents, Does.Contain("number = \"1\","));
                Assert.That(contents, Does.Contain("publisher = \"Octopus Books\","));
                Assert.That(contents, Does.Contain("series = \"Musings of the Mathematical Ninja\","));
                Assert.That(contents, Does.Contain("title = \"{Cracking Mathematics}\","));
                Assert.That(contents, Does.Contain("translator = \"Ninja, Mathematical\","));
                Assert.That(contents, Does.Contain("volume = \"1\","));
                Assert.That(contents, Does.Contain("year = \"2016\""));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }

        [Test]
        public void WriteThesis_CompleteMasters()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                exporter.WriteThesis(writer, TestUtility.CreateShannonThesis());

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@mastersthesis{Shannon1937,"));
                Assert.That(contents, Does.Contain("author = \"Shannon, Claude\","));
                Assert.That(contents, Does.Contain("doi = \"1721.1/11173\","));
                Assert.That(contents, Does.Contain("isbn = \"111111111X\","));
                Assert.That(contents, Does.Contain("school = \"Massachusetts Institute of Technology\","));
                Assert.That(contents, Does.Contain("title = \"{A symbolic analysis of relay and switching circuits}\","));
                Assert.That(contents, Does.Contain("year = \"1937\""));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }

        [Test]
        public void WriteThesis_EscapesSpecialCharacters()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var thesis = new Thesis()
                {
                    Key = "key",
                    Title = "Special_Char"
                };
                exporter.WriteThesis(writer, thesis);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("\\_"));
            }
        }

        [Test]
        public void WriteThesis_Licentiate()
        {
            // There is no BibTeX type for a licentiate thesis, so it is a special case of PhD.
            // They aren't written anymore in Europe, but of course the past works haven't gone extinct.

            using (var writer = new StringWriter())
            {
                var thesis = new Thesis()
                {
                    Author = "Shannon, Claude",
                    Key = "ShannonX",
                    Kind = ThesisKind.Licentiate,
                    Title = "A thesis that never was"
                };

                var exporter = new BibTexExporter();
                exporter.WriteThesis(writer, thesis);

                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@phdthesis{ShannonX,"));
                Assert.That(contents, Does.Contain("type = \"Licentiate thesis\""));
            }
        }

        [Test]
        public void WriteThesis_MinimalDoctoral()
        {
            using (var writer = new StringWriter())
            {
                // Actually this is missing some required BibTeX fields
                var thesis = new Thesis()
                {
                    Author = "Shannon, Claude",
                    Key = "Shannon1940",
                    Kind = ThesisKind.Doctoral,
                    Title = "An algebra for theoretical genetics"
                };

                var exporter = new BibTexExporter();
                exporter.WriteThesis(writer, thesis);

                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@phdthesis{Shannon1940,"));
            }
        }

        [Test]
        public void WriteWebSite_WritesAllFields()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                exporter.WriteWebSite(writer, TestUtility.CreateMersenneWebSite());

                // Verify the contents
                var contents = writer.ToString();
                Assert.That(contents, Does.StartWith("@electronic{Gimps,"));
                Assert.That(contents, Does.Contain("author = \"GIMPS\","));
                Assert.That(contents, Does.Contain("title = \"{Great Internet Mersenne Prime Search}\","));
                Assert.That(contents, Does.Contain("url = \"https://www.mersenne.org/\","));
                Assert.That(contents, Does.Contain("urldate = \"2017/02/27\","));
                Assert.That(contents, Does.Contain("year = \"2017\""));
                Assert.That(contents.TrimEnd(), Does.EndWith("}"));
            }
        }

        [Test]
        public void WriteWebSite_EscapesSpecialCharacters()
        {
            using (var writer = new StringWriter())
            {
                var exporter = new BibTexExporter();
                var site = new WebSite()
                {
                    Key = "key",
                    Title = "Special_Char",
                    Url = "example.com/~me"
                };
                exporter.WriteWebSite(writer, site);

                var contents = writer.ToString();
                Assert.That(contents, Does.Contain("\\_"));
                Assert.That(contents, Does.Contain("\\~{}"));
            }
        }
    }
}
