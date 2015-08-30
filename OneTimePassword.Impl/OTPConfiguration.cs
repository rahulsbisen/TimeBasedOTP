using System;

namespace OneTimePassword.Impl
{
    public class OTPConfiguration
    {
        public long IntervalSeconds { get; set;  }
        public String PrivateKey { get; set; }
        public int NumberOfDigitsInOTP { get; set; }

    }
}