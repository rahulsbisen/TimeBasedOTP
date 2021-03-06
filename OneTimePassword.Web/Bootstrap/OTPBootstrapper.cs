﻿using System;
using System.Configuration;
using Nancy;
using Nancy.TinyIoc;
using OneTimePassword.Contract;
using OneTimePassword.Impl;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;

namespace OneTimePassword.Web.Bootstrap
{
    public class OTPBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            var otpConfiguration = new OTPConfiguration()
            {
                NumberOfDigitsInOTP = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfDigitsInOTP"]),
                OTPExpiryInSeconds = Convert.ToInt64(ConfigurationManager.AppSettings["OTPExpiryInSeconds"])
            };
            container.Register(otpConfiguration);
            container.Register<IOTPService,OTPService>();
            container.Register<IOTPAlgorithm,HmacBasedOTPAlgorithm>();
            container.Register<IErrorFactory,ErrorFactory>();
            container.Register<IMovingFactorAlgorithm>(new ExpiryBasedMovingFactorAlgorithm(otpConfiguration));
        }
    }
}