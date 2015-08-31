using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Contract
{
    public interface IOTPService
    {
        GenerateOTPResponse GenerateOtp(GenerateOTPRequest generateOtpRequest);

        ValidateOTPResponse ValidateOtp(ValidateOTPRequest validateOtpRequest);
    }
}