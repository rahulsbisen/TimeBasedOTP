using System;
using System.Collections.Generic;

namespace OneTimePassword.Impl.Algorithm
{
    public class ExpiryBasedMovingFactorAlgorithm : IMovingFactorAlgorithm
    {
        private readonly OTPConfiguration _otpConfiguration;
        private readonly Func<DateTime> _currentTimeFunction;

        public ExpiryBasedMovingFactorAlgorithm(OTPConfiguration otpConfiguration,
            Func<DateTime> currentTimeFunction = null)
        {
            this._currentTimeFunction = currentTimeFunction ?? (() => DateTime.Now);
            this._otpConfiguration = otpConfiguration;
        }

        public long GetMovingFactor()
        {
            var dateTime = _currentTimeFunction();
            TimeSpan ts = GetEphocTimeSpan(dateTime);
            var totalSeconds = (long) ts.TotalSeconds;
            return totalSeconds/_otpConfiguration.OTPExpiryInSeconds;
        }

        public List<long> GetMovingFactorForValidation()
        {
            var currentTime = _currentTimeFunction();
            var result = new List<long>();
            for (int i = -1; i <= 0; i++)
            {
                var movingFactorDateTime = currentTime.AddSeconds(_otpConfiguration.OTPExpiryInSeconds*i);
                TimeSpan ts = GetEphocTimeSpan(movingFactorDateTime);
                var totalSeconds = (long) ts.TotalSeconds;
                var movingFactorForValidation = totalSeconds/_otpConfiguration.OTPExpiryInSeconds;
                result.Add(movingFactorForValidation);
            }
            return result;
        }

        private static TimeSpan GetEphocTimeSpan(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}