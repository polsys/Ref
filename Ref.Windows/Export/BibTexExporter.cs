using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Polsys.Ref.Models;

namespace Polsys.Ref.Export
{
    /// <summary>
    /// Exports the catalogue to a BibTeX (.bib) file.
    /// </summary>
    internal class BibTexExporter : CatalogueExporter
    {
        /// <summary>
        /// The default file extension used by this exporter.
        /// </summary>
        public override string FileExtension
        {
            get { return "bib"; }
        }

        /// <summary>
        /// The name of this exporter.
        /// </summary>
        public override string Name
        {
            get { return "BibTeX"; }
        }

        /// <summary>
        /// Exports the catalogue to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the catalogue to.</param>
        /// <param name="catalogue">The catalogue to export.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public override bool Export(Stream stream, Catalogue catalogue)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                WriteHeaderComment(writer);
                writer.WriteLine();

                foreach (var entry in catalogue.Entries)
                {
                    if (entry is Article)
                        WriteArticle(writer, (Article)entry);
                    else if (entry is Book)
                        WriteBook(writer, (Book)entry);
                    writer.WriteLine();
                }
            }

            return true;
        }

        internal void WriteArticle(TextWriter writer, Article article)
        {
            if (IsUnkeyed(article.Key, article.Title, writer))
                return;

            var fields = new List<string>();
            AddField(fields, ReplaceSemicolonsWithAnd(article.Author), "author");
            AddField(fields, article.Doi, "doi");
            AddField(fields, article.Issn, "issn");
            AddField(fields, article.Journal, "journal");
            AddField(fields, article.Number, "number");
            AddField(fields, article.PageRange, "pages");
            AddField(fields, EncloseInBraces(article.Title), "title");
            AddField(fields, article.Volume, "volume");
            AddField(fields, article.Year, "year");

            WriteEntry(writer, "@article", article.Key, fields);
        }

        internal void WriteBook(TextWriter writer, Book book)
        {
            if (IsUnkeyed(book.Key, book.Title, writer))
                return;

            // Only write the fields with a value
            var fields = new List<string>();
            AddField(fields, book.Address, "address");
            AddField(fields, ReplaceSemicolonsWithAnd(book.Author), "author");
            AddField(fields, book.Edition, "edition");
            AddField(fields, book.Editor, "editor");
            AddField(fields, book.Isbn, "isbn");
            AddField(fields, book.Number, "number");
            AddField(fields, book.Publisher, "publisher");
            AddField(fields, book.Series, "series");
            AddField(fields, EncloseInBraces(book.Title), "title");
            AddField(fields, book.Translator, "translator");
            AddField(fields, book.Volume, "volume");
            AddField(fields, book.Year, "year");

            // Write the entry
            WriteEntry(writer, "@book", book.Key, fields);
        }

        private static void AddField(List<string> fields, string value, string fieldName)
        {
            if (!string.IsNullOrEmpty(value))
                fields.Add(fieldName + " = \"" + value + "\"");
        }

        private static string EncloseInBraces(string value)
        {
            return "{" + value + "}";
        }

        private static bool IsUnkeyed(string key, string title, TextWriter writer)
        {
            if (string.IsNullOrEmpty(key))
            {
                writer.WriteLine("% No key defined for \"" + title + "\", skipping");
                return true;
            }
            return false;
        }

        private static string ReplaceSemicolonsWithAnd(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            else
                return value.Replace(";", " and ");
        }

        private static void WriteEntry(TextWriter writer, string entryType, string key, List<string> fields)
        {
            writer.WriteLine(entryType + "{" + key + ",");
            for (int i = 0; i < fields.Count; i++)
            {
                // Indentation included
                writer.Write("  " + fields[i]);

                // Trailing comma if not the last field
                if (i != fields.Count - 1)
                    writer.WriteLine(",");
                else
                    writer.WriteLine();
            }
            writer.WriteLine("}");
        }

        internal void WriteHeaderComment(TextWriter writer)
        {
            var versionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            writer.WriteLine("% Auto-generated file. Any modifications will be lost if the file is regenerated.");
            writer.WriteLine("% Generated by Ref " + versionString);
            writer.WriteLine("% " + DateTime.Now.ToString());
        }
    }
}
