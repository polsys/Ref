﻿using System;
using System.Text;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The view model for the Copy Reference dialog.
    /// </summary>
    internal class CopyReferenceDialogViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the full citation text.
        /// </summary>
        public string Citation
        {
            get { return GenerateCitation(); }
        }

        /// <summary>
        /// Gets or sets the citation style.
        /// </summary>
        public CitationStyle CitationStyle
        {
            get { return _citationStyle; }
            set
            {
                if (_citationStyle != value)
                {
                    _citationStyle = value;
                    NotifyPropertyChanged(nameof(CitationStyle));
                    NotifyPropertyChanged(nameof(Citation));
                }
            }
        }
        private CitationStyle _citationStyle;

        /// <summary>
        /// Gets or sets the citation markup type.
        /// </summary>
        public ReferenceOutputType OutputType
        {
            get { return _outputType; }
            set
            {
                if (_outputType != value)
                {
                    _outputType = value;
                    NotifyPropertyChanged(nameof(OutputType));
                    NotifyPropertyChanged(nameof(Citation));
                }
            }
        }
        private ReferenceOutputType _outputType;

        private PublicationViewModelBase _entry;

        /// <summary>
        /// Creates a new view model instance with the specified entry.
        /// </summary>
        /// <param name="entry">The entry to display reference for.</param>
        public CopyReferenceDialogViewModel(PublicationViewModelBase entry)
        {
            _entry = entry;
        }

        private string GenerateCitation()
        {
            switch (CitationStyle)
            {
                case CitationStyle.Apa:
                    return GenerateApa();
                default:
                    throw new ArgumentException("Unknown citation style");

            }
        }

        private string GenerateApa()
        {
            // Already has some additional capacity
            var result = new StringBuilder(128);

            if (_entry is ArticleViewModel)
            {
                var article = _entry as ArticleViewModel;

                WriteApaAuthors(result, article.Author);
                result.Append(" (");
                result.Append(article.Year);
                result.Append("). ");
                result.Append(article.Title);
                result.Append(". ");
                result.Append(BeginItalics());
                result.Append(article.Journal);
                result.Append(EndItalics());
                result.Append(", ");
                result.Append(BeginItalics());
                result.Append(article.Volume);
                result.Append(EndItalics());
                result.Append("(");
                result.Append(article.Number);
                result.Append("), ");
                result.Append(article.PageRange);
                result.Append(".");

                if (!string.IsNullOrEmpty(article.Doi))
                {
                    result.Append(" doi:");
                    if (_outputType == ReferenceOutputType.Markdown)
                        result.Append(" "); // Some parsers seem to parse doi: as part of the link
                    result.Append(MakeLink(article.Doi, "https://dx.doi.org/" + article.Doi));
                }
            }
            else if (_entry is BookViewModel)
            {
                var book = _entry as BookViewModel;

                WriteApaAuthors(result, book.Author);
                result.Append(" (");
                result.Append(book.Year);
                result.Append("). ");
                result.Append(BeginItalics());
                result.Append(book.Title);
                result.Append(EndItalics());
                result.Append(". ");

                if (!string.IsNullOrEmpty(book.Address))
                {
                    result.Append(book.Address);
                    result.Append(": ");
                }

                result.Append(book.Publisher);
                result.Append(".");
            }

            return result.ToString();
        }

        private static void WriteApaAuthors(StringBuilder sb, string author)
        {
            // Authors are separated by commas and the final . is omitted if the forename is abbreviated.
            // Actually all forenames should be abbreviated, but don't try messing with that.
            sb.Append(author.Replace(';', ','));
            if (!author.EndsWith("."))
                sb.Append(".");
        }

        private string BeginItalics()
        {
            if (_outputType == ReferenceOutputType.Html)
                return "<i>";
            else if (_outputType == ReferenceOutputType.Markdown)
                return "*";
            else
                return "";
        }

        private string EndItalics()
        {
            if (_outputType == ReferenceOutputType.Html)
                return "</i>";
            else if (_outputType == ReferenceOutputType.Markdown)
                return "*";
            else
                return "";
        }

        private string MakeLink(string text, string target)
        {
            if (_outputType == ReferenceOutputType.Html)
                return "<a href=\"" + target + "\">" + text + "</a>";
            else if (_outputType == ReferenceOutputType.Markdown)
                return "[" + text + "](" + target + ")";
            else
                return text;
        }
    }

    internal enum CitationStyle
    {
        Apa,
        Chicago,
        Harvard,
        Ieee,
        Mla
    }

    internal enum ReferenceOutputType
    {
        Plaintext,
        Html,
        Markdown
    }
}