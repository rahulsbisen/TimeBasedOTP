using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace OneTimePassword.Web.Modules
{
    public class BootstrapModule : NancyModule
    {
        public BootstrapModule()
        {
            Get["/"] = parameters => {
                return View["Index"];
            };
        }
    }
}