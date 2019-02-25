using Common;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebNew1
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
            //检验登录
            ResultEx res = new ResultEx();
            var SSO_Token = filterContext.HttpContext.Session["SSO_Token"];
            //获取returnurl
            string strHost = filterContext.HttpContext.Request.Url.Host;
            string strPort = filterContext.HttpContext.Request.Url.Port.ToString();
            string url = string.Format("http://{0}:{1}{2}", strHost, strPort, filterContext.HttpContext.Request.RawUrl);
            string returnurl = Regex.Replace(url, @"(\?|&)cert=.*", "", RegexOptions.IgnoreCase);
            if (SSO_Token != null)//验证token获取用户信息
            {
                res = IdentityByToken(SSO_Token.ToString());
                if (!res.Flag)
                {
                    //跳转到登录
                    filterContext.HttpContext.Session.Abandon();//清除session
                    filterContext.Result =  new RedirectResult(CommonConstants.SSO_URL + "/Home/Login?returnurl=" + returnurl);
                }
            }
            else if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["cert"]))//验证一次性cert
            {
                res = IdentityByCert(filterContext.HttpContext.Request.QueryString["cert"]);
                if (res.Flag)
                {
                    string token = res.Data.ToString();
                    //更新session
                    filterContext.HttpContext.Session["SSO_Token"] = token;
                    //验证token
                    res = IdentityByToken(token);
                }
                if (!res.Flag)
                {
                    //跳转到登录
                    filterContext.HttpContext.Session.Abandon();//清除session
                    filterContext.Result = new RedirectResult(CommonConstants.SSO_URL + "/Home/Login?returnurl=" + returnurl);
                }

            }
            else
            {
                //跳转到登录
                filterContext.Result = new RedirectResult(CommonConstants.SSO_URL + "/Home/Login?returnurl=" + returnurl);
            }
          

            base.OnActionExecuting(filterContext);
        }


        public ResultEx IdentityByToken(string token)
        {
            string url = CommonConstants.SSO_URL + "/SSO/Identity";
            //设置Http的正文
            string postdata = "{\"token\":\"" + token + "\"}";
            return CommonHelper.HttpClientPost(url, postdata);
        }

        public ResultEx IdentityByCert(string cert)
        {
            string url = CommonConstants.SSO_URL + "/SSO/Token";
            //设置Http的正文
            string postdata = "{\"cert\":\"" + cert + "\"}";
            return CommonHelper.HttpClientPost(url, postdata);
        }
       
    }
}
