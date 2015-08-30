﻿using Nancy;
using Nancy.Testing;
using NUnit.Framework;

namespace OneTimePassword.Web.Tests
{
    [TestFixture]
    public class HomeModuleTest : NancyTestBase
    {
        [Test]
        public void ShouldOpenPageWithTwoLinks()
        {
            var browserResponse = browser.Get("/", context => { context.HttpRequest(); });
           
            Assert.That(browserResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK), browserResponse.Body.AsString());
            browserResponse.Body["a[href='Generate']"].ShouldExistOnce();
            browserResponse.Body["a[href='Validate']"].ShouldExistOnce();
        }
    }
}