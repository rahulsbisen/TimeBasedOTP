using System.Collections.Generic;

namespace OneTimePassword.Impl
{
    public interface IMovingFactorAlgorithm
    {
        long GetMovingFactor();
        List<long> GetMovingFactorForValidation();
    }
}