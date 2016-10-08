using System;
using System.Collections.Generic;

namespace Ref.Windows.Models
{
    /// <summary>
    /// Contains all the entries and handles operations related to them.
    /// </summary>
    internal class Catalogue
    {
        /// <summary>
        /// Gets the list of entries in this collection.
        /// </summary>
        public IList<Book> Entries { get; }

        /// <summary>
        /// Constructs an empty Collection.
        /// </summary>
        public Catalogue()
        {
            Entries = new List<Book>();
        }
    }
}
