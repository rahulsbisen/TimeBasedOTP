using System;
using System.Collections.Generic;
using NUnit.Framework;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using Rhino.Mocks;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class OTPServiceTests
    {
        [SetUp]
        public void Setup()
        {
            otpAlgorithm = MockRepository.GenerateMock<IOTPAlgorithm>();
            movingFactorAlgorithm = MockRepository.GenerateMock<IMovingFactorAlgorithm>();
            errorFactory = MockRepository.GenerateMock<IErrorFactory>();
            otpConfiguration = new OTPConfiguration()
            {
                OTPExpiryInSeconds = 31,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            otpService = new OTPService(otpAlgorithm, movingFactorAlgorithm, errorFactory, otpConfiguration);
            invalidRequestError = new OTPError()
            {
                Code = "InvalidRequest",
                Description = "Please check your request and try again."
            };
            errorFactory.Expect(factory => factory.GetInvalidRequestError()).Return(invalidRequestError);

            genericError = new OTPError()
            {
                Code = "InternalError",
                Description = "Something went wrong, please try again later."
            };
            errorFactory.Expect(factory => factory.GetErrorForException(null)).IgnoreArguments().Return(genericError);
        }

        private IOTPService otpService;
        private IOTPAlgorithm otpAlgorithm;
        private IMovingFactorAlgorithm movingFactorAlgorithm;
        private IErrorFactory errorFactory;
        private OTPConfiguration otpConfiguration;
        private OTPError invalidRequestError;
        private OTPError genericError;


        [Test]
        public void ShouldGenerateOTPForAGivenUserId()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "321382113asjd72131";

            var generateOtpRequest = new GenerateOTPRequest
            {
                UserId = userId
            };

            var movingFactor = 87302;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactor()).Return(movingFactor);
            otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactor,
                        otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);


            var generateOTPResponse = otpService.GenerateOtp(generateOtpRequest);
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.EqualTo(generatedOtp));
        }

        [Test]
        public void ShouldReturnSuccessResponseIfOTPIsValidated()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "213213";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            var movingFactor = 87302;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactor
            });
            otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactor,
                        otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        [Test]
        public void ShouldReturnFailureResponseIfOTPIsNotValidated()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "213213";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            var movingFactor = 87305;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactor
            });
            otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactor,
                        otpConfiguration.NumberOfDigitsInOTP)).Return("809012");

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.False);
        }

        [Test]
        public void ShouldReturnSuccessIfAnyOfMovingFactorIsMatched()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "213213";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            var movingFactorOne = 87302;
            var movingFactorTwo = 87303;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactorOne,
                movingFactorTwo
            });
            otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactorOne,
                        otpConfiguration.NumberOfDigitsInOTP)).Return("809012");
            otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactorTwo,
                        otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        [Test]
        public void ShouldReturnErrorForInvalidGenerateOTPRequest()
        {
            var generateOTPResponse = otpService.GenerateOtp(new GenerateOTPRequest());
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.Null);
            Assert.That(generateOTPResponse.OTP, Is.Null);
            Assert.That(generateOTPResponse.Error, Is.EqualTo(invalidRequestError));
        }

        [Test]
        public void ShouldReturnErrorForInvalidValidateOTPRequest()
        {
            var validateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest());
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.Null);
            Assert.That(validateOTPResponse.Success, Is.False);
            Assert.That(validateOTPResponse.Error, Is.EqualTo(invalidRequestError));
        }

        [Test]
        public void ShouldReturnErrorIfArgumentExceptionIsThrownByAlgorithmForValidateOTPRequest()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "213213";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            var movingFactor = 87302;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation())
                .Return(new List<long>() {movingFactor});
            otpAlgorithm.Expect(algorithm =>
                algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactor,
                    otpConfiguration.NumberOfDigitsInOTP))
                .Throw(new ArgumentOutOfRangeException(nameof(userId)));

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.Null);
            Assert.That(validateOTPResponse.Success, Is.False);
            Assert.That(validateOTPResponse.Error, Is.EqualTo(genericError));
        }

        [Test]
        public void ShouldReturnErrorIfArgumentExceptionIsThrownByAlgorithmForGenerateOTPRequest()
        {
            var userId = Guid.NewGuid().ToString();

            var generateOtpRequest = new GenerateOTPRequest
            {
                UserId = userId
            };

            var movingFactor = 87302;
            movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactor()).Return(movingFactor);
            otpAlgorithm.Expect(algorithm =>
                algorithm.GenerateOTP(userId, otpConfiguration.PrivateKey, movingFactor,
                    otpConfiguration.NumberOfDigitsInOTP))
                .Throw(new ArgumentOutOfRangeException(nameof(userId)));

            var generateOTPResponse = otpService.GenerateOtp(generateOtpRequest);
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.Null);
            Assert.That(generateOTPResponse.OTP, Is.Null);
            Assert.That(generateOTPResponse.Error, Is.EqualTo(genericError));
        }
    }
}