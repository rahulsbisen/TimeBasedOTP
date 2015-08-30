using System;
using System.Security.Cryptography;
using System.Text;

namespace OneTimePassword.Impl
{
    public class HmacBasedOTPAlgorithm : IOTPAlgorithm
    {
        private static readonly int[] DigitsPower = new int[]{ 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };

        /// <exception cref="ArgumentOutOfRangeException">Number of digits are not between 1 and 8 inclusive.</exception>
        public string GenerateOTP(String input, String secret, long movingFactor, int numberOfDigits)
        {
            ValidateInput(input, secret, numberOfDigits);

            var secretBytes = Encoding.UTF8.GetBytes(input + secret);
            var movingFactorBytes = BitConverter.GetBytes(movingFactor);

            HMACSHA1 mac = new HMACSHA1(secretBytes);
            var hash = mac.ComputeHash(movingFactorBytes);

            var intHash = BitConverter.ToInt32(hash,0);

            var otpResult = intHash % DigitsPower[numberOfDigits];
            var result = otpResult.ToString();
            return result.PadLeft(numberOfDigits, '0');
        }

        private static void ValidateInput(string input, string secret, int numberOfDigits)
        {
            if (numberOfDigits > 8 || numberOfDigits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDigits));
            }

            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            if (String.IsNullOrEmpty(secret))
            {
                throw new ArgumentOutOfRangeException(nameof(secret));
            }
        }
    }
}