namespace OneTimePassword.Contract.Response
{
    public class GenerateOTPResponse : BaseResponse
    {
        public string UserId { get; set; }
        public string OTP { get; set; }
    }
}