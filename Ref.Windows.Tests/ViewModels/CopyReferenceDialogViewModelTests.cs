﻿using System.ComponentModel;
using NUnit.Framework;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    class CopyReferenceDialogViewModelTests
    {
        [Test]
        public void CitationStyle_RaisesPropertyChanged()
        {
            var vm = new CopyReferenceDialogViewModel(new BookViewModel(TestUtility.CreateCrackingMathematics()));

            // Doing this manually instead of TestUtility since there are two properties to test
            // TODO: Refactor into a nicer method there
            int citationStyleChanged = 0;
            int citationChanged = 0;
            vm.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                if (args.PropertyName == "CitationStyle")
                {
                    citationStyleChanged++;
                }
                else if (args.PropertyName == "Citation")
                {
                    citationChanged++;
                }
            };

            vm.CitationStyle = CitationStyle.Ieee;

            Assert.That(citationStyleChanged, Is.EqualTo(1));
            Assert.That(citationChanged, Is.EqualTo(1));
        }

        [Test]
        public void OutputType_RaisesPropertyChanged()
        {
            var vm = new CopyReferenceDialogViewModel(new BookViewModel(TestUtility.CreateCrackingMathematics()));

            int outputTypeChanged = 0;
            int citationChanged = 0;
            vm.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                if (args.PropertyName == "OutputType")
                {
                    outputTypeChanged++;
                }
                else if (args.PropertyName == "Citation")
                {
                    citationChanged++;
                }
            };

            vm.OutputType = ReferenceOutputType.Markdown;

            Assert.That(outputTypeChanged, Is.EqualTo(1));
            Assert.That(citationChanged, Is.EqualTo(1));
        }

        // APA
        // Should actually be Cohen, H.
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. Mathematics of Computation, 24(110), 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. <i>Mathematics of Computation</i>, <i>24</i>(110), 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. *Mathematics of Computation*, *24*(110), 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        public void VerifyMinimalArticle(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var article = new Article()
            {
                Author = "Cohen, Henri",
                Journal = "Mathematics of Computation",
                Number = "110",
                Volume = "24",
                PageRange = "423-429",
                Title = "On amicable and sociable numbers",
                Year = "1970"
            };
            var vm = new CopyReferenceDialogViewModel(new ArticleViewModel(article));
            vm.CitationStyle = style;
            vm.OutputType = type;

            Assert.That(vm.Citation, Is.EqualTo(expected));
        }

        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "Bull. Amer. Math. Soc., 72(6), 1079. doi:10.1090/S0002-9904-1966-11654-3",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "<i>Bull. Amer. Math. Soc.</i>, <i>72</i>(6), 1079. doi:<a href=\"https://dx.doi.org/10.1090/S0002-9904-1966-11654-3\">10.1090/S0002-9904-1966-11654-3</a>",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "*Bull. Amer. Math. Soc.*, *72*(6), 1079. doi: [10.1090/S0002-9904-1966-11654-3](https://dx.doi.org/10.1090/S0002-9904-1966-11654-3)",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        public void VerifyCompleteArticle(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var vm = new CopyReferenceDialogViewModel(new ArticleViewModel(TestUtility.CreateCounterexample()));
            vm.CitationStyle = style;
            vm.OutputType = type;

            Assert.That(vm.Citation, Is.EqualTo(expected));
        }

        // APA
        // Should actually be Silver, N.
        [TestCase("Silver, Nate. (2012). The Signal and the Noise. Penguin Books.", CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Silver, Nate. (2012). <i>The Signal and the Noise</i>. Penguin Books.", CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Silver, Nate. (2012). *The Signal and the Noise*. Penguin Books.", CitationStyle.Apa, ReferenceOutputType.Markdown)]
        public void VerifyMinimalBook(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var book = new Book()
            {
                Author = "Silver, Nate",
                Publisher = "Penguin Books",
                Title = "The Signal and the Noise",
                Year = "2012"
            };
            var vm = new CopyReferenceDialogViewModel(new BookViewModel(book));
            vm.CitationStyle = style;
            vm.OutputType = type;

            Assert.That(vm.Citation, Is.EqualTo(expected));
        }

        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). Applications Programming in C. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). <i>Applications Programming in C</i>. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). *Applications Programming in C*. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        public void VerifyCompleteBook(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var book = new Book()
            {
                Address = "New York, NY",
                Author = "Johnsonbaugh, R., Kalin, M.",
                Publisher = "Macmillan",
                Title = "Applications Programming in C",
                Year = "1989"
            };
            var vm = new CopyReferenceDialogViewModel(new BookViewModel(book));
            vm.CitationStyle = style;
            vm.OutputType = type;

            Assert.That(vm.Citation, Is.EqualTo(expected));
        }
    }
}