using System;
using System.Text;
using Polsys.Ref.Models;
using System.Globalization;

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
                case CitationStyle.Chicago:
                    return GenerateChicago();
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

                WriteAuthors(result, article.Author);
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

                if (!string.IsNullOrEmpty(article.Number))
                {
                    result.Append("(");
                    result.Append(article.Number);
                    result.Append(")");
                }
                result.Append(", ");
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

                WriteAuthors(result, book.Author);
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
            else if (_entry is ThesisViewModel)
            {
                var thesis = _entry as ThesisViewModel;

                WriteAuthors(result, thesis.Author);
                result.Append(" (");
                result.Append(thesis.Year);
                result.Append("). ");
                result.Append(BeginItalics());
                result.Append(thesis.Title);
                result.Append(EndItalics());
                result.Append(" (");
                result.Append(GetThesisString(thesis.Kind));
                result.Append("). ");
                result.Append(thesis.School);
                result.Append(".");
            }
            else if (_entry is WebSiteViewModel)
            {
                var site = _entry as WebSiteViewModel;

                WriteAuthors(result, site.Author);
                if (!string.IsNullOrEmpty(site.Year))
                {
                    result.Append(" (");
                    result.Append(site.Year);
                    result.Append(").");
                }
                result.Append(" ");
                result.Append(BeginItalics());
                result.Append(site.Title);
                result.Append(EndItalics());
                // TODO: Localization
                result.Append(". Retrieved ");
                result.Append(site.AccessDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture));
                result.Append(", from ");
                result.Append(site.Url);
            }

            return result.ToString();
        }

        private string GenerateChicago()
        {
            var result = new StringBuilder(128);

            if (_entry is ArticleViewModel)
            {
                var article = _entry as ArticleViewModel;

                WriteAuthors(result, article.Author);
                result.Append(" \"");
                result.Append(article.Title);
                result.Append(".\" ");
                result.Append(BeginItalics());
                result.Append(article.Journal);
                result.Append(EndItalics());
                result.Append(" ");
                result.Append(article.Volume);

                if (!string.IsNullOrEmpty(article.Number))
                {
                    result.Append(", no. ");
                    result.Append(article.Number);
                    result.Append(" ");
                }
                else
                {
                    result.Append(" ");
                }

                result.Append("(");
                result.Append(article.Year);
                result.Append("): ");
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

                WriteAuthors(result, book.Author);
                result.Append(" ");
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
                result.Append(", ");
                result.Append(book.Year);
                result.Append(".");
            }
            else if (_entry is ThesisViewModel)
            {
                var thesis = _entry as ThesisViewModel;

                WriteAuthors(result, thesis.Author);
                result.Append(" \"");
                result.Append(thesis.Title);
                result.Append(".\" ");
                result.Append(GetThesisString(thesis.Kind));
                result.Append(", ");
                result.Append(thesis.School);
                result.Append(", ");
                result.Append(thesis.Year);
                result.Append(".");
            }
            else if (_entry is WebSiteViewModel)
            {
                var site = _entry as WebSiteViewModel;

                WriteAuthors(result, site.Author);
                result.Append(" ");
                if (!string.IsNullOrEmpty(site.Year))
                {
                    result.Append(site.Year);
                    result.Append(". ");
                }
                result.Append("\"");
                result.Append(site.Title);
                result.Append(".\"");
                // TODO: Localization
                result.Append(" Accessed ");
                result.Append(site.AccessDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture));
                result.Append(". ");
                result.Append(site.Url);
                result.Append(".");
            }

            return result.ToString();
        }

        private static void WriteAuthors(StringBuilder sb, string author)
        {
            // Authors are separated by commas and the final . is omitted if the forename is abbreviated.
            // Actually each style manipulates the names in some way, but don't try messing with that.
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

        private string GetThesisString(ThesisKind kind)
        {
            switch (kind)
            {
                case ThesisKind.Doctoral: return "PhD thesis";
                case ThesisKind.Licentiate: return "Licentiate thesis";
                case ThesisKind.Masters: return "Master's thesis";
                default:
                    return "Thesis"; // A somewhat sane way to fail
            }
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
