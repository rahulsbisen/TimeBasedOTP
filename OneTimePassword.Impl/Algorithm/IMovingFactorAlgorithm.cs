using System.Collections.Generic;

namespace OneTimePassword.Impl.Algorithm
{
    public interface IMovingFactorAlgorithm
    {
        long GetMovingFactor();
        List<long> GetMovingFactorForValidation();
    }
}