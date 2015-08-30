using Nancy.Testing;
using NUnit.Framework;
using OneTimePassword.Web.Bootstrap;

namespace OneTimePassword.Web.Tests
{
    public abstract class NancyTestBase
    {
        protected Browser browser;

        [SetUp]
        public void Setup()
        {
            var bootstrapper = new Bootstrapper();
            browser = new Browser(bootstrapper);
        }
    }
}