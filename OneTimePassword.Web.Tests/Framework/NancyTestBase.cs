using Nancy.Testing;
using NUnit.Framework;
using OneTimePassword.Web.Bootstrap;

namespace OneTimePassword.Web.Tests.Framework
{
    public abstract class NancyTestBase
    {
        protected Browser Browser;

        [SetUp]
        public void Setup()
        {
            var bootstrapper = new OTPBootstrapper();
            Browser = new Browser(bootstrapper);
        }
    }
}