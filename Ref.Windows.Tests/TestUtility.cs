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
        public static Article CreateCounterexample()
        {
            // This article contains all possible article fields, despite its length!
            return new Article()
            {
                Author = "Lander, L.J.; Parkin, T.R.",
                Doi = "10.1090/S0002-9904-1966-11654-3",
                Issn = "0273-0979",
                Journal = "Bull. Amer. Math. Soc.",
                Key = "Lander1966",
                Notes = "This is maybe the shortest proof ever.",
                Number = "6",
                PageRange = "1079",
                Title = "Counterexample to Euler's conjecture on sums of like powers",
                Volume = "72",
                Year = "1966"
            };
        }

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
                Isbn = "978-1-84403-862-6",
                Key = "Beveridge2016",
                Notes = "Have you ever seen a maths book this pretty?",
                Number = "1",
                Publisher = "Octopus Books",
                Series = "Musings of the Mathematical Ninja", // Not really, at least yet.
                Title = "Cracking Mathematics",
                Translator = "(Multiple... for all the other editions)", // Now if there was a Finnish translation...
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
