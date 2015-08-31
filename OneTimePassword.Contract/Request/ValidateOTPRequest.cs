namespace OneTimePassword.Contract.Request
{
    public class ValidateOTPRequest
    {
        public string UserId { get; set; }
        public string OTP { get; set; }
    }
}