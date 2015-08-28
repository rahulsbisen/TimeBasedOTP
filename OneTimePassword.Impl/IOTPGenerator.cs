using System;

namespace OneTimePassword.Impl
{
    public interface IOTPGenerator
    {
        String CreateOTP(String secret);
    }
}