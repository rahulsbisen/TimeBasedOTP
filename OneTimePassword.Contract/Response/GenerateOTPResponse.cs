using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneTimePassword.Contract.Response
{
    public class GenerateOTPResponse : BaseResponse
    {
        public String UserId { get; set; }
        public String OTP { get; set; }
    }
}
