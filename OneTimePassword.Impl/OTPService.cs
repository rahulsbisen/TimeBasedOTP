using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using static System.String;

namespace OneTimePassword.Impl
{
    public class OTPService : IOTPService
    {
        private readonly IOTPGenerator otpGenerator;
        private readonly IOTPValidator otpValidator;
        private readonly IErrorFactory errorFactory;

        public OTPService(IOTPGenerator otpGenerator, IOTPValidator otpValidator, IErrorFactory errorFactory)
        {
            this.otpGenerator = otpGenerator;
            this.otpValidator = otpValidator;
            this.errorFactory = errorFactory;
        }

        public GenerateOTPResponse GenerateOtp(GenerateOTPRequest generateOtpRequest)
        {
            if (!IsNullOrEmpty(generateOtpRequest?.UserId))
            {
                var otp = otpGenerator.CreateOTP(generateOtpRequest.UserId);
                return new GenerateOTPResponse()
                {
                    UserId = generateOtpRequest.UserId,
                    OTP = otp
                };
            }
            return new GenerateOTPResponse()
            {
                Error = errorFactory.GetInvalidRequestError()
            };
        }

        public ValidateOTPResponse ValidateOtp(ValidateOTPRequest validateOtpRequest)
        {
            if (!IsNullOrEmpty(validateOtpRequest?.UserId) && !IsNullOrEmpty(validateOtpRequest.OTP))
            {
                var isValidOTP = otpValidator.CheckOtp(validateOtpRequest.UserId, validateOtpRequest.OTP);
                return new ValidateOTPResponse()
                {
                    UserId = validateOtpRequest.UserId,
                    Success = isValidOTP
                };
            }
            return new ValidateOTPResponse()
            {
                Error = errorFactory.GetInvalidRequestError()
            };
        }
    }
}
