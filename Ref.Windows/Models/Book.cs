namespace Polsys.Ref.Models
{
    /// <summary>
    /// Represents a single book.
    /// </summary>
    internal class Book
    {
        /// <summary>
        /// The author or authors of this book.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The unique citation key for this entry.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The publisher of this book.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// The title of this book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The year this book was published in.
        /// </summary>
        public string Year { get; set; }
    }
}
