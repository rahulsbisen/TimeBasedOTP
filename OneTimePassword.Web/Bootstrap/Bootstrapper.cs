using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.TinyIoc;
using OneTimePassword.Contract;
using OneTimePassword.Impl;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;

namespace OneTimePassword.Web.Bootstrap
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            var otpConfiguration = new OTPConfiguration();
            container.Register(otpConfiguration);
            container.Register<IOTPService,OTPService>();
            container.Register<IOTPAlgorithm,HmacBasedOTPAlgorithm>();
            container.Register<IErrorFactory,ErrorFactory>();
            container.Register<IMovingFactorAlgorithm>(new ExpiryBasedMovingFactorAlgorithm(otpConfiguration));
        }
    }
}