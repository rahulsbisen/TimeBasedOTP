using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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