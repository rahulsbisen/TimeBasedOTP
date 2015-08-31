namespace OneTimePassword.Impl
{
    public class OTPConfiguration
    {
        public OTPConfiguration()
        {
            OTPExpiryInSeconds = 30;
            NumberOfDigitsInOTP = 6;
            PrivateKey = "jasoiduasl21312213nkasd2";
        }

        public long OTPExpiryInSeconds { get; set; }
        public string PrivateKey { get; set; }
        public int NumberOfDigitsInOTP { get; set; }
    }
}