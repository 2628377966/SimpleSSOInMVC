using Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SSO.Controllers
{
    public class LoginController : Controller
    {

        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string appid,string appsecret)
        {
            if (appid=="web1"&&appsecret== "a93c65a6-202d-4ef6-88d2-38a877368dee")
            {
                return View();
            }
            return Content("appid或者appsecret不正确");
            
        }
        [HttpPost]
        [AllowAnonymous]

        public ActionResult SaveLogin(string username, string password)
        {
            //根据username查询用户
            //检测password正确性
            //如果正确，写入redis
            var res = new Models.ResultEx();
            if (username == "admin" && password == "123456")
            {
                var user = new Models.User { UserName = username };
                string token = Guid.NewGuid().ToString();
                //将token信息写入cookie
                System.Web.HttpCookie authCookie = new System.Web.HttpCookie("token", token);
                authCookie.Expires = DateTime.Now.AddMinutes(int.Parse(CommonConstants.Redis_Timeout));
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                //用户信息保存在redis中
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
                {
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(token, Newtonsoft.Json.JsonConvert.SerializeObject(user), TimeSpan.FromMinutes(30));
                }
                res = new Models.ResultEx() { Flag = true, Data = new { token = token } };
                return Json(res);
            }

            res = new Models.ResultEx() { Flag = false };
            return Json(res);  
        }

   
    }
}