using Nancy;
using Nancy.ModelBinding;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;

namespace OneTimePassword.Web.Modules
{
    public class ValidateModule : NancyModule
    {
        public ValidateModule(IOTPService otpService)
        {
            Get["/Validate"] = parameters => {
                                                 return View["Validate"];
            };

            Post["/Validate"] = parameters =>
            {
                var validateOTPRequest = this.Bind<ValidateOTPRequest>();
                var validateOTPResponse = otpService.ValidateOtp(validateOTPRequest);
                return View["Validate", validateOTPResponse];
            };
        }
    }
}