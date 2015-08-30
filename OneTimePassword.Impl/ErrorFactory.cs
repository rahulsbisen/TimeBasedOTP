using System;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl
{
    public class ErrorFactory : IErrorFactory
    {
        public OTPError GetInvalidRequestError()
        {
            return new OTPError()
            {
                Code = "InvalidRequest",
                Description = "Please check your request and try again."
            };
        }

        public OTPError GetErrorForException(ArgumentOutOfRangeException exception)
        {
            return new OTPError()
            {
                Code = "InternalError",
                Description = "Something went wrong, please try again later."
            };
        }
    }
}