using System.ComponentModel;
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

            TestUtility.AssertRaisesPropertyChanged(vm, () => { vm.CitationStyle = CitationStyle.Ieee; }, "CitationStyle", "Citation");
        }

        [Test]
        public void OutputType_RaisesPropertyChanged()
        {
            var vm = new CopyReferenceDialogViewModel(new BookViewModel(TestUtility.CreateCrackingMathematics()));

            TestUtility.AssertRaisesPropertyChanged(vm, () => { vm.OutputType = ReferenceOutputType.Markdown; }, "OutputType", "Citation");
        }

        // APA
        // Should actually be Cohen, H.
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. Mathematics of Computation, 24, 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. <i>Mathematics of Computation</i>, <i>24</i>, 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Cohen, Henri. (1970). On amicable and sociable numbers. *Mathematics of Computation*, *24*, 423-429.",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        // Chicago
        [TestCase("Cohen, Henri. \"On amicable and sociable numbers.\" Mathematics of Computation 24 (1970): 423-429.",
            CitationStyle.Chicago, ReferenceOutputType.Plaintext)]
        [TestCase("Cohen, Henri. \"On amicable and sociable numbers.\" *Mathematics of Computation* 24 (1970): 423-429.",
            CitationStyle.Chicago, ReferenceOutputType.Markdown)]
        public void VerifyMinimalArticle(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var article = new Article()
            {
                Author = "Cohen, Henri",
                Journal = "Mathematics of Computation",
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

        // APA
        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "Bull. Amer. Math. Soc., 72(6), 1079. doi:10.1090/S0002-9904-1966-11654-3",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "<i>Bull. Amer. Math. Soc.</i>, <i>72</i>(6), 1079. doi:<a href=\"https://dx.doi.org/10.1090/S0002-9904-1966-11654-3\">10.1090/S0002-9904-1966-11654-3</a>",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Lander, L.J., Parkin, T.R. (1966). Counterexample to Euler's conjecture on sums of like powers. " +
            "*Bull. Amer. Math. Soc.*, *72*(6), 1079. doi: [10.1090/S0002-9904-1966-11654-3](https://dx.doi.org/10.1090/S0002-9904-1966-11654-3)",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        // Chicago
        [TestCase("Lander, L.J., Parkin, T.R. \"Counterexample to Euler's conjecture on sums of like powers.\" " +
            "Bull. Amer. Math. Soc. 72, no. 6 (1966): 1079. doi:10.1090/S0002-9904-1966-11654-3",
            CitationStyle.Chicago, ReferenceOutputType.Plaintext)]
        [TestCase("Lander, L.J., Parkin, T.R. \"Counterexample to Euler's conjecture on sums of like powers.\" " +
            "*Bull. Amer. Math. Soc.* 72, no. 6 (1966): 1079. doi: [10.1090/S0002-9904-1966-11654-3](https://dx.doi.org/10.1090/S0002-9904-1966-11654-3)",
            CitationStyle.Chicago, ReferenceOutputType.Markdown)]
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
        // Chicago
        [TestCase("Silver, Nate. The Signal and the Noise. Penguin Books, 2012.", CitationStyle.Chicago, ReferenceOutputType.Plaintext)]
        [TestCase("Silver, Nate. *The Signal and the Noise*. Penguin Books, 2012.", CitationStyle.Chicago, ReferenceOutputType.Markdown)]
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

        // APA
        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). Applications Programming in C. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Plaintext)]
        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). <i>Applications Programming in C</i>. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Html)]
        [TestCase("Johnsonbaugh, R., Kalin, M. (1989). *Applications Programming in C*. New York, NY: Macmillan.",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        // Chicago
        // Should actually be "Johnsonbaugh, R., and M. Kalin"
        [TestCase("Johnsonbaugh, R., Kalin, M. Applications Programming in C. New York, NY: Macmillan, 1989.",
            CitationStyle.Chicago, ReferenceOutputType.Plaintext)]
        [TestCase("Johnsonbaugh, R., Kalin, M. *Applications Programming in C*. New York, NY: Macmillan, 1989.",
            CitationStyle.Chicago, ReferenceOutputType.Markdown)]
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

        // APA
        [TestCase("Shannon, Claude. (1937). *A symbolic analysis of relay and switching circuits* (Master's thesis). Massachusetts Institute of Technology.",
            CitationStyle.Apa, ReferenceOutputType.Markdown)]
        // Chicago
        [TestCase("Shannon, Claude. \"A symbolic analysis of relay and switching circuits.\" Master's thesis, Massachusetts Institute of Technology, 1937.",
            CitationStyle.Chicago, ReferenceOutputType.Markdown)]
        public void VerifyCompleteThesis(string expected, CitationStyle style, ReferenceOutputType type)
        {
            var vm = new CopyReferenceDialogViewModel(new ThesisViewModel(TestUtility.CreateShannonThesis()));
            vm.CitationStyle = style;
            vm.OutputType = type;

            Assert.That(vm.Citation, Is.EqualTo(expected));
        }

        [TestCase(CitationStyle.Apa)]
        [TestCase(CitationStyle.Chicago)]
        public void VerifyDoctoralThesis(CitationStyle style)
        {
            var thesis = TestUtility.CreateShannonThesis();
            thesis.Kind = ThesisKind.Doctoral;

            var vm = new CopyReferenceDialogViewModel(new ThesisViewModel(thesis));
            vm.CitationStyle = style;
            vm.OutputType = ReferenceOutputType.Plaintext;

            Assert.That(vm.Citation, Does.Contain("PhD thesis"));
        }

        [TestCase(CitationStyle.Apa)]
        [TestCase(CitationStyle.Chicago)]
        public void VerifyLicentiateThesis(CitationStyle style)
        {
            var thesis = TestUtility.CreateShannonThesis();
            thesis.Kind = ThesisKind.Licentiate;

            var vm = new CopyReferenceDialogViewModel(new ThesisViewModel(thesis));
            vm.CitationStyle = style;
            vm.OutputType = ReferenceOutputType.Plaintext;

            Assert.That(vm.Citation, Does.Contain("Licentiate thesis"));
        }
    }
}
