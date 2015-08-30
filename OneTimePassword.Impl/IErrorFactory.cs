using System;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl
{
    public interface IErrorFactory
    {
        OTPError GetInvalidRequestError();
        OTPError GetErrorForException(ArgumentOutOfRangeException exception);
    }
}