using System;
using NUnit.Framework;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using Rhino.Mocks;

namespace OneTimePassword.Impl.Tests
{
    [TestFixture]
    public class OTPServiceTests
    {
        [SetUp]
        public void Setup()
        {
            otpGenerator = MockRepository.GenerateMock<IOTPGenerator>();
            otpValidator = MockRepository.GenerateMock<IOTPValidator>();
            errorFactory = MockRepository.GenerateMock<IErrorFactory>();
            otpService = new OTPService(otpGenerator, otpValidator, errorFactory);
            invalidRequestError = new OTPError()
            {
                Code = "InvalidRequest",
                Description = "Please check your request and try again."
            };
            errorFactory.Expect(factory => factory.GetInvalidRequestError()).Return(invalidRequestError);
        }

        private OTPService otpService;
        private IOTPGenerator otpGenerator;
        private IOTPValidator otpValidator;
        private IErrorFactory errorFactory;
        private OTPError invalidRequestError;

       
        [Test]
        public void ShouldGenerateOTPForAGivenUserId()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "321382113asjd72131";

            var generateOtpRequest = new GenerateOTPRequest
            {
                UserId = userId
            };
            otpGenerator.Expect(generator => generator.CreateOTP(userId)).Return(generatedOtp);
            

            var generateOTPResponse = otpService.GenerateOtp(generateOtpRequest);
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(generateOTPResponse.OTP, Is.EqualTo(generatedOtp));
        }

        [Test]
        public void ShouldReturnSuccessResponseIfOTPIsValidated()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "321382113asjd72131";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            otpValidator.Expect(validator => validator.CheckOtp(userId, generatedOtp)).Return(true);

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.True);
        }

        [Test]
        public void ShouldReturnFailureResponseIfOTPIsNotValidated()
        {
            var userId = Guid.NewGuid().ToString();
            var generatedOtp = "321382113asjd72131";

            var validateOTPRequest = new ValidateOTPRequest()
            {
                UserId = userId,
                OTP = generatedOtp
            };

            otpValidator.Expect(validator => validator.CheckOtp(userId, generatedOtp)).Return(false);

            var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
            Assert.That(validateOTPResponse, Is.Not.Null);
            Assert.That(validateOTPResponse.UserId, Is.EqualTo(userId));
            Assert.That(validateOTPResponse.Success, Is.False);
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
            var generateOTPResponse = otpService.ValidateOtp(new ValidateOTPRequest());
            Assert.That(generateOTPResponse, Is.Not.Null);
            Assert.That(generateOTPResponse.UserId, Is.Null);
            Assert.That(generateOTPResponse.Success, Is.False);
            Assert.That(generateOTPResponse.Error, Is.EqualTo(invalidRequestError));
        }

    }
}