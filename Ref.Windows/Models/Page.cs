using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a part of an entry, typically composed of one or more pages.
    /// </summary>
    [DataContract]
    internal class Page
    {
        /// <summary>
        /// Gets or sets the notes on this snippet.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the page range containing this snippet.
        /// </summary>
        [DataMember]
        public string PageRange { get; set; }

        /// <summary>
        /// Gets or sets the title of this snippet.
        /// </summary>
        [DataMember]
        public string Title { get; set; }
    }
}
