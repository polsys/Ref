namespace Polsys.Ref.Models
{
    /// <summary>
    /// Interface for all catalogue entries, such as <see cref="Book"/>, but not subentries like <see cref="Page"/>.
    /// The properties are common to all entry types.
    /// </summary>
    internal interface ICatalogueEntry
    {
        /// <summary>
        /// The author or authors of this entry.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// The unique citation key for this entry.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Additional notes for this entry.
        /// </summary>
        string Notes { get; set; }

        /// <summary>
        /// The title of this entry.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The year this entry was published in.
        /// </summary>
        string Year { get; set; }
    }
}
