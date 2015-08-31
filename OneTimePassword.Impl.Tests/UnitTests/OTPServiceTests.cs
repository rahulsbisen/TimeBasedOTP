using System;
using System.Collections.Generic;
using NUnit.Framework;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;
using Rhino.Mocks;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class OTPServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _otpAlgorithm = MockRepository.GenerateMock<IOTPAlgorithm>();
            _movingFactorAlgorithm = MockRepository.GenerateMock<IMovingFactorAlgorithm>();
            _errorFactory = MockRepository.GenerateMock<IErrorFactory>();
            _otpConfiguration = new OTPConfiguration()
            {
                OTPExpiryInSeconds = 31,
                NumberOfDigitsInOTP = 6,
                PrivateKey = "as9121jd623ms23h232k3"
            };
            _otpService = new OTPService(_otpAlgorithm, _movingFactorAlgorithm, _errorFactory, _otpConfiguration);
            _invalidRequestError = new OTPError()
            {
                Code = "InvalidRequest",
                Description = "Please check your request and try again."
            };
            _errorFactory.Stub(factory => factory.GetInvalidRequestError()).Return(_invalidRequestError);

            _genericError = new OTPError()
            {
                Code = "InternalError",
                Description = "Something went wrong, please try again later."
            };
            _errorFactory.Stub(factory => factory.GetErrorForException(null)).IgnoreArguments().Return(_genericError);
        }

        [TearDown]
        public void Teardown()
        {
            _otpAlgorithm.VerifyAllExpectations();
            _movingFactorAlgorithm.VerifyAllExpectations();
        }

        private IOTPService _otpService;
        private IOTPAlgorithm _otpAlgorithm;
        private IMovingFactorAlgorithm _movingFactorAlgorithm;
        private IErrorFactory _errorFactory;
        private OTPConfiguration _otpConfiguration;
        private OTPError _invalidRequestError;
        private OTPError _genericError;


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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactor()).Return(movingFactor);
            _otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactor,
                        _otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);


            var generateOTPResponse = _otpService.GenerateOtp(generateOtpRequest);
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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactor
            });
            _otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactor,
                        _otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);

            var validateOTPResponse = _otpService.ValidateOtp(validateOTPRequest);
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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactor
            });
            _otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactor,
                        _otpConfiguration.NumberOfDigitsInOTP)).Return("809012");

            var validateOTPResponse = _otpService.ValidateOtp(validateOTPRequest);
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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation()).Return(new List<long>()
            {
                movingFactorOne,
                movingFactorTwo
            });
            _otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactorOne,
                        _otpConfiguration.NumberOfDigitsInOTP)).Return("809012");
            _otpAlgorithm.Expect(
                algorithm =>
                    algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactorTwo,
                        _otpConfiguration.NumberOfDigitsInOTP)).Return(generatedOtp);

            var validateOTPResponse = _otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        [Test]
        public void ShouldReturnErrorForInvalidGenerateOTPRequest()
        {
            var generateOTPResponse = _otpService.GenerateOtp(new GenerateOTPRequest());
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.Null);
            Assert.That(generateOTPResponse.OTP, Is.Null);
            Assert.That(generateOTPResponse.Error, Is.EqualTo(_invalidRequestError));
        }

        [Test]
        public void ShouldReturnErrorForInvalidValidateOTPRequest()
        {
            var validateOTPResponse = _otpService.ValidateOtp(new ValidateOTPRequest());
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.Null);
            Assert.That(validateOTPResponse.Success, Is.False);
            Assert.That(validateOTPResponse.Error, Is.EqualTo(_invalidRequestError));
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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactorForValidation())
                .Return(new List<long>() {movingFactor});
            _otpAlgorithm.Expect(algorithm =>
                algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactor,
                    _otpConfiguration.NumberOfDigitsInOTP))
                .Throw(new ArgumentOutOfRangeException(nameof(userId)));

            var validateOTPResponse = _otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.Null);
            Assert.That(validateOTPResponse.Success, Is.False);
            Assert.That(validateOTPResponse.Error, Is.EqualTo(_genericError));
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
            _movingFactorAlgorithm.Expect(algorithm => algorithm.GetMovingFactor()).Return(movingFactor);
            _otpAlgorithm.Expect(algorithm =>
                algorithm.GenerateOTP(userId, _otpConfiguration.PrivateKey, movingFactor,
                    _otpConfiguration.NumberOfDigitsInOTP))
                .Throw(new ArgumentOutOfRangeException(nameof(userId)));

            var generateOTPResponse = _otpService.GenerateOtp(generateOtpRequest);
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.Null);
            Assert.That(generateOTPResponse.OTP, Is.Null);
            Assert.That(generateOTPResponse.Error, Is.EqualTo(_genericError));
        }
    }
}