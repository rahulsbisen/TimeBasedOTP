using System;

namespace OneTimePassword.Impl.Algorithm
{
    public interface IOTPAlgorithm
    {
        String GenerateOTP(String input, String secret, long movingFactor, int numberOfDigits);
    }
}