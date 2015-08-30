using System;
using NUnit.Framework;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class StringUtilitiesTests
    {
        [Test()]
        public void ShouldReturnTrueForSameAsciiString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime("kaldhasn721321", "kaldhasn721321"), Is.True);
        }

        [Test()]
        public void ShouldReturnTrueForSameUnicodeString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime("rahulsbisen@gmail.com", "rahulsbisen@gmail.com"), Is.True);
        }

        [Test()]
        public void ShouldReturnTrueForBothNullString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime(null, null), Is.True);
        }

        [Test()]
        public void ShouldReturnTrueForBothEmptyString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime(String.Empty, String.Empty), Is.True);
        }

        [Test()]
        public void ShouldReturnFalseForDifferentAsciiString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime("kaldhasn721321", "joljksadsad2"), Is.False);
        }

        [Test()]
        public void ShouldReturnFalseForDifferentUnicodeString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime("rahulsbisen@gmail.com", "abd@gmail.com"), Is.False);
        }

        [Test()]
        public void ShouldReturnFalseForOneNullString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime(null, "asdsadas"), Is.False);
        }

        [Test()]
        public void ShouldReturnFalseForOneEmptyString()
        {
            Assert.That(StringUtilities.StringEqualsInConstantTime(String.Empty, "sadasmkasodww"), Is.False);
        }
    }
}