using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSODemoNew1.Models
{
    public static class ControllerExtension
    {
        public static JsonpResult Jsonp(this Controller controller, object data)
        {
            JsonpResult result = new JsonpResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
    }
}