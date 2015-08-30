using System;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using static System.String;

namespace OneTimePassword.Impl
{
    public class OTPService : IOTPService
    {
        private readonly IOTPAlgorithm otpAlgorithm;
        private readonly IMovingFactorAlgorithm movingFactorAlgorithm;
        private readonly IErrorFactory errorFactory;
        private readonly OTPConfiguration otpConfiguration;

        public OTPService(IOTPAlgorithm otpAlgorithm, IMovingFactorAlgorithm movingFactorAlgorithm,
            IErrorFactory errorFactory, OTPConfiguration otpConfiguration)
        {
            this.otpAlgorithm = otpAlgorithm;
            this.movingFactorAlgorithm = movingFactorAlgorithm;
            this.errorFactory = errorFactory;
            this.otpConfiguration = otpConfiguration;
        }

        public GenerateOTPResponse GenerateOtp(GenerateOTPRequest generateOtpRequest)
        {
            if (!IsNullOrEmpty(generateOtpRequest?.UserId))
            {
                try
                {
                    var movingFactor = movingFactorAlgorithm.GetMovingFactor();
                    var otp = otpAlgorithm.GenerateOTP(generateOtpRequest.UserId, otpConfiguration.PrivateKey,
                        movingFactor,
                        otpConfiguration.NumberOfDigitsInOTP);
                    Console.WriteLine("Generation: OTP : {0} MovingFactor: {1}", otp, movingFactor);
                    return new GenerateOTPResponse()
                    {
                        UserId = generateOtpRequest.UserId,
                        OTP = otp
                    };
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    return new GenerateOTPResponse()
                    {
                        Error = errorFactory.GetErrorForException(exception)
                    };
                }
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
                try
                {
                    var movingFactorForValidation = movingFactorAlgorithm.GetMovingFactorForValidation();

                    foreach (var movingFactor in movingFactorForValidation)
                    {
                        var internalOtp = otpAlgorithm.GenerateOTP(validateOtpRequest.UserId,
                            otpConfiguration.PrivateKey, movingFactor,
                            otpConfiguration.NumberOfDigitsInOTP);

                        Console.WriteLine("Validation: OTP : {0} MovingFactor: {1}", internalOtp, movingFactor);

                        var isValidOTP = StringUtilities.StringEqualsInConstantTime(internalOtp, validateOtpRequest.OTP);
                        if (isValidOTP)
                        {
                            return new ValidateOTPResponse()
                            {
                                UserId = validateOtpRequest.UserId,
                                Success = true
                            };
                        }
                    }

                    return new ValidateOTPResponse()
                    {
                        UserId = validateOtpRequest.UserId,
                        Success = false
                    };
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    return new ValidateOTPResponse()
                    {
                        Error = errorFactory.GetErrorForException(exception)
                    };
                }
            }
            return new ValidateOTPResponse()
            {
                Error = errorFactory.GetInvalidRequestError()
            };
        }
    }
}