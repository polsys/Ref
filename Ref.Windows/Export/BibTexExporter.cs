﻿using System;
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
                    else if (entry is Thesis)
                        WriteThesis(writer, (Thesis)entry);
                    else if (entry is WebSite)
                        WriteWebSite(writer, (WebSite)entry);
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
            AddField(fields, EncloseInBraces(EscapeSpecialCharacters(article.Title)), "title");
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
            AddField(fields, EncloseInBraces(EscapeSpecialCharacters(book.Title)), "title");
            AddField(fields, book.Translator, "translator");
            AddField(fields, book.Volume, "volume");
            AddField(fields, book.Year, "year");

            // Write the entry
            WriteEntry(writer, "@book", book.Key, fields);
        }

        internal void WriteThesis(TextWriter writer, Thesis thesis)
        {
            if (IsUnkeyed(thesis.Key, thesis.Title, writer))
                return;

            var fields = new List<string>();
            AddField(fields, ReplaceSemicolonsWithAnd(thesis.Author), "author");
            AddField(fields, thesis.Doi, "doi");
            AddField(fields, thesis.Isbn, "isbn");

            // Licentiate thesis is a special case of PhD
            // This must be updated if more cases are added - there is no hard fail for an unknown type
            // TODO: Hardcoded string
            if (thesis.Kind == ThesisKind.Licentiate)
                AddField(fields, "Licentiate thesis", "type");

            AddField(fields, thesis.School, "school");
            AddField(fields, EncloseInBraces(EscapeSpecialCharacters(thesis.Title)), "title");
            AddField(fields, thesis.Year, "year");

            var entryType = thesis.Kind == ThesisKind.Masters ? "@mastersthesis" : "@phdthesis";
            WriteEntry(writer, entryType, thesis.Key, fields);
        }

        internal void WriteWebSite(TextWriter writer, WebSite site)
        {
            if (IsUnkeyed(site.Key, site.Title, writer))
                return;

            var fields = new List<string>();
            AddField(fields, ReplaceSemicolonsWithAnd(site.Author), "author");
            AddField(fields, EncloseInBraces(EscapeSpecialCharacters(site.Title)), "title");
            // BibLaTeX does not like escaped special characters, whereas BibTeX requires them.
            // Using the 'url' field instead of 'howpublished'.
            AddField(fields, EscapeSpecialCharacters(site.Url), "url");
            // BibLaTeX date format
            AddField(fields, site.AccessDate.ToString("yyyy'/'MM'/'dd"), "urldate");
            AddField(fields, site.Year, "year");

            // @electronic is supported by BibTeX and aliased to @online in BibLaTeX
            WriteEntry(writer, "@electronic", site.Key, fields);
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

        internal static string EscapeSpecialCharacters(string input)
        {
            // Escape LaTeX special characters # % & ^ _ ~
            // Do not escape the following characters:
            //   $  Inline math mode
            //   \  Required for commands and manually escaping $
            //   {} Required for commands, uncommon in text
            //   Anything within math mode
            //   Any character following \, since they are manually escaped

            if (input == null)
                return null;

            // This is quite inefficient, but not on the hottest path (I hope)
            bool inMathMode = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '$')
                {
                    inMathMode = !inMathMode;
                    continue;
                }
                else if (inMathMode || (i > 0 && input[i - 1] == '\\'))
                {
                    // Do not perform escapes within math mode or if the character is already escaped
                    continue;
                }

                if (input[i] == '#' || input[i] == '%' || input[i] == '&' || input[i] == '_')
                {
                    // Simply escape X with \X

                    input = input.Substring(0, i) + "\\" + input[i] + input.Substring(i + 1);
                    i++; // Skip the new character
                }
                else if (input[i] == '^' || input[i] == '~')
                {
                    // Escape accent with \X{}

                    input = input.Substring(0, i) + "\\" + input[i] + "{}" + input.Substring(i + 1);
                    i += 3; // Skip the new characters
                }
            }

            return input;
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
            var versionString = GetVersionString();
            writer.WriteLine("% Auto-generated file. Any modifications will be lost if the file is regenerated.");
            writer.WriteLine("% Generated by Ref " + versionString);
            writer.WriteLine("% " + DateTime.Now.ToString());
        }
    }
}
