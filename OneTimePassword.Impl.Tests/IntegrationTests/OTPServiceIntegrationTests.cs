using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl.Tests.IntegrationTests
{
    [TestFixture(Category = "IntegrationTests")]
    public class OTPServiceIntegrationTests
    {
        private IOTPService otpService;

        [SetUp]
        public void Setup()
        {
            var otpConfiguration = new OTPConfiguration()
            {
                IntervalSeconds = 31,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);
        }

        [TestCaseSource(nameof(GeneratedUserIds))]
        public void GenerateAndValidateOTP(String userId)
        {
            var generateOTPResponse = otpService.GenerateOtp(new GenerateOTPRequest()
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);
            Assert.That(IsValidNumberFormat(generateOTPResponse.OTP),Is.True);

            var validateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest()
            {
                OTP = generateOTPResponse.OTP,
                UserId = userId
            });
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        private bool IsValidNumberFormat(string otp)
        {
            var intValue = Int32.Parse(otp);
            return intValue > 0;
        }

        [Test]
        public void ShouldValidateWithinWithinExpiryLimit()
        {
            var otpConfiguration = new OTPConfiguration()
            {
                IntervalSeconds = 5,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);

            string userId = "2j32jk432m23482394jkddsd";
            var generateOTPResponse = otpService.GenerateOtp(new GenerateOTPRequest()
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);

            Thread.Sleep(3000);

            var validateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest()
            {
                OTP = generateOTPResponse.OTP,
                UserId = userId
            });
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        [Test]
        public void ShouldNotValidateOutsideExpiryLimit()
        {
            var otpConfiguration = new OTPConfiguration()
            {
                IntervalSeconds = 2,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);

            string userId = "2j32jk432m23482394jkddsd";
            var generateOTPResponse = otpService.GenerateOtp(new GenerateOTPRequest()
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);

            Thread.Sleep(5000);

            var validateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest()
            {
                OTP = generateOTPResponse.OTP,
                UserId = userId
            });
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.False);
        }

        private IEnumerable<String> GeneratedUserIds()
        {
            for (int i = 0; i < 500; i++)
            {
                yield return GenerateRandomUserId();
            }
        }


        private String GenerateRandomUserId()
        {
            return Guid.NewGuid().ToString("n").Substring(0, 16);
        }
    }
}