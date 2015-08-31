namespace OneTimePassword.Impl.Algorithm
{
    public interface IOTPAlgorithm
    {
        string GenerateOTP(string input, string secret, long movingFactor, int numberOfDigits);
    }
}