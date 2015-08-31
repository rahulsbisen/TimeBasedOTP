namespace OneTimePassword.Impl.Utils
{
    public static class StringUtilities
    {
        public static bool StringEqualsInConstantTime(string lhs, string rhs)
        {
            if (lhs == null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            uint diff = (uint) lhs.Length ^ (uint) rhs.Length;

            for (int i = 0; i < lhs.Length && i < rhs.Length; i++)
            {
                diff |= lhs[i] ^ (uint) rhs[i];
            }

            return diff == 0;
        }
    }
}