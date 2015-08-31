namespace OneTimePassword.Contract.Response
{
    public class ValidateOTPResponse : BaseResponse
    {
        public string UserId { get; set; }
        public bool Success { get; set; }
    }
}