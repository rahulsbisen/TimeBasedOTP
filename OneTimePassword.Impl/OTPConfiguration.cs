using System;

namespace OneTimePassword.Impl
{
    public class OTPConfiguration
    {
        public long OTPExpiryInSeconds { get; set; }
        public String PrivateKey { get; set; }
        public int NumberOfDigitsInOTP { get; set; }

        public OTPConfiguration()
        {
            OTPExpiryInSeconds = 30;
            NumberOfDigitsInOTP = 6;
            PrivateKey = "jasoiduasl21312213nkasd2";
        }
    }
}