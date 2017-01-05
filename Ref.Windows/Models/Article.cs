using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a single article in a journal or a magazine.
    /// </summary>
    [DataContract]
    internal class Article : ICatalogueEntry
    {
        /// <summary>
        /// The author or authors of this article.
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// The Digital Object Identifier of this article.
        /// </summary>
        [DataMember]
        public string Doi { get; set; }

        /// <summary>
        /// The International Standard Serial Number of the journal this article was published in.
        /// </summary>
        [DataMember]
        public string Issn { get; set; }

        /// <summary>
        /// The publication this article was published in.
        /// </summary>
        [DataMember]
        public string Journal { get; set; }

        /// <summary>
        /// The unique citation key for this entry.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Additional notes for this entry.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// The number of the publication.
        /// This usually is the number within a yearly volume.
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// The pages this article appears in.
        /// </summary>
        [DataMember]
        public string PageRange { get; set; }

        /// <summary>
        /// The title of this article.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The volume of the publication.
        /// </summary>
        [DataMember]
        public string Volume { get; set; }

        /// <summary>
        /// The year this article was published in.
        /// </summary>
        [DataMember]
        public string Year { get; set; }

        /// <summary>
        /// The list of pages in this article.
        /// </summary>
        [DataMember]
        public List<Page> Pages
        {
            get
            {
                // This must be initialized here because the deserializer skips ctor
                if (_pages == null)
                    _pages = new List<Page>();
                return _pages;
            }
        }
        private List<Page> _pages;
    }
}
