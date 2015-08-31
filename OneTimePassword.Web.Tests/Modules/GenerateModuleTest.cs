using Nancy;
using Nancy.Testing;
using NUnit.Framework;
using OneTimePassword.Web.Tests.Framework;

namespace OneTimePassword.Web.Tests.Modules
{
    [TestFixture]
    public class GenerateModuleTest : NancyTestBase
    {
        [Test]
        public void GeneratePageShouldHaveOTPGenerationForm()
        {
            var browserResponse = Browser.Get("/Generate", context => { context.HttpRequest(); });

            Assert.That(browserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["form"].ShouldExist();
            browserResponse.Body["input#generate_userId"].ShouldExistOnce();
            browserResponse.Body["input#submit_generateOtp"].ShouldExistOnce();
            browserResponse.Body["a[href='/']"].ShouldExistOnce();
        }

        [Test]
        public void GenerateOTPOnFormSubmit()
        {
            var browserResponse = Browser.Post("/Generate", context =>
            {
                context.FormValue("userId", "random_user");
                context.HttpRequest();
            });

            Assert.That(browserResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["form"].ShouldExist();
            browserResponse.Body["input#generate_userId"].ShouldExistOnce();
            browserResponse.Body["input#submit_generateOtp"].ShouldExistOnce();

            browserResponse.Body["#error_description"].ShouldNotExist();
            browserResponse.Body["#otp_result"].ShouldExist();
        }
    }
}