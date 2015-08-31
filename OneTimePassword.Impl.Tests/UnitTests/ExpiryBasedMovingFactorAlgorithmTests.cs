using System;
using NUnit.Framework;
using OneTimePassword.Impl.Algorithm;
using Rhino.Mocks;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class ExpiryBasedMovingFactorAlgorithmTests
    {
        [SetUp]
        public void Setup()
        {
            _otpConfiguration = new OTPConfiguration
            {
                OTPExpiryInSeconds = 30,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };

            _currentTimeFunction = MockRepository.GenerateMock<Func<DateTime>>();
            _expiryBasedMovingFactorAlgorithm = new ExpiryBasedMovingFactorAlgorithm(_otpConfiguration,
                _currentTimeFunction);
        }

        [TearDown]
        public void Teardown()
        {
            _currentTimeFunction.VerifyAllExpectations();
        }

        private OTPConfiguration _otpConfiguration;
        private Func<DateTime> _currentTimeFunction;
        private IMovingFactorAlgorithm _expiryBasedMovingFactorAlgorithm;

        [Test]
        public void ShouldGenerateDifferentMovingFactorOutsideTimeInterval()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 00);
            var dateTimePlus31Seconds = dateTime.AddSeconds(30);
            _currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            _currentTimeFunction.Expect(func => func()).Return(dateTimePlus31Seconds).Repeat.Once();
            var secondMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            Assert.That(firstMovingFactor, Is.Not.EqualTo(secondMovingFactor));
        }

        [Test]
        public void ShouldGenerateSameMovingFactorWithinTimeInterval()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 00);
            var dateTimePlus10Seconds = dateTime.AddSeconds(10);
            _currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            _currentTimeFunction.Expect(func => func()).Return(dateTimePlus10Seconds).Repeat.Once();
            var secondMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            Assert.That(firstMovingFactor, Is.EqualTo(secondMovingFactor));
        }

        [Test]
        public void ShouldGetMovingFactorForValidationWhichContainsOriginalMovingFactor()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 29);
            var dateTimePlus10Seconds = dateTime.AddSeconds(10);
            _currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            _currentTimeFunction.Expect(func => func()).Return(dateTimePlus10Seconds).Repeat.Twice();
            var secondMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            var movingFactorForValidation = _expiryBasedMovingFactorAlgorithm.GetMovingFactorForValidation();
            Assert.That(movingFactorForValidation, Is.Not.Null);
            Assert.That(movingFactorForValidation, Has.Member(firstMovingFactor));
            Assert.That(movingFactorForValidation, Has.Member(secondMovingFactor));
        }

        [Test]
        public void ShouldGetMovingFactorForValidationWhichDoesNotContainsOriginalMovingFactorIfTimeHasExpired()
        {
            var dateTime = new DateTime(2015, 08, 29, 09, 00, 29);
            var dateTimePlus31Seconds = dateTime.AddSeconds(31);
            _currentTimeFunction.Expect(func => func()).Return(dateTime).Repeat.Once();
            var firstMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            _currentTimeFunction.Expect(func => func()).Return(dateTimePlus31Seconds).Repeat.Twice();
            var secondMovingFactor = _expiryBasedMovingFactorAlgorithm.GetMovingFactor();

            var movingFactorForValidation = _expiryBasedMovingFactorAlgorithm.GetMovingFactorForValidation();
            Assert.That(movingFactorForValidation, Is.Not.Null);
            Assert.That(movingFactorForValidation, Has.No.Member(firstMovingFactor));
            Assert.That(movingFactorForValidation, Has.Member(secondMovingFactor));
        }
    }
}