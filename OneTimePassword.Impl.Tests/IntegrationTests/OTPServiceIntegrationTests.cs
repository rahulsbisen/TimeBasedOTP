﻿using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;

namespace OneTimePassword.Impl.Tests.IntegrationTests
{
    [TestFixture(Category = "IntegrationTests")]
    public class OTPServiceIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            var otpConfiguration = new OTPConfiguration
            {
                OTPExpiryInSeconds = 31,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            _otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);
        }

        private IOTPService _otpService;

        [TestCaseSource(nameof(GeneratedUserIds))]
        public void GenerateAndValidateOTP(string userId)
        {
            var generateOTPResponse = _otpService.GenerateOtp(new GenerateOTPRequest
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);
            Assert.That(IsValidNumberFormat(generateOTPResponse.OTP), Is.True);

            var validateOTPResponse = _otpService.ValidateOtp(new ValidateOTPRequest
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
            var intValue = int.Parse(otp);
            return intValue > 0;
        }

        private IEnumerable<string> GeneratedUserIds()
        {
            for (int i = 0; i < 500; i++)
            {
                yield return GenerateRandomUserId();
            }
        }


        private string GenerateRandomUserId()
        {
            return Guid.NewGuid().ToString("n").Substring(0, 16);
        }

        [Test]
        public void ShouldNotValidateOutsideExpiryLimit()
        {
            var otpConfiguration = new OTPConfiguration
            {
                OTPExpiryInSeconds = 2,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            _otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);

            string userId = "2j32jk432m23482394jkddsd";
            var generateOTPResponse = _otpService.GenerateOtp(new GenerateOTPRequest
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);

            Thread.Sleep(5000);

            var validateOTPResponse = _otpService.ValidateOtp(new ValidateOTPRequest
            {
                OTP = generateOTPResponse.OTP,
                UserId = userId
            });
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.False);
        }

        [Test]
        public void ShouldValidateWithinWithinExpiryLimit()
        {
            var otpConfiguration = new OTPConfiguration
            {
                OTPExpiryInSeconds = 5,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            _otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(otpConfiguration), new ErrorFactory(),
                otpConfiguration);

            string userId = "2j32jk432m23482394jkddsd";
            var generateOTPResponse = _otpService.GenerateOtp(new GenerateOTPRequest
            {
                UserId = userId
            });

            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.Not.Empty);

            Thread.Sleep(3000);

            var validateOTPResponse = _otpService.ValidateOtp(new ValidateOTPRequest
            {
                OTP = generateOTPResponse.OTP,
                UserId = userId
            });
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }
    }
}