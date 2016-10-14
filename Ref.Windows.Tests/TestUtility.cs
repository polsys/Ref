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
            return new Book()
            {
                Author = "Beveridge, Colin",
                Key = "Beveridge2016",
                Publisher = "Octopus Books",
                Title = "Cracking Mathematics",
                Year = "2016"
            };
        }
        
        public static Book CreateMakeAndDo()
        {
            return new Book()
            {
                Author = "Parker, Matt",
                Key = "Parker2014",
                Publisher = "Particular Books",
                Title = "Things to Make and Do in the Fourth Dimension",
                Year = "2014"
            };
        }
    }
}
