namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a part of an entry, typically composed of one or more pages.
    /// </summary>
    internal class Page
    {
        /// <summary>
        /// Gets or sets the notes on this snippet.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the page range containing this snippet.
        /// </summary>
        public string Pages { get; set; }

        /// <summary>
        /// Gets or sets the title of this snippet.
        /// </summary>
        public string Title { get; set; }
    }
}
