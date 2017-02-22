using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Contains all the entries and handles operations related to them.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Article))]
    [KnownType(typeof(Book))]
    [KnownType(typeof(Thesis))]
    internal class Catalogue
    {
        /// <summary>
        /// Gets the list of entries in this collection.
        /// </summary>
        [DataMember]
        public List<ICatalogueEntry> Entries
        {
            get
            {
                // This must be initialized here because the deserializer skips ctor
                if (_entries == null)
                    _entries = new List<ICatalogueEntry>();
                return _entries;
            }
        }
        private List<ICatalogueEntry> _entries;
    }
}
