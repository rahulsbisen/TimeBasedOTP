using Nancy;
using Nancy.ModelBinding;
using OneTimePassword.Contract;
using OneTimePassword.Contract.Request;
using OneTimePassword.Contract.Response;

namespace OneTimePassword.Web.Modules
{
    public class GenerateModule : NancyModule
    {
        public GenerateModule(IOTPService otpService)
        {
            Get["/Generate"] = parameters => {return View["Generate"];
            };

            Post["/Generate"] = parameters =>
            {
                var generateOTPRequest = this.Bind<GenerateOTPRequest>();
                var generateOTPResponse = otpService.GenerateOtp(generateOTPRequest);
                return View["Generate", generateOTPResponse];
            };
        }
    }

    
}