using System;
using System.Collections.Generic;

namespace OneTimePassword.Impl
{
    public class ExpiryBasedMovingFactorAlgorithm : IMovingFactorAlgorithm
    {
        private readonly OTPConfiguration otpConfiguration;
        private readonly Func<DateTime> currentTimeFunction;

        public ExpiryBasedMovingFactorAlgorithm(OTPConfiguration otpConfiguration,
            Func<DateTime> currentTimeFunction = null)
        {
            this.currentTimeFunction = currentTimeFunction ?? (() => DateTime.Now);
            this.otpConfiguration = otpConfiguration;
        }

        public long GetMovingFactor()
        {
            var dateTime = currentTimeFunction();
            TimeSpan ts = GetEphocTimeSpan(dateTime);
            var totalSeconds = (long) ts.TotalSeconds;
            return totalSeconds/otpConfiguration.OTPExpiryInSeconds;
        }

        public List<long> GetMovingFactorForValidation()
        {
            var currentTime = currentTimeFunction();
            var result = new List<long>();
            for (int i = -1; i <= 0; i++)
            {
                var movingFactorDateTime = currentTime.AddSeconds(otpConfiguration.OTPExpiryInSeconds*i);
                TimeSpan ts = GetEphocTimeSpan(movingFactorDateTime);
                var totalSeconds = (long) ts.TotalSeconds;
                var movingFactorForValidation = totalSeconds/otpConfiguration.OTPExpiryInSeconds;
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