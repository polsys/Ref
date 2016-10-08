using System;
using Ref.Windows.Models;

namespace Ref.Windows.ViewModels
{
    /// <summary>
    /// The View Model for <see cref="Book"/>.
    /// </summary>
    internal class BookViewModel
    {
        public string Author { get; set; }
        public string Key { get; set; }
        public string Publisher { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }

        /// <summary>
        /// Constructs a new BookViewModel from the specified <see cref="Book"/>.
        /// </summary>
        /// <param name="book">The Book this ViewModel refers to.</param>
        public BookViewModel(Book book)
        {
            Author = book.Author;
            Key = book.Key;
            Publisher = book.Publisher;
            Title = book.Title;
            Year = book.Year;
        }
    }
}
