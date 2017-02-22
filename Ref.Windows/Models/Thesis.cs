using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a thesis of any kind.
    /// </summary>
    [DataContract]
    internal class Thesis : ICatalogueEntry
    {
        /// <summary>
        /// The author or authors of this thesis.
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// The Digital Object Identifier of this thesis.
        /// </summary>
        [DataMember]
        public string Doi { get; set; }

        /// <summary>
        /// The International Standard Book Number of this thesis.
        /// </summary>
        [DataMember]
        public string Isbn { get; set; }

        /// <summary>
        /// The unique citation key for this entry.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// The type of this thesis.
        /// </summary>
        [DataMember]
        public ThesisKind Kind { get; set; }

        /// <summary>
        /// Additional notes for this entry.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// The school this thesis was published in.
        /// </summary>
        [DataMember]
        public string School { get; set; }

        /// <summary>
        /// The title of this thesis.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The year this book was published in.
        /// </summary>
        [DataMember]
        public string Year { get; set; }

        /// <summary>
        /// The list of pages in this book.
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

    /// <summary>
    /// Represents the recognized thesis types.
    /// </summary>
    public enum ThesisKind
    {
        Doctoral,
        Licentiate,
        Masters
    }
}
