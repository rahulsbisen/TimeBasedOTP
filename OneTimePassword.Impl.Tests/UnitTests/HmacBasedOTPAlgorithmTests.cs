using System;
using NUnit.Framework;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Utils;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class HmacBasedOTPAlgorithmTests
    {
        [SetUp]
        public void Setup()
        {
            _hmacBasedOTPAlgorithm = new HmacBasedOTPAlgorithm();
        }

        private IOTPAlgorithm _hmacBasedOTPAlgorithm;

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void ShouldGenerateAnOTPWithGivenNumberOfDigits(int numberOfDigits)
        {
            var generateOTP = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10,
                numberOfDigits);
            Assert.That(generateOTP, Is.Not.Null);
            Assert.That(generateOTP.Length, Is.EqualTo(numberOfDigits));
        }

        [Test]
        public void ShouldGenerateDifferentOTPGivenDifferentUserInputAndSameMovingFactor()
        {
            var generateOTPFirst = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10, 6);

            var generateOTPSecond = _hmacBasedOTPAlgorithm.GenerateOTP("onasmasoawlwe3", "alasnkasdhas7232", 10, 6);

            Assert.That(StringUtilities.StringEqualsInConstantTime(generateOTPFirst, generateOTPSecond), Is.False);
        }

        [Test]
        public void ShouldGenerateDifferentOTPGivenSameUserInputAndDifferentMovingFactor()
        {
            var generateOTPFirst = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10, 6);

            var generateOTPSecond = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 12, 6);

            Assert.That(StringUtilities.StringEqualsInConstantTime(generateOTPFirst, generateOTPSecond), Is.False);
        }

        [Test]
        public void ShouldGenerateSameOTPGivenSameUserInputAndMovingFactor()
        {
            var generateOTPFirst = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10, 6);

            var generateOTPSecond = _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10, 6);

            Assert.That(StringUtilities.StringEqualsInConstantTime(generateOTPFirst, generateOTPSecond), Is.True);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(-3)]
        public void ShouldThrowExceptionForInvalidNumberOfDigits(int numberOfDigits)
        {
            _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "alasnkasdhas7232", 10, numberOfDigits);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void ShouldThrowExceptionIfSecretIsNullOrEmpty()
        {
            _hmacBasedOTPAlgorithm.GenerateOTP("hdaska7232kls6", "", 10, 5);
        }

        [Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void ShouldThrowExceptionIfUserInputIsNullOrEmpty()
        {
            _hmacBasedOTPAlgorithm.GenerateOTP("", "alasnkasdhas7232", 10, 5);
        }
    }
}