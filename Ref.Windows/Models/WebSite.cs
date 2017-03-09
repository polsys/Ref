using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a web site.
    /// </summary>
    [DataContract]
    internal class WebSite : ICatalogueEntry
    {
        /// <summary>
        /// The latest date this site was accessed.
        /// </summary>
        [DataMember]
        public DateTime AccessDate { get; set; }

        /// <summary>
        /// The author or authors of this site.
        /// </summary>
        [DataMember]
        public string Author { get; set; }

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
        /// The web address of this site.
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// The title of this site.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The year this site was published in.
        /// </summary>
        [DataMember]
        public string Year { get; set; }

        /// <summary>
        /// The list of notes.
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

        public WebSite()
        {
            // Default initialize the date
            AccessDate = DateTime.Today;
        }
    }
}
