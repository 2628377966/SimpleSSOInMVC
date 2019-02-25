using Common;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Web1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginAttribute());

        }
    }

    /// <summary>
    /// 登录全局Filter （判断用户是否登录）
    /// </summary>
    public class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allowMous = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (!allowMous)
            {
                var url_token = System.Web.HttpContext.Current.Request["token"];
                var cookie_token = System.Web.HttpContext.Current.Request.Cookies.Get("token");

                if (cookie_token == null && string.IsNullOrEmpty(url_token))
                {
                    //重新登录
                    HttpContext.Current.Response.Redirect(TokenReplace(), false);
                }
                else if (cookie_token == null && !string.IsNullOrEmpty(url_token))
                {
                    //获取用户信息
                    using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
                    {
                        IDatabase db = redis.GetDatabase();
                        string user = db.StringGet(url_token);
                        if (string.IsNullOrEmpty(user))
                        {
                            //重新登录
                            HttpContext.Current.Response.Redirect(TokenReplace(), false);
                        }
                        else
                        {
                            //写入本地cookie
                            System.Web.HttpCookie authCookie = new System.Web.HttpCookie("token", url_token);
                            authCookie.Expires = DateTime.Now.AddMinutes(Int32.Parse(CommonConstants.Redis_Timeout));
                            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                        }
                    }
                }
                else if (cookie_token != null)
                {
                    //获取用户信息
                    using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
                    {
                        IDatabase db = redis.GetDatabase();
                        string user = db.StringGet(cookie_token.Value);
                        if (string.IsNullOrEmpty(user))
                        {
                            //重新登录
                            HttpContext.Current.Response.Redirect(TokenReplace(), false);
                        }
                    }
                }

            }

            base.OnActionExecuting(filterContext);
        }

        public string TokenReplace()
        {
            string strHost = HttpContext.Current.Request.Url.Host;
            string strPort = HttpContext.Current.Request.Url.Port.ToString();
            string url = string.Format("http://{0}:{1}{2}", strHost, strPort, HttpContext.Current.Request.RawUrl);
            url = Regex.Replace(url, @"(\?|&)Token=.*", "", RegexOptions.IgnoreCase);
            return $"{CommonConstants.SSO_URL}/login/login?appid={CommonConstants.WEB1_APPID}&appsecret={CommonConstants.WEB1_APPSECRET}&returnurl={url}";
        }
    }

}
