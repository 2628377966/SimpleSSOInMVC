using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebNew1.Controllers
{
    public class HomeController : Controller
    {
    
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            var SSO_Token = Session["SSO_Token"];
            //SSO登出
            string url = CommonConstants.SSO_URL + "/SSO/Logout";
            string postdata = "{\"token\":\"" + SSO_Token.ToString() + "\"}";
            var res = CommonHelper.HttpClientPost(url, postdata);
            if (res.Flag)
            {
                //清除session
                Session.Abandon();
                return RedirectToAction("Index");
            }
            return Content("登出失败");
        }
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
    }
}