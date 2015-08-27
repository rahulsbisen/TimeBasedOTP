using System;

namespace OneTimePassword.Contract.Response
{
    public class ValidateOTPResponse : BaseResponse
    {
        public String UserId { get; set; }
        public Boolean Success { get; set; }
    }
}