using System;

namespace OneTimePassword.Contract.Request
{
    public class ValidateOTPRequest
    {
        public String UserId { get; set; }
        public String OTP { get; set; }
    }
}