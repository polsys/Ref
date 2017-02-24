﻿using System;
using System.IO;
using System.Text;
using Polsys.Ref.Models;

namespace Polsys.Ref.Export
{
    /// <summary>
    /// Exports the catalogue to a Word XML bibliography file.
    /// </summary>
    internal class WordExporter: CatalogueExporter
    {
        public override string FileExtension
        {
            get
            {
                return "xml";
            }
        }

        public override string Name
        {
            get
            {
                return "Microsoft Word sources";
            }
        }

        public override bool Export(Stream stream, Catalogue catalogue)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                // Start the XML document and the source collection
                writer.Write("<?xml version=\"1.0\"?>");
                writer.Write("<!-- Generated by Ref " + GetVersionString() + " at " + DateTime.Now.ToString() + ". -->");
                writer.Write("<b:Sources SelectedStyle=\"\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" " +
                    "xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\">");

                // Write each entry
                foreach (var entry in catalogue.Entries)
                {
                    WriteEntry(entry, writer);
                }

                // End the collection
                writer.Write("</b:Sources>");
            }

            return true;
        }

        internal void WriteEntry(ICatalogueEntry entry, TextWriter writer)
        {
            writer.Write("<b:Source>");
            WriteRandomGuid(writer);

            if (entry is Article)
                WriteArticle((Article)entry, writer);
            else if (entry is Book)
                WriteBook((Book)entry, writer);
            else if (entry is Thesis)
                WriteThesis((Thesis)entry, writer);

            writer.Write("</b:Source>");
        }

        internal void WriteArticle(Article article, TextWriter writer)
        {
            writer.Write("<b:SourceType>JournalArticle</b:SourceType>");

            WriteNonEmptyElement("b:Tag", article.Key, writer);
            WriteNonEmptyElement("b:DOI", article.Doi, writer);
            WriteNonEmptyElement("b:StandardNumber", article.Issn, writer);
            WriteNonEmptyElement("b:JournalName", article.Journal, writer);
            WriteNonEmptyElement("b:Issue", article.Number, writer);
            WriteNonEmptyElement("b:Pages", article.PageRange, writer);
            WriteNonEmptyElement("b:Title", article.Title, writer);
            WriteNonEmptyElement("b:Volume", article.Volume, writer);
            WriteNonEmptyElement("b:Year", article.Year, writer);
            
            if (!string.IsNullOrEmpty(article.Author))
            {
                writer.Write("<b:Author><b:Author>");
                WriteNameList(article.Author, writer);
                writer.Write("</b:Author></b:Author>");
            }
        }

        internal void WriteBook(Book book, TextWriter writer)
        {
            writer.Write("<b:SourceType>Book</b:SourceType>");

            WriteNonEmptyElement("b:Tag", book.Key, writer);
            WriteNonEmptyElement("b:City", book.Address, writer);
            WriteNonEmptyElement("b:Edition", book.Edition, writer);
            WriteNonEmptyElement("b:StandardNumber", book.Isbn, writer);
            WriteNonEmptyElement("b:Publisher", book.Publisher, writer);
            WriteNonEmptyElement("b:Title", book.Title, writer);
            WriteNonEmptyElement("b:Volume", book.Volume, writer);
            WriteNonEmptyElement("b:Year", book.Year, writer);

            // The first b:Author is the parent element for all kinds of people involved
            writer.Write("<b:Author>");
            if (!string.IsNullOrEmpty(book.Author))
            {
                writer.Write("<b:Author>");
                WriteNameList(book.Author, writer);
                writer.Write("</b:Author>");
            }
            if (!string.IsNullOrEmpty(book.Editor))
            {
                writer.Write("<b:Editor>");
                WriteNameList(book.Editor, writer);
                writer.Write("</b:Editor>");
            }
            if (!string.IsNullOrEmpty(book.Translator))
            {
                writer.Write("<b:Translator>");
                WriteNameList(book.Translator, writer);
                writer.Write("</b:Translator>");
            }
            writer.Write("</b:Author>");
        }

        internal void WriteThesis(Thesis thesis, TextWriter writer)
        {
            writer.Write("<b:SourceType>Report</b:SourceType>");
            switch (thesis.Kind)
            {
                case ThesisKind.Masters:
                    writer.Write("<b:ThesisType>Master's thesis</b:ThesisType>"); break;
                case ThesisKind.Licentiate:
                    writer.Write("<b:ThesisType>Licentiate thesis</b:ThesisType>"); break;
                default:
                    writer.Write("<b:ThesisType>PhD thesis</b:ThesisType>"); break;
            }

            WriteNonEmptyElement("b:Tag", thesis.Key, writer);
            WriteNonEmptyElement("b:DOI", thesis.Doi, writer);
            WriteNonEmptyElement("b:StandardNumber", thesis.Isbn, writer);
            WriteNonEmptyElement("b:Institution", thesis.School, writer);
            WriteNonEmptyElement("b:Title", thesis.Title, writer);
            WriteNonEmptyElement("b:Year", thesis.Year, writer);

            if (!string.IsNullOrEmpty(thesis.Author))
            {
                writer.Write("<b:Author><b:Author>");
                WriteNameList(thesis.Author, writer);
                writer.Write("</b:Author></b:Author>");
            }
        }

        internal static void WriteNameList(string authorString, TextWriter writer)
        {
            // TODO: Middle names are not used, but Word supports them.
            writer.Write("<b:NameList>");
            
            var names = authorString == null ? new string[0] : authorString.Split(';');
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                // Parse the name in either "Last, First", "First Last" or "Name" convention
                string lastName = string.Empty;
                string firstName = string.Empty;
                if (name.Contains(","))
                {
                    var splitIndex = name.IndexOf(',');
                    lastName = name.Substring(0, splitIndex).Trim();
                    firstName = name.Substring(splitIndex + 1, name.Length - splitIndex - 1).Trim();
                }
                else
                {
                    var trimmedName = name.Trim();
                    var splitIndex = trimmedName.LastIndexOf(' '); // Only consider the last token the last name
                    if (splitIndex == -1)
                    {
                        lastName = trimmedName;
                    }
                    else
                    {
                        firstName = trimmedName.Substring(0, splitIndex).Trim();
                        lastName = trimmedName.Substring(splitIndex + 1, trimmedName.Length - splitIndex - 1).Trim();
                    }
                }

                writer.Write("<b:Person>");
                if (!string.IsNullOrEmpty(lastName))
                {
                    writer.Write("<b:Last>");
                    writer.Write(lastName);
                    writer.Write("</b:Last>");
                }
                if (!string.IsNullOrEmpty(firstName))
                {
                    writer.Write("<b:First>");
                    writer.Write(firstName);
                    writer.Write("</b:First>");
                }
                writer.Write("</b:Person>");
            }

            writer.Write("</b:NameList>");
        }

        internal static void WriteNonEmptyElement(string name, string value, TextWriter writer)
        {
            if (string.IsNullOrEmpty(value))
                return;

            writer.Write("<" + name + ">");
            writer.Write(value);
            writer.Write("</" + name + ">");
        }

        internal static void WriteRandomGuid(TextWriter writer)
        {
            writer.Write("<b:Guid>{");
            var guid = Guid.NewGuid();
            writer.Write(guid.ToString().ToUpperInvariant());
            writer.Write("}</b:Guid>");
        }
    }
}