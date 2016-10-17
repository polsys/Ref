using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a single book.
    /// </summary>
    [DataContract]
    internal class Book : ICatalogueEntry
    {
        /// <summary>
        /// The address/city of the publisher of this book.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// The author or authors of this book.
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// The edition of this book.
        /// </summary>
        [DataMember]
        public string Edition { get; set; }

        /// <summary>
        /// The editor or editors of this book.
        /// </summary>
        [DataMember]
        public string Editor { get; set; }

        /// <summary>
        /// The unique citation key for this entry.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// The number of this book in a series.
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// The publisher of this book.
        /// </summary>
        [DataMember]
        public string Publisher { get; set; }

        /// <summary>
        /// The series this book is part of.
        /// </summary>
        [DataMember]
        public string Series { get; set; }

        /// <summary>
        /// The title of this book.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The volume of this book.
        /// </summary>
        [DataMember]
        public string Volume { get; set; }

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
}
