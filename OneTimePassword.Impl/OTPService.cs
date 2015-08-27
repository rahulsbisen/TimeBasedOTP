using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Impl
{
    public class OTPService : IOTPService
    {
        public GenerateOTPResponse GenerateOtp(GenerateOTPRequest generateOtpRequest)
        {
            throw new NotImplementedException();
        }

        public ValidateOTPResponse ValidateOtp(ValidateOTPRequest validateOtpRequest)
        {
            throw new NotImplementedException();
        }
    }
}
