using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl
{
    public interface IErrorFactory
    {
        OTPError GetInvalidRequestError();
    }

    class ErrorFactory : IErrorFactory
    {
        public OTPError GetInvalidRequestError()
        {
            return new OTPError()
            {
                Code = "InvalidRequest",
                Description = "Please check your request and try again."
            };
        }
    }
}