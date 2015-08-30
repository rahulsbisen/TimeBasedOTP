using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class ExpiryBasedMovingFactorAlgorithmTests
    {
        private OTPConfiguration otpConfiguration;
        private Func<DateTime> currentTimeFunction;
        private IMovingFactorAlgorithm expiryBasedMovingFactorAlgorithm;

        [SetUp]
        public void Setup()
        {
            otpConfiguration = new OTPConfiguration()
            {
                OTPExpiryInSeconds = 30,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };

            currentTimeFunction = MockRepository.GenerateMock<Func<DateTime>>();
            expiryBasedMovingFactorAlgorithm = new ExpiryBasedMovingFactorAlgorithm(otpConfiguration,
                currentTimeFunction);
        }

        [Test()]
        public void ShouldGenerateSameMovingFactorWithinTimeInterval()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 00);
            var dateTimePlus10Seconds = dateTime.AddSeconds(10);
            currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            currentTimeFunction.Expect(func => func()).Return(dateTimePlus10Seconds).Repeat.Once();
            var secondMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            Assert.That(firstMovingFactor, Is.EqualTo(secondMovingFactor));
        }

        [Test()]
        public void ShouldGenerateDifferentMovingFactorOutsideTimeInterval()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 00);
            var dateTimePlus31Seconds = dateTime.AddSeconds(30);
            currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            currentTimeFunction.Expect(func => func()).Return(dateTimePlus31Seconds).Repeat.Once();
            var secondMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            Assert.That(firstMovingFactor, Is.Not.EqualTo(secondMovingFactor));
        }

        [Test]
        public void ShouldGetMovingFactorForValidationWhichContainsOriginalMovingFactor()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 29);
            var dateTimePlus10Seconds = dateTime.AddSeconds(10);
            currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            currentTimeFunction.Expect(func => func()).Return(dateTimePlus10Seconds).Repeat.Twice();
            var secondMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            var movingFactorForValidation = expiryBasedMovingFactorAlgorithm.GetMovingFactorForValidation();
            Assert.That(movingFactorForValidation, Is.Not.Null);
            Assert.That(movingFactorForValidation, Has.Member(firstMovingFactor));
            Assert.That(movingFactorForValidation, Has.Member(secondMovingFactor));
        }

        [Test]
        public void ShouldGetMovingFactorForValidationWhichDoesNotContainsOriginalMovingFactorIfTimeHasExpired()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 29);
            var dateTimePlus31Seconds = dateTime.AddSeconds(31);
            currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            currentTimeFunction.Expect(func => func()).Return(dateTimePlus31Seconds).Repeat.Twice();
            var secondMovingFactor = expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            var movingFactorForValidation = expiryBasedMovingFactorAlgorithm.GetMovingFactorForValidation();
            Assert.That(movingFactorForValidation, Is.Not.Null);
            Assert.That(movingFactorForValidation, Has.No.Member(firstMovingFactor));
            Assert.That(movingFactorForValidation, Has.Member(secondMovingFactor));
        }
    }
}