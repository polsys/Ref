using System.ComponentModel;
using NUnit.Framework;
using Polsys.Ref.Models;

namespace Polsys.Ref.Tests
{
    static class TestUtility
    {
        /// <summary>
        /// Asserts that calling <paramref name="func"/> raises a <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// with the specified property name.
        /// </summary>
        /// <param name="viewModel">The view model implementing <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="func">The test code.</param>
        /// <param name="propertyName">The expected property name.</param>
        public static void AssertRaisesPropertyChanged(INotifyPropertyChanged viewModel, TestDelegate func, string propertyName)
        {
            int propertyChangedCount = 0;
            PropertyChangedEventHandler handler = (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == propertyName)
                    propertyChangedCount++;
            };

            try
            {
                viewModel.PropertyChanged += handler;
                func.Invoke();
                Assert.That(propertyChangedCount, Is.EqualTo(1),
                    "The PropertyChanged event for '" + propertyName + "' was not fired or was fired more than once.");
            }
            finally
            {
                viewModel.PropertyChanged -= handler;
            }
        }

        // Example data
        public static Book CreateCrackingMathematics()
        {
            // This book contains all possible book fields
            // (In addition to containing great content!)
            return new Book()
            {
                Address = "London",
                Author = "Beveridge, Colin",
                Edition = "1st",
                Editor = "Poulter, Pollyanna",
                Key = "Beveridge2016",
                Number = "1",
                Publisher = "Octopus Books",
                Series = "Musings of the Mathematical Ninja", // Not really, at least yet.
                Title = "Cracking Mathematics",
                Volume = "1", // No Volume II yet.
                Year = "2016"
            };
        }
        
        public static Book CreateMakeAndDo()
        {
            // This does not contain all the fields, but is useful as a second distinct book
            // (And a great read as well!)
            return new Book()
            {
                Address = "London",
                Author = "Parker, Matt",
                Edition = "First",
                Key = "Parker2014",
                Publisher = "Particular Books",
                Title = "Things to Make and Do in the Fourth Dimension",
                Year = "2014"
            };
        }
    }
}
