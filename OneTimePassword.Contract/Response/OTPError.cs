using System;

namespace OneTimePassword.Contract.Response
{
    public class OTPError
    {
        public String Code { get; set; }
        public String Description { get; set; }
    }
}