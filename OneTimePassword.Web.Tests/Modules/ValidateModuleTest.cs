using Nancy;
using Nancy.Testing;
using NUnit.Framework;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using OneTimePassword.Impl;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;
using OneTimePassword.Web.Tests.Framework;

namespace OneTimePassword.Web.Tests.Modules
{
    [TestFixture]
    public class ValidateModuleTest : NancyTestBase
    {
        [Test]
        public void ValidatePageShouldHaveValidationForm()
        {
            var browserResponse = Browser.Get("/Validate", context => { context.HttpRequest(); });

            Assert.That(browserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["form"].ShouldExist();
            browserResponse.Body["input#validate_userId"].ShouldExistOnce();
            browserResponse.Body["input#validate_otp"].ShouldExistOnce();
            browserResponse.Body["input#submit_validateOtp"].ShouldExistOnce();
            browserResponse.Body["a[href='/']"].ShouldExistOnce();
        }

        [Test]
        public void ValidateSuccessfullyForCorrectOTPOnFormSubmit()
        {
            var userId = "random_user";
            string otp = GetValidOTP(userId);

            var browserResponse = Browser.Post("/Validate", context =>
            {
                context.FormValue("userId", userId);
                context.FormValue("otp", otp);
                context.HttpRequest();
            });

            Assert.That(browserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["form"].ShouldExist();
            browserResponse.Body["input#validate_userId"].ShouldExistOnce();
            browserResponse.Body["input#validate_otp"].ShouldExistOnce();
            browserResponse.Body["input#submit_validateOtp"].ShouldExistOnce();

            browserResponse.Body["#error_description"].ShouldNotExist();
            browserResponse.Body["#result b"].ShouldExist().And.AnyShouldContain("Authentication Success.");
        }

        [Test]
        public void ValidateFailForWrongOTPOnFormSubmit()
        {
            var userId = "random_user";
            string otp = "123456";

            var browserResponse = Browser.Post("/Validate", context =>
            {
                context.FormValue("userId", userId);
                context.FormValue("otp", otp);
                context.HttpRequest();
            });

            Assert.That(browserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["form"].ShouldExist();
            browserResponse.Body["input#validate_userId"].ShouldExistOnce();
            browserResponse.Body["input#validate_otp"].ShouldExistOnce();
            browserResponse.Body["input#submit_validateOtp"].ShouldExistOnce();

            browserResponse.Body["#error_description"].ShouldNotExist();
            browserResponse.Body["#result b"].ShouldExist().And.AnyShouldContain("Authentication Failed.");
        }

        private string GetValidOTP(string userId)
        {
            var otpService = new OTPService(new HmacBasedOTPAlgorithm(),
                new ExpiryBasedMovingFactorAlgorithm(new OTPConfiguration()), new ErrorFactory(), new OTPConfiguration());
            var generateOtpRequest = new GenerateOTPRequest() {UserId = userId};
            GenerateOTPResponse generateOTPResponse = otpService.GenerateOtp(generateOtpRequest);
            return generateOTPResponse.OTP;
        }
    }
}