using System.Collections.Generic;

namespace OneTimePassword.Contract.Response
{
    public class BaseResponse
    {
        public OTPError Error { get; set; }
    }
}