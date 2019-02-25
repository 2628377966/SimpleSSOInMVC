using Common;
using SSODemoNew1.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SSODemoNew1.Controllers
{
    public class HomeController : Controller
    {
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            if (Common.CurrentUser.IsLogin && !string.IsNullOrEmpty(ReturnUrl))//已经登录
            {
                return Redirect(SSOUrl(ReturnUrl));//生成密钥并返回
            }
            //...返回登录页面，正常流程。
            ViewBag.ReturnUrl = ReturnUrl; 
            return View();
        }

        //登录提交逻辑
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            //登录成功 && 分站的请求
            if (username == "admin" && password == "123456")
            {
                Guid token = Guid.NewGuid();
                var user = new CurrentUser.InfoModel { Token = token, UserName = "admin" };
                //登录成功保存cookie
                CurrentUser.Set(user);
                //在redis中保存token和用户信息
                //用户信息保存在redis中
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
                {
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(token.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(user), TimeSpan.FromMinutes(30));
                }
            }
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = SSOUrl(returnUrl);
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        public string SSOUrl(string url)
        {
            //添加cert和token的缓存，设置3小时有效
            var sso = HttpContext.Cache["SSO"] == null ? new List<SSO>() : (List<SSO>)HttpContext.Cache["SSO"];
            Guid cert = Guid.NewGuid();
            sso.Add(new SSO { Cert = cert, Token = CurrentUser.User.Token, Time = DateTime.Now.AddHours(3) });
            HttpContext.Cache["SSO"] = sso;
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.Contains('?'))
                {
                    var args = url.Substring(url.IndexOf('?') + 1).Split('&');
                    url = url.Substring(0, url.IndexOf('?') + 1);
                    foreach (var arg in args)
                    {
                        if (!arg.StartsWith("cert="))
                        {
                            url += arg;
                        }
                    }
                    url += "cert=" + cert;
                }
                else
                {
                    url += "?cert=" + cert;
                }
            }
            return url;
        }

        public ActionResult Logout()
        {
            var token = CurrentUser.User.Token.ToString();
            //清空cookie
            CurrentUser.Set(null);
            //删除用户信息
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
            {
                IDatabase db = redis.GetDatabase();
                db.KeyDelete(token);
            }
            return RedirectToAction("Login");
        }

    }


}