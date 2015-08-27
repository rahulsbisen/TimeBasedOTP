using System;
using System.Collections.Generic;

namespace OneTimePassword.Contract.Response
{
    public class BaseResponse
    {
        public List<Error> Errors;
    }

    public class Error
    {
        public String Code { get; set; }
        public String Description { get; set; }
    }
}