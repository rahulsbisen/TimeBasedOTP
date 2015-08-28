using System;
using System.Collections.Generic;

namespace OneTimePassword.Contract.Response
{
    public class BaseResponse
    {
        public OTPError Error { get; set; }
    }

    public class OTPError
    {
        public String Code { get; set; }
        public String Description { get; set; }
    }
}