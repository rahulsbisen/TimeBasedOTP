using System;

namespace OneTimePassword.Impl
{
    public interface IOTPAlgorithm
    {
        String GenerateOTP(String input, String secret, long movingFactor, int numberOfDigits);
    }
}