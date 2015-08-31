using System;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl.Error
{
    public interface IErrorFactory
    {
        OTPError GetInvalidRequestError();
        OTPError GetErrorForException(ArgumentOutOfRangeException exception);
    }
}