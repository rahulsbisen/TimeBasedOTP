﻿using System;
using NUnit.Framework;

namespace OneTimePassword.Impl.Tests.UnitTests
{
    [TestFixture()]
    public class ErrorFactoryTests
    {
        [Test()]
        public void ShouldGetInvalidRequestError()
        {
            var invalidRequestError = new ErrorFactory().GetInvalidRequestError();
            Assert.That(invalidRequestError, Is.Not.Null);
            Assert.That(invalidRequestError.Code, Is.EqualTo("InvalidRequest"));
            Assert.That(invalidRequestError.Description, Is.EqualTo("Please check your request and try again."));
        }

        [Test]
        public void ShouldGetErrorForArgumentOutOfRangeException()
        {
            var errorForException = new ErrorFactory().GetErrorForException(new ArgumentOutOfRangeException());
            Assert.That(errorForException, Is.Not.Null);
            Assert.That(errorForException.Code, Is.EqualTo("InternalError"));
            Assert.That(errorForException.Description, Is.EqualTo("Something went wrong, please try again later."));
        }
    }
}