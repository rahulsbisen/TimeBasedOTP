using System;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;
using OneTimePassword.Impl.Algorithm;
using OneTimePassword.Impl.Error;
using OneTimePassword.Impl.Utils;
using static System.String;

namespace OneTimePassword.Impl
{
    public class OTPService : IOTPService
    {
        private readonly IErrorFactory _errorFactory;
        private readonly IMovingFactorAlgorithm _movingFactorAlgorithm;
        private readonly IOTPAlgorithm _otpAlgorithm;
        private readonly OTPConfiguration _otpConfiguration;

        public OTPService(IOTPAlgorithm otpAlgorithm, IMovingFactorAlgorithm movingFactorAlgorithm,
            IErrorFactory errorFactory, OTPConfiguration otpConfiguration)
        {
            _otpAlgorithm = otpAlgorithm;
            _movingFactorAlgorithm = movingFactorAlgorithm;
            _errorFactory = errorFactory;
            _otpConfiguration = otpConfiguration;
        }

        public GenerateOTPResponse GenerateOtp(GenerateOTPRequest generateOtpRequest)
        {
            if (!IsNullOrEmpty(generateOtpRequest?.UserId))
            {
                try
                {
                    var movingFactor = _movingFactorAlgorithm.GetMovingFactor();
                    var otp = _otpAlgorithm.GenerateOTP(generateOtpRequest.UserId, _otpConfiguration.PrivateKey,
                        movingFactor,
                        _otpConfiguration.NumberOfDigitsInOTP);
                    Console.WriteLine("Generation: OTP : {0} MovingFactor: {1}", otp, movingFactor);
                    return new GenerateOTPResponse
                    {
                        UserId = generateOtpRequest.UserId,
                        OTP = otp
                    };
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    return new GenerateOTPResponse
                    {
                        Error = _errorFactory.GetErrorForException(exception)
                    };
                }
            }
            return new GenerateOTPResponse
            {
                Error = _errorFactory.GetInvalidRequestError()
            };
        }

        public ValidateOTPResponse ValidateOtp(ValidateOTPRequest validateOtpRequest)
        {
            if (!IsNullOrEmpty(validateOtpRequest?.UserId) && !IsNullOrEmpty(validateOtpRequest.OTP))
            {
                try
                {
                    var movingFactorForValidation = _movingFactorAlgorithm.GetMovingFactorForValidation();

                    foreach (var movingFactor in movingFactorForValidation)
                    {
                        var internalOtp = _otpAlgorithm.GenerateOTP(validateOtpRequest.UserId,
                            _otpConfiguration.PrivateKey, movingFactor,
                            _otpConfiguration.NumberOfDigitsInOTP);

                        Console.WriteLine("Validation: OTP : {0} MovingFactor: {1}", internalOtp, movingFactor);

                        var isValidOTP = StringUtilities.StringEqualsInConstantTime(internalOtp, validateOtpRequest.OTP);
                        if (isValidOTP)
                        {
                            return new ValidateOTPResponse
                            {
                                UserId = validateOtpRequest.UserId,
                                Success = true
                            };
                        }
                    }

                    return new ValidateOTPResponse
                    {
                        UserId = validateOtpRequest.UserId,
                        Success = false
                    };
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    return new ValidateOTPResponse
                    {
                        Error = _errorFactory.GetErrorForException(exception)
                    };
                }
            }
            return new ValidateOTPResponse
            {
                Error = _errorFactory.GetInvalidRequestError()
            };
        }
    }
}