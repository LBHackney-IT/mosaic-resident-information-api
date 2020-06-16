using FluentAssertions;
using MosaicResidentInformationApi.V1.UseCase;
using NUnit.Framework;

namespace MosaicResidentInformationApi.Tests.V1.UseCase
{
    public class ValidatePostcodeTests
    {
        [TestCase("E8 1JJ")]
        [TestCase("E8T 1JJ")]
        [TestCase("EY98 1JJ")]
        [TestCase("EH1A 1JJ")]
        [TestCase("EH8 1JJ")]
        [TestCase("E87 1JJ")]
        [TestCase("E87  1JJ")]
        [TestCase("e87  1Jj")]
        [TestCase(null)]
        public void ValidPostcodesReturnTrue(string postcode)
        {
            var validator = new ValidatePostcode();
            validator.Execute(postcode).Should().BeTrue();
        }
        [TestCase("1")]
        [TestCase("BA56 6Y")]
        [TestCase("B 7JI")]
        [TestCase("BA56 YTH")]
        [TestCase("BA 6YU")]
        [TestCase("BAY 6IY")]
        [TestCase("BA56 YH")]
        [TestCase("6A56 6YH")]
        [TestCase("B656 6YU")]
        [TestCase("BHHHH656 6YU")]
        [TestCase("BH6 6YUuuu")]
        [TestCase("Q33 8TH")]
        public void InvalidPostcodesReturnsFalse(string postcode)
        {
            var validator = new ValidatePostcode();
            validator.Execute(postcode).Should().BeFalse();
        }
    }
}
