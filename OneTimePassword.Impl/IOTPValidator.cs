using System;

namespace OneTimePassword.Impl
{
    public interface IOTPValidator
    {
        Boolean CheckOtp(String userId, String generatedOtp);
    }
}