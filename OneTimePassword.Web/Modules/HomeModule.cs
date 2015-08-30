using System.Collections.Generic;
using System.Web;
using Nancy;

namespace OneTimePassword.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => {
                return View["Home"];
            };
        }
    }
}