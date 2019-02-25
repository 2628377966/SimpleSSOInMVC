using Common;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SSO
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
                var token = System.Web.HttpContext.Current.Request.Cookies.Get("token");
                if (token == null)
                {
                    //重新登录
                    HttpContext.Current.Response.Redirect(TokenReplace(), false);
                }
                else
                {
                    //获取用户信息
                    using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(CommonConstants.RedisServer))
                    {
                        IDatabase db = redis.GetDatabase();
                        string user = db.StringGet(token.Value);
                        if (string.IsNullOrEmpty(user))
                        {
                            //重新登录
                            HttpContext.Current.Response.Redirect(TokenReplace(), false);
                            token.Expires = DateTime.Now.AddDays(-1);//cookie设置过期
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
            return $"{CommonConstants.SSO_URL}/login/login?appid={CommonConstants.SSO_APPID}&appsecret={CommonConstants.SSO_APPSECRET}&returnurl={url}";
        }
    }
}
